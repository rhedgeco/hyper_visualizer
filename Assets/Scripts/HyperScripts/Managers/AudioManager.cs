using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HyperScripts.Threading;
using NAudio.Wave;
using UnityEngine;

namespace HyperScripts.Managers
{
    public static class AudioManager
    {
        private static AudioSource _source;
        private static float[] _samples;

        public static bool Playing { get; private set; }

        internal static AudioSource Source
        {
            get => _source;
            set
            {
                _source = value;
                _source.loop = false;
            }
        }

        internal static AudioClip Clip => Source.clip;

        internal static AudioClip DefaultAudio
        {
            set
            {
                Source.clip = value;
                RecalculateSamples();
            }
        }

        public static string AudioTitle => Source.clip.name;

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

        internal static IEnumerator ImportAudioRoutine(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File '{path}' could not be found.");
                StatusManager.UpdateStatus($"File '{path}' could not be found.");
                yield break;
            }

            AudioDecodeWorker worker = new AudioDecodeWorker(path, 
                Path.Combine(Application.persistentDataPath, "TempConversion"),
                (sampleArray, channels, samplerate) =>
            {
                AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(path),
                    sampleArray.Length / channels, channels, samplerate, false);
                clip.SetData(sampleArray, 0);
                HyperCore.TotalTime = clip.length;
                Source.clip = clip;
                _samples = sampleArray;
                StatusManager.UpdateStatus("Loaded Audio Data.");
            });
            
            HyperThreadDispatcher.StartWorker(worker);
        }

        internal static float[] GetPartialSampleArray(int startIndex, int length)
        {
            float[] partial = new float[length];
            if (Samples.Length - startIndex < length) length = Samples.Length - startIndex;
            Array.Copy(Samples, startIndex, partial, 0, length);
            return partial;
        }

        internal static void UpdateAudioState()
        {
            if (!RenderingManager.Rendering)
            {
                if (Playing)
                {
                    if (!Source.isPlaying) Source.Play();
                }
                else
                {
                    if (Source.isPlaying) Source.Pause();
                }

                if (TimelineManager.Timeline.value >= RenderingManager.TimelineSliderMaxValue)
                    Playing = false; // Stop playing if clip is past due
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

        public static void TogglePlay()
        {
            if (RenderingManager.Rendering) return;
            if (!Playing && TimelineManager.Timeline.value >= RenderingManager.TimelineSliderMaxValue)
            {
                TimelineManager.Timeline.value = 0;
                Playing = true;
            }
            else Playing = !Playing;
        }

        public static void StopPlaying()
        {
            Playing = false;
            Source.Stop();
            Source.timeSamples = 0;
            TimelineManager.UpdateTimelineState();
        }
    }
}