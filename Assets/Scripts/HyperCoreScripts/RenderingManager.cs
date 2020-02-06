using System.Collections;
using System.IO;
using NatCorder;
using NatCorder.Clocks;
using UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HyperCoreScripts
{
    public static class RenderingManager
    {
        public const float TimelineSliderMaxValue = 0.9999f;
        
        private static int _fps = 30;

        public static int Fps
        {
            get => _fps;
            set
            {
                if (value > 99) _fps = 99;
                if (value < 1) _fps = 1;
                _fps = value;
            }
        }

        internal static IEnumerator RenderRoutine(string outputPath)
        {
            AudioClip clip = AudioManager.Clip;
            float length = clip.length;
            int channels = clip.channels;
            int audioSamples = clip.frequency;
            int samplesPerFrame = audioSamples / Fps * channels;

            MP4Recorder recorder = new MP4Recorder(MainRenderer.Width, MainRenderer.Height, Fps, audioSamples, channels,
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
            FixedIntervalClock clock = new FixedIntervalClock(Fps);

            OverlayController.Loading.StartLoading("Rendering HyperVisualization\n\n" +
                                                   $"frame: 0/{(int) (length * Fps)}");
            StatusController.UpdateStatus("Rendering HyperVideo");
            yield return new WaitForEndOfFrame();

            for (int frame = 0; frame <= length * Fps; frame++)
            {
                yield return new WaitForEndOfFrame();
                TimelineManager.SetTimelinePos(frame / (length * Fps));
                long timestamp = clock.Timestamp;
                Texture2D fTex = MainRenderer.GetFrame();
                float[] commitSamples = AudioManager.GetPartialSampleArray(samplesPerFrame * frame, samplesPerFrame);
                recorder.CommitFrame(fTex.GetPixels32(), timestamp);
                recorder.CommitSamples(commitSamples, timestamp);
                Object.DestroyImmediate(fTex);

                float percent = (float) frame / (int) (length * Fps);
                StatusController.UpdateStatus($"Generated Frame {frame}/{(int) (length * Fps)}");
                OverlayController.Loading.UpdateLoading("Rendering HyperVisualization\n\n" +
                                                        $"frame: {frame}/{(int) (length * Fps)}", percent);
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
            HyperCoreManager.UpdateHyperFrame();
            MainRenderer.RenderFrame();
        }
    }
}