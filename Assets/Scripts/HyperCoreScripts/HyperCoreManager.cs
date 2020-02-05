using System;
using System.Collections;
using NatCorder;
using NatCorder.Clocks;
using UnityEngine;

namespace HyperCoreScripts
{
    public class HyperCoreManager : MonoBehaviour
    {
        [SerializeField] private int fps = 60;
        [SerializeField] private AudioClip testAudio; //TODO: Replace with actual audio import

        private static void UpdateHyperFrame()
        {
            HyperValues values = new HyperValues(0f, 0f, new float[2], 1f);
            HyperCore.DeltaTime = 0f; //TODO: Adjust delta
            HyperCore.BeginFrame.Invoke(values);
            HyperCore.UpdateFrame.Invoke(values);
            HyperCore.EndFrame.Invoke(values);
        }
        
        public void RenderFootage(float length)
        {
            StartCoroutine(RenderRoutine(length));
        }

        private IEnumerator RenderRoutine(float length)
        {
            int audioSamples = testAudio.frequency;
            int channels = testAudio.channels;
            float[] samples = new float[testAudio.samples * channels];
            testAudio.GetData(samples, 0);
            int samplesPerFrame = audioSamples / fps;

            MP4Recorder recorder = new MP4Recorder(MainRenderer.Width, MainRenderer.Height, fps, audioSamples,
                channels, s => { Debug.Log(s); });
            FixedIntervalClock clock = new FixedIntervalClock(fps, false);

            for (int frame = 0; frame < length * fps; frame++)
            {
                yield return new WaitForEndOfFrame();
                long timestamp = clock.Timestamp;
                Texture2D fTex = MainRenderer.GetFrame(true);
                float[] commitSamples = GetPartialSampleArray(samples, samplesPerFrame * frame, samplesPerFrame);
                recorder.CommitFrame(fTex.GetPixels32(), timestamp);
                recorder.CommitSamples(commitSamples, timestamp);
                DestroyImmediate(fTex);
                UpdateHyperFrame();
                clock.Tick();
                Debug.Log($"Generated Frame {frame}/{(int) (length * fps) - 1}");
            }

            recorder.Dispose();
        }

        // Copies a smaller section of the sample array
        private float[] GetPartialSampleArray(float[] samples, int startIndex, int length)
        {
            float[] partial = new float[length];
            Array.Copy(samples, startIndex, partial, 0, length);
            return partial;
        }
    }
}