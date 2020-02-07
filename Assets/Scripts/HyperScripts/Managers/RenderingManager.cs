using System.Collections;
using System.IO;
using NatCorder;
using NatCorder.Clocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HyperScripts.Managers
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

        public static bool Rendering { get; private set; }

        internal static IEnumerator RenderRoutine(string outputPath)
        {
            HyperCoreRuntime.DisableSound();
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
                        StatusManager.UpdateStatus("ERROR: Could not locate saved file. Contact dev.");
                        return;
                    }

                    if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                    {
                        Debug.LogError("ERROR: Output path is not accessible.");
                        StatusManager.UpdateStatus("ERROR: Output path is not accessible.");
                        return;
                    }

                    if (File.Exists(outputPath)) File.Delete(outputPath);
                    File.Move(s, outputPath);

                    Debug.Log("Finished Rendering");
                    StatusManager.UpdateStatus("Finished Rendering");
                    OverlayManager.Loading.EndLoading();
                });
            FixedIntervalClock clock = new FixedIntervalClock(Fps);

            OverlayManager.Loading.StartLoading("Rendering HyperVisualization\n\n" +
                                                $"frame: 0/{(int) (length * Fps)}");
            StatusManager.UpdateStatus("Rendering HyperVideo");
            yield return new WaitForEndOfFrame();

            Rendering = true;
            AudioManager.StopPlaying();
            for (int frame = 0; frame <= length * Fps; frame++)
            {
                yield return new WaitForEndOfFrame();
                HyperCoreRuntime.UpdateHyperFrame(frame / (length * Fps),
                    0f, AudioManager.GetSpectrumData(samplesPerFrame * frame / channels, 2048, 0),
                    AudioManager.GetSpectrumData(samplesPerFrame * frame / channels, 2048, 1), 0f);
                long timestamp = clock.Timestamp;
                Texture2D fTex = MainRenderer.GetFrame();
                float[] commitSamples = AudioManager.GetPartialSampleArray(samplesPerFrame * frame, samplesPerFrame);
                recorder.CommitFrame(fTex.GetPixels32(), timestamp);
                recorder.CommitSamples(commitSamples, timestamp);
                Object.DestroyImmediate(fTex);

                float percent = (float) frame / (int) (length * Fps);
                StatusManager.UpdateStatus($"Generated Frame {frame}/{(int) (length * Fps)}");
                OverlayManager.Loading.UpdateLoading("Rendering HyperVisualization\n\n" +
                                                     $"frame: {frame}/{(int) (length * Fps)}", percent);
            }

            OverlayManager.Loading.UpdateLoading("Finalizing Export", 1);
            recorder.Dispose();
            HyperCoreRuntime.EnableSound();
        }
    }
}