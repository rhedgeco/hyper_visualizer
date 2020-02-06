using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using SFB;
using UI;
using UnityEngine;

namespace HyperCoreScripts
{
    public class AudioManager
    {
        public static bool Playing { get; internal set; }
        
        private static AudioSource _source;

        public static AudioSource Source
        {
            get => _source;
            internal set
            {
                _source = value;
                _source.loop = false;
            }
        }

        public static AudioClip Clip
        {
            get => Source.clip;
        }

        public static AudioClip DefaultAudio
        {
            set => Source.clip = value;
        }

        public static string AudioTitle
        {
            get => Source.clip.name;
        }

        internal static IEnumerator ImportAudioRoutine(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File '{path}' could not be found.");
                StatusController.UpdateStatus($"File '{path}' could not be found.");
                yield break;
            }

            // Load the wave file reader or convert if it is an mp3
            WaveFileReader reader;
            if (Path.HasExtension(path) &&
                Path.GetExtension(path).Equals(".mp3", StringComparison.InvariantCultureIgnoreCase))
            {
                OverlayController.Loading.StartLoading("Converting mp3 to wav...");
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                reader = GetWavReaderFromMp3(path);
                if (reader == null)
                {
                    Debug.LogError($"Error converting file");
                    StatusController.UpdateStatus($"Error converting file");
                    OverlayController.Loading.EndLoading();
                    yield break;
                }
            }
            else if (Path.HasExtension(path) &&
                     Path.GetExtension(path).Equals(".wav", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    reader = new WaveFileReader(path);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading file: {e.Message}");
                    StatusController.UpdateStatus($"Error loading file: {e.Message}");
                    OverlayController.Loading.EndLoading();
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Unsupported file extension.");
                StatusController.UpdateStatus("Unsupported file extension.");
                OverlayController.Loading.EndLoading();
                yield break;
            }

            if (!reader.CanRead)
            {
                Debug.LogError("Cannot read audio file.");
                StatusController.UpdateStatus("Cannot read audio file.");
                OverlayController.Loading.EndLoading();
                yield break;
            }

            StatusController.UpdateStatus($"Reading wav file : {Path.GetFileNameWithoutExtension(path)}");
            OverlayController.Loading.StartLoading($"Reading wav file : {Path.GetFileNameWithoutExtension(path)}");
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
                    OverlayController.Loading.UpdateLoading(percent);
                    timeCheck = Time.realtimeSinceStartup;
                    yield return null;
                }
            }

            float[] samples = sampleList.ToArray();
            AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(path),
                samples.Length / reader.WaveFormat.Channels,
                reader.WaveFormat.Channels,
                reader.WaveFormat.SampleRate, false);
            clip.SetData(samples, 0);
            HyperCore.TotalTime = clip.length;
            _source.clip = clip;
            StatusController.UpdateStatus("Loaded Audio Data.");
            OverlayController.Loading.EndLoading();
            yield return null;
        }
        
        // Converts an mp3 to wave on disk and returns the wavreader
        private static WaveFileReader GetWavReaderFromMp3(string mp3Path)
        {
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "TempConversion");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string outpath = Path.Combine(dir, Path.GetFileName(mp3Path) ?? throw new NullReferenceException());
                if (File.Exists(outpath)) File.Delete(outpath);

                using (var reader = new Mp3FileReader(mp3Path))
                {
                    WaveFileWriter.CreateWaveFile(outpath, reader);
                }

                WaveFileReader wavReader = new WaveFileReader(outpath);
                StatusController.UpdateStatus("Converted mp3 to wav...");
                return wavReader;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error converting mp3 file: {e.Message}");
                StatusController.UpdateStatus($"Error converting mp3 file: {e.Message}");
                return null;
            }
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