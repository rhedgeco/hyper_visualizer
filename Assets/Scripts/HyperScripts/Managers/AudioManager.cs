using System;
using System.Collections.Generic;
using System.IO;
using hyper_engine;
using HyperScripts.Threading;
using Plugins.Free.FFT;
using UnityEngine;

namespace HyperScripts.Managers
{
    public static class AudioManager
    {
        private static AudioSource _source;
        private static float[] _samples;
        private static List<double[][]> _fftCache = new List<double[][]>();
        private static int _fftSmoothing = 8;

        public static string AudioTitle => Source.clip.name;

        public static bool Playing { get; private set; }

        internal static int FftSmoothing
        {
            get => _fftSmoothing;
            set
            {
                _fftSmoothing = value;
                PurgeFftCache();
            }
        }

        internal static AudioClip Clip => Source.clip;

        internal static AudioSource Source
        {
            get => _source;
            set
            {
                _source = value;
                _source.loop = false;
            }
        }

        internal static AudioClip DefaultAudio
        {
            set
            {
                Source.clip = value;
                RecalculateSamples();
            }
        }

        internal static int CachedFftFrames => _fftCache.Count;

        internal static bool CanCache { get; set; } = true;

        private static float[] Samples
        {
            get
            {
                if (_samples == null) RecalculateSamples();
                return _samples;
            }
        }

        private static void RecalculateSamples()
        {
            _samples = new float[Clip.samples * Clip.channels];
            Clip.GetData(_samples, 0);
        }

        internal static void ImportAudioThreaded(string path)
        {
            AudioDecodeWorker worker = new AudioDecodeWorker(path,
                Path.Combine(Application.persistentDataPath, "TempConversion"),
                (sampleArray, channels, samplerate) =>
                {
                    AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(path),
                        sampleArray.Length / channels, channels, samplerate, false);
                    clip.SetData(sampleArray, 0);
                    HyperCoreRuntime.coreChange.SetTotalTime(clip.length);
                    Source.clip = clip;
                    _samples = sampleArray;
                    StatusManager.UpdateStatus("Loaded Audio Data.");
                });

            HyperThreadDispatcher.StartWorker(worker);
        }

        internal static void CacheFftThreaded()
        {
            CanCache = false;
            FftCachingWorker worker =
                new FftCachingWorker(_samples, RenderingManager.FrameCount, 8192, FftSmoothing);

            HyperThreadDispatcher.StartWorker(worker);
        }

        internal static double[] GetSpectrumData(int startIndex, int length, int channel)
        {
            double[] spec = Windowing.HackyRyanWindow(_samples, startIndex, length, channel, FftSmoothing);
            LomontFFT fft = new LomontFFT();
            fft.RealFFT(spec, true);
            return spec;
        }

        internal static float[] GetPartialSampleArray(int startIndex, int length)
        {
            float[] partial = new float[length];
            if (Samples.Length - startIndex < length) length = Samples.Length - startIndex;
            if (length >= 0) Array.Copy(Samples, startIndex, partial, 0, length);
            return partial;
        }

        internal static float GetMaxValueInSamples(float[] samples, bool absoluteValue = true)
        {
            float max = samples[0];
            foreach (float sample in samples)
            {
                float s = absoluteValue ? Mathf.Abs(sample) : sample;
                if (sample > max) max = sample;
            }

            return max;
        }

        internal static float GetMaxValueInSamplesFromSource(int startIndex, int length, bool absoluteValue = true)
        {
            float max = _samples[startIndex];
            for (int i = startIndex; i < startIndex + length; i++)
            {
                float s = absoluteValue ? Mathf.Abs(_samples[i]) : _samples[i];
                if (_samples[i] > max) max = _samples[i];
            }

            return max;
        }

        internal static void UpdateAudioState()
        {
            if (RenderingManager.Rendering) return;

            if (Playing)
            {
                if (!Source.isPlaying) Source.Play();
            }
            else
            {
                if (Source.isPlaying) Source.Pause();
            }

            if (Source.timeSamples >= Clip.samples - (Clip.frequency / 10))
            {
                Playing = false; // Stop playing if clip is past due
                Source.Pause();
                Source.timeSamples = Clip.samples - Clip.channels;
            }
        }

        internal static void UpdateAudioToTimeline()
        {
            float value = TimelineManager.Timeline.value;
            AudioClip clip = Source.clip;
            int sampleTarget = (int) (clip.samples * value);
            if (sampleTarget == clip.samples) sampleTarget -= clip.channels;
            Source.timeSamples = sampleTarget;
        }

        internal static void PurgeFftCache()
        {
            _fftCache.Clear();
            CanCache = true;
        }

        internal static void AddFftCache(double[][] cacheFrame)
        {
            _fftCache.Add(cacheFrame);
        }

        internal static bool GetCachedFftFrame(int frameNum, out double[] frame, int channel)
        {
            frame = new double[0];
            if (_fftCache.Count <= frameNum) return false;
            frame = _fftCache[frameNum][channel];
            return true;
        }

        internal static void TogglePlay()
        {
            if (RenderingManager.Rendering) return;
            if (!Playing && Source.timeSamples >= Clip.samples - Clip.channels)
            {
                Source.timeSamples = 0;
                Playing = true;
            }
            else Playing = !Playing;
        }

        internal static void StopPlaying()
        {
            Playing = false;
            Source.Stop();
            Source.timeSamples = 0;
            TimelineManager.UpdateTimelineState();
        }

        internal static void SkipTime(float time)
        {
            int targetTime = Source.timeSamples + (int) (time * Clip.frequency);
            if (targetTime < 0) targetTime = 0;
            if (targetTime > Clip.samples - Clip.channels) targetTime = Clip.samples - Clip.channels;
            Source.timeSamples = targetTime;
        }
    }
}