using System;
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
        [SerializeField] private AudioClip startupAudio; //TODO: Replace with actual audio import
        [SerializeField] private float arrowSkipAmount = 5f;

        private static HyperCoreManager _instance;
        private static AudioSource _source;
        private const float TIMELINE_SLIDER_MAX_VALUE = 0.9999f;

        public static bool Playing { get; internal set; }

        public static string AudioTitle => _source.clip.name;

        private void Awake()
        {
            // Set up singleton
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(this);
            DontDestroyOnLoad(gameObject);

            HyperCore.TotalTime = startupAudio.length;
            timelineSlider.onValueChanged.AddListener(QuickRenderFrame);
            gameObject.AddComponent<AudioListener>();
            _source = gameObject.AddComponent<AudioSource>();
            _source.loop = false;
            _source.clip = startupAudio;
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
            StartCoroutine(ImportAudioRoutine());
        }

        private IEnumerator ImportAudioRoutine()
        {
            string path = StandaloneFileBrowser.OpenFilePanel("Open Audio File", "", new[]
            {
                new ExtensionFilter("Audio Files", "wav", "WAV", "mp3", "MP3"),
                new ExtensionFilter("All Files", "*"),
            }, false)[0];

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
                reader = GetWavReaderFromMp3(path);
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
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Unsupported file extension.");
                StatusController.UpdateStatus("Unsupported file extension.");
                yield break;
            }

            if (!reader.CanRead)
            {
                Debug.LogError("Cannot read audio file.");
                StatusController.UpdateStatus("Cannot read audio file.");
                yield break;
            }
            
            StatusController.UpdateStatus($"Loading wav file : {Path.GetFileName(path)}");
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
                    StatusController.UpdateStatus($"Loaded \t{(int)((float)reader.Position/reader.Length*100)}%");
                    timeCheck = Time.realtimeSinceStartup;
                    yield return null;
                }
            }

            float[] samples = sampleList.ToArray();
            AudioClip clip = AudioClip.Create(Path.GetFileName(path), samples.Length / reader.WaveFormat.Channels,
                reader.WaveFormat.Channels,
                reader.WaveFormat.SampleRate, false);
            clip.SetData(samples, 0);
            HyperCore.TotalTime = clip.length;
            _source.clip = clip;
            StatusController.UpdateStatus("Loaded Audio Data.");
            yield return null;
        }

        // Converts an mp3 to wave on disk and returns the wavreader
        private WaveFileReader GetWavReaderFromMp3(string mp3Path)
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

        public void RenderFootage()
        {
            StartCoroutine(RenderRoutine());
        }

        private IEnumerator RenderRoutine()
        {
            AudioClip clip = _source.clip;
            float length = clip.length;
            int channels = clip.channels;
            int audioSamples = clip.frequency;
            float[] samples = new float[clip.samples * channels];
            clip.GetData(samples, 0);
            int samplesPerFrame = audioSamples / fps * channels;
            
            StatusController.UpdateStatus("Rendering HyperVideo");

            MP4Recorder recorder = new MP4Recorder(MainRenderer.Width, MainRenderer.Height, fps, audioSamples,
                channels, s => { Debug.Log(s); });
            FixedIntervalClock clock = new FixedIntervalClock(fps);

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
                Debug.Log($"Generated Frame {frame}/{(int) (length * fps)}");
                StatusController.UpdateStatus($"Generated Frame {frame}/{(int) (length * fps)}");
            }

            recorder.Dispose();
        }

        // Copies a smaller section of the sample array
        private float[] GetPartialSampleArray(float[] samples, int startIndex, int length)
        {
            float[] partial = new float[length];
            if (samples.Length - startIndex < length) length = samples.Length - startIndex;
            Array.Copy(samples, startIndex, partial, 0, length);
            return partial;
        }
    }
}