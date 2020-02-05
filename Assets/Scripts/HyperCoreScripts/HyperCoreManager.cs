﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NatCorder;
using NatCorder.Clocks;
using NAudio.Wave;
using SFB;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCoreScripts
{
    public class HyperCoreManager : MonoBehaviour
    {
        [SerializeField] private int fps = 60;
        [SerializeField] private Slider timelineSlider;
        [SerializeField] private AudioClip testAudio; //TODO: Replace with actual audio import
        [SerializeField] private float arrowSkipAmount = 5f;

        private static HyperCoreManager _instance;
        private static AudioSource _source;
        private const float TIMELINE_SLIDER_MAX_VALUE = 0.9999f;

        public static bool Playing { get; internal set; }

        private void Awake()
        {
            // Set up singleton
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(this);
            DontDestroyOnLoad(gameObject);

            HyperCore.TotalTime = testAudio.length;
            timelineSlider.onValueChanged.AddListener(QuickRenderFrame);
            gameObject.AddComponent<AudioListener>();
            _source = gameObject.AddComponent<AudioSource>();
            _source.loop = false;
            _source.clip = testAudio;
        }

        private void Start()
        {
            // Has to be played once for some reason, otherwise the 'time' cannot be modified
            _source.Play();
        }

        private void Update()
        {
            AudioClip clip = _source.clip;

            // Handle keyboard control over the timeline
            if (Input.GetButtonDown($"TimelineLeft")) timelineSlider.value -= arrowSkipAmount / clip.length;
            if (Input.GetButtonDown($"TimelineRight")) timelineSlider.value += arrowSkipAmount / clip.length;
            if (Input.GetButtonDown($"PlayPause")) TogglePlay();

            if (timelineSlider.value >= TIMELINE_SLIDER_MAX_VALUE) Playing = false; // Stop playing if clip is past due
            if (Playing)
            {
                if (!_source.isPlaying) _source.Play();
            }
            else
            {
                if (_source.isPlaying) _source.Pause();
            }

            SetTimelinePos((float) _source.timeSamples / clip.samples);
        }

        public static void TogglePlay()
        {
            if (!Playing && _instance.timelineSlider.value >= TIMELINE_SLIDER_MAX_VALUE)
            {
                _instance.timelineSlider.value = 0;
                Playing = true;
            }
            else Playing = !Playing;
        }

        public void StopPlaying()
        {
            Playing = false;
            _source.Stop();
            SetTimelinePos(0);
        }

        private void SetTimelinePos(float value)
        {
            timelineSlider.value = value; // Has a listener that calls QuickRenderFrame
        }

        private void QuickRenderFrame(float value)
        {
            AudioClip clip = _source.clip;
            HyperCore.Time = HyperCore.TotalTime * value;
            int sampleTarget = (int) (clip.samples * value);
            if (sampleTarget == clip.samples) sampleTarget -= clip.channels;
            _source.timeSamples = sampleTarget;
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

        public void ImportAudioFromFile()
        {
            string path = StandaloneFileBrowser.OpenFilePanel("Open Audio File", "", new[]
            {
                new ExtensionFilter("Audio Files", "wav"),
                new ExtensionFilter("All Files", "*"),
            }, false)[0];

            if (!File.Exists(path))
            {
                Debug.LogWarning($"File '{path}' could not be found.");
                return;
            }

            WaveFileReader reader = new WaveFileReader(path);
            if (!reader.CanRead)
            {
                Debug.LogWarning("Cannot read audio file.");
                return;
            }

            List<float> sampleList = new List<float>();
            float[] sampleFrame;
            while ((sampleFrame = reader.ReadNextSampleFrame()) != null)
            {
                sampleList.AddRange(sampleFrame);
            }

            float[] samples = sampleList.ToArray();
            AudioClip clip = AudioClip.Create("TestAudio", samples.Length / reader.WaveFormat.Channels,
                reader.WaveFormat.Channels,
                reader.WaveFormat.SampleRate, false);
            clip.SetData(samples, 0);
            HyperCore.TotalTime = clip.length;
            _source.clip = clip;
            Debug.Log("Loaded audio data");
        }

        public void RenderFootage(float length)
        {
            StartCoroutine(RenderRoutine(length));
        }

        private IEnumerator RenderRoutine(float length)
        {
            AudioClip clip = _source.clip;
            int audioSamples = clip.frequency;
            int channels = clip.channels;
            float[] samples = new float[clip.samples * channels];
            clip.GetData(samples, 0);
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