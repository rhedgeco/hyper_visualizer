using System;
using System.Collections;
using System.IO;
using NatCorder;
using NatCorder.Clocks;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HyperCoreScripts
{
    public class RenderingManager
    {
        public const float TimelineSliderMaxValue = 0.9999f;
        
        private static int fps = 30;

        public static int Fps
        {
            get => fps;
            set
            {
                if (value > 99) fps = 99;
                if (value < 1) fps = 1;
                fps = value;
            }
        }

        internal static IEnumerator RenderRoutine(string outputPath)
        {
            AudioClip clip = AudioManager.Clip;
            int fps = Fps;
            float length = clip.length;
            int channels = clip.channels;
            int audioSamples = clip.frequency;
            float[] samples = new float[clip.samples * channels];
            clip.GetData(samples, 0);
            int samplesPerFrame = audioSamples / fps * channels;

            MP4Recorder recorder = new MP4Recorder(MainRenderer.Width, MainRenderer.Height, fps, audioSamples, channels,
                s =>
                {
                    // Set up to move file to output location after rendering
                    if (!File.Exists(s))
                    {
                        Debug.LogError("ERROR: Could not locate saved file. Contact dev.");
                        StatusController.UpdateStatus("ERROR: Could not locate saved file. Contact dev.");
                        return;
                    }

                    if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                    {
                        Debug.LogError("ERROR: Output path is not accessible.");
                        StatusController.UpdateStatus("ERROR: Output path is not accessible.");
                        return;
                    }

                    if (File.Exists(outputPath)) File.Delete(outputPath);
                    File.Move(s, outputPath);

                    Debug.Log("Finished Rendering");
                    StatusController.UpdateStatus("Finished Rendering");
                    OverlayController.Loading.EndLoading();
                });
            FixedIntervalClock clock = new FixedIntervalClock(fps);

            OverlayController.Loading.StartLoading($"Rendering HyperVisualization\n\n" +
                                                   $"frame: 0/{(int) (length * fps)}");
            StatusController.UpdateStatus("Rendering HyperVideo");
            yield return new WaitForEndOfFrame();

            for (int frame = 0; frame <= length * fps; frame++)
            {
                yield return new WaitForEndOfFrame();
                TimelineManager.SetTimelinePos(frame / (length * fps));
                long timestamp = clock.Timestamp;
                Texture2D fTex = MainRenderer.GetFrame();
                float[] commitSamples = GetPartialSampleArray(samples, samplesPerFrame * frame, samplesPerFrame);
                recorder.CommitFrame(fTex.GetPixels32(), timestamp);
                recorder.CommitSamples(commitSamples, timestamp);
                Object.DestroyImmediate(fTex);

                float percent = (float) frame / (int) (length * fps);
                StatusController.UpdateStatus($"Generated Frame {frame}/{(int) (length * fps)}");
                OverlayController.Loading.UpdateLoading($"Rendering HyperVisualization\n\n" +
                                                        $"frame: {frame}/{(int) (length * fps)}", percent);
            }

            OverlayController.Loading.UpdateLoading("Finalizing Export", 1);
            recorder.Dispose();
        }

        internal static void QuickRenderFrame(float value)
        {
            AudioClip clip = AudioManager.Source.clip;
            HyperCore.Time = HyperCore.TotalTime * value;
            int sampleTarget = (int) (clip.samples * value);
            if (sampleTarget == clip.samples) sampleTarget -= clip.channels;
            AudioManager.Source.timeSamples = sampleTarget;
            UpdateHyperFrame();
            MainRenderer.RenderFrame();
        }
        
        private static void UpdateHyperFrame()
        {
            HyperValues values = new HyperValues(0f, new float[2], 1f);
            HyperCore.BeginFrame.Invoke(values);
            HyperCore.UpdateFrame.Invoke(values);
            HyperCore.EndFrame.Invoke(values);
        }

        // Copies a smaller section of the sample array
        private static float[] GetPartialSampleArray(float[] samples, int startIndex, int length)
        {
            float[] partial = new float[length];
            if (samples.Length - startIndex < length) length = samples.Length - startIndex;
            Array.Copy(samples, startIndex, partial, 0, length);
            return partial;
        }
    }
}