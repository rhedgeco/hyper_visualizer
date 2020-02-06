using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using UI;
using UnityEngine;

namespace HyperCoreScripts.Managers
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

        private static WaveFileReader GetWavReaderFromMp3(string mp3Path)
        {
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "TempConversion");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string outPath = Path.Combine(dir, Path.GetFileName(mp3Path) ?? throw new NullReferenceException());
                if (File.Exists(outPath)) File.Delete(outPath);

                using (var reader = new Mp3FileReader(mp3Path))
                {
                    WaveFileWriter.CreateWaveFile(outPath, reader);
                }

                WaveFileReader wavReader = new WaveFileReader(outPath);
                StatusManager.UpdateStatus("Converted mp3 to wav...");
                return wavReader;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error converting mp3 file: {e.Message}");
                StatusManager.UpdateStatus($"Error converting mp3 file: {e.Message}");
                return null;
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

            // Load the wave file reader or convert if it is an mp3
            WaveFileReader reader;
            if (Path.HasExtension(path) &&
                // ReSharper disable once PossibleNullReferenceException
                Path.GetExtension(path).Equals(".mp3", StringComparison.InvariantCultureIgnoreCase))
            {
                OverlayManager.Loading.StartLoading("Converting mp3 to wav...");
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                reader = GetWavReaderFromMp3(path);
                if (reader == null)
                {
                    Debug.LogError("Error converting file");
                    StatusManager.UpdateStatus("Error converting file");
                    OverlayManager.Loading.EndLoading();
                    yield break;
                }
            }
            else if (Path.HasExtension(path) &&
                     // ReSharper disable once PossibleNullReferenceException
                     Path.GetExtension(path).Equals(".wav", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    reader = new WaveFileReader(path);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading file: {e.Message}");
                    StatusManager.UpdateStatus($"Error loading file: {e.Message}");
                    OverlayManager.Loading.EndLoading();
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Unsupported file extension.");
                StatusManager.UpdateStatus("Unsupported file extension.");
                OverlayManager.Loading.EndLoading();
                yield break;
            }

            if (!reader.CanRead)
            {
                Debug.LogError("Cannot read audio file.");
                StatusManager.UpdateStatus("Cannot read audio file.");
                OverlayManager.Loading.EndLoading();
                yield break;
            }

            StatusManager.UpdateStatus($"Reading wav file : {Path.GetFileNameWithoutExtension(path)}");
            OverlayManager.Loading.StartLoading($"Reading wav file : {Path.GetFileNameWithoutExtension(path)}");
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            List<float> sampleList = new List<float>();
            float[] sampleFrame;
            float timeCheck = Time.realtimeSinceStartup;
            while ((sampleFrame = reader.ReadNextSampleFrame()) != null) //TODO: Load Asynchronously
            {
                sampleList.AddRange(sampleFrame);
                if (Time.realtimeSinceStartup > timeCheck + 0.5)
                {
                    float percent = (float) reader.Position / reader.Length;
                    OverlayManager.Loading.UpdateLoading(percent);
                    timeCheck = Time.realtimeSinceStartup;
                    yield return null;
                }
            }

            float[] sampleArray = sampleList.ToArray();
            AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(path),
                sampleArray.Length / reader.WaveFormat.Channels,
                reader.WaveFormat.Channels,
                reader.WaveFormat.SampleRate, false);
            clip.SetData(sampleArray, 0);
            HyperCore.TotalTime = clip.length;
            Source.clip = clip;
            RecalculateSamples();
            StatusManager.UpdateStatus("Loaded Audio Data.");
            OverlayManager.Loading.EndLoading();
            yield return null;
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

        public static void TogglePlay()
        {
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
            TimelineManager.SetTimelinePos(0);
        }
    }
}