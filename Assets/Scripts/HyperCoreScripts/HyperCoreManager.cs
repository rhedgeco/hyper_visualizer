using System;
using System.Collections;
using NatCorder;
using NatCorder.Clocks;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCoreScripts
{
    public class HyperCoreManager : MonoBehaviour
    {
        [SerializeField] private int fps = 60;
        [SerializeField] private Slider timelineSlider;
        [SerializeField] private AudioClip testAudio; //TODO: Replace with actual audio import

        private static AudioSource _source;

        public static bool Playing { get; internal set; } = false;

        private void Awake()
        {
            HyperCore.TotalTime = testAudio.length;
            timelineSlider.onValueChanged.AddListener(QuickRenderFrame);
            gameObject.AddComponent<AudioListener>();
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.loop = false;
            _source.clip = testAudio;
        }

        private void Update()
        {
            if(Playing)
            {
                if(!_source.isPlaying) _source.Play();
            }
            else
            {
                if(_source.isPlaying) _source.Pause();
            }
            SetTimelinePos(_source.time / _source.clip.length);
        }

        public static void TogglePlay()
        {
            Playing = !Playing;
        }

        public void StopPlaying()
        {
            Playing = false;
            _source.Stop();
        }

        private void SetTimelinePos(float value)
        {
            timelineSlider.value = value; // Has a listener that calls QuickRenderFrame
        }

        private void QuickRenderFrame(float value)
        {
            HyperCore.Time = HyperCore.TotalTime * value;
            _source.time = HyperCore.Time;
            UpdateHyperFrame();
            MainRenderer.RenderFrame();
        }

        private void UpdateHyperFrame()
        {
            HyperValues values = new HyperValues(0f, new float[2], 1f);
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

            for (int frame = 0; frame <= length * fps; frame++)
            {
                yield return new WaitForEndOfFrame();
                SetTimelinePos(frame / (length * fps));
                long timestamp = clock.Timestamp;
                Texture2D fTex = MainRenderer.GetFrame();
                float[] commitSamples = GetPartialSampleArray(samples, samplesPerFrame * frame, samplesPerFrame);
                recorder.CommitFrame(fTex.GetPixels32(), timestamp);
                recorder.CommitSamples(commitSamples, timestamp);
                DestroyImmediate(fTex);
                clock.Tick();
                Debug.Log($"Generated Frame {frame}/{(int) (length * fps)}");
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