using System;
using System.Collections;
using NatCorder;
using NatCorder.Clocks;
using UnityEngine;
using UnityEngine.Events;

namespace HyperCore
{
    public class HyperCoreManager : MonoBehaviour
    {
        [SerializeField] private MainRenderer _mainRenderer;
        [SerializeField] private int fps = 60;
        [SerializeField] private AudioClip _testAudio; //TODO: Replace with actual audio import

        private static HyperCore Core { get; } = new HyperCore();
        public static long CurrentFrame { get; private set; }
        
        public static float DeltaTime { get; private set; }

        public static void PushFrame(long frameNumber)
        {
            if (frameNumber == CurrentFrame) return;
            Core.UpdateFrame.Invoke(new HyperValues(0f, 0f, new float[2], 1f));
            CurrentFrame = frameNumber;
        }

        public static void ConnectFrameUpdate(UnityAction<HyperValues> method)
        {
            Core.UpdateFrame.AddListener(method);
        }

        private void UpdateFrame()
        {
            DeltaTime = 0f; //TODO: Adjust delta
            Core.UpdateFrame.Invoke(new HyperValues(0f, 0f, new float[2], 1f));
        }

        public void RenderFootage(float length)
        {
            StartCoroutine(RenderRoutine(length));
        }

        private IEnumerator RenderRoutine(float length)
        {
            int audioSamples = _testAudio.frequency;
            int channels = _testAudio.channels;
            float[] samples = new float[_testAudio.samples * channels];
            _testAudio.GetData(samples, 0);
            int samplesPerFrame = audioSamples / fps;

            MP4Recorder recorder = new MP4Recorder(_mainRenderer.Width, _mainRenderer.Height, fps, audioSamples,
                channels, s => { Debug.Log(s); });
            FixedIntervalClock clock = new FixedIntervalClock(fps, false);

            for (int frame = 0; frame < length * fps; frame++)
            {
                yield return new WaitForEndOfFrame();
                long timestamp = clock.Timestamp;
                Texture2D fTex = _mainRenderer.GetFrame(true);
                float[] commitSamples = GetPartialSampleArray(samples, samplesPerFrame * frame, samplesPerFrame);
                recorder.CommitFrame(fTex.GetPixels32(), timestamp);
                recorder.CommitSamples(commitSamples, timestamp);
                DestroyImmediate(fTex);
                UpdateFrame();
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