using System;
using System.IO;
using hyper_engine;
using HyperScripts.Managers;
using SFB;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HyperScripts
{
    public class HyperCoreRuntime : MonoBehaviour
    {
        private static HyperCoreRuntime _instance;

        [SerializeField] private TimelineSlider timelineSlider;
        [SerializeField] private AudioClip startupAudio;
        [SerializeField] private float arrowSkipAmount = 5f;

        public static HyperInternalAccessor coreChange = new HyperInternalAccessor();

        public static int CurrentFrame =>
            Mathf.FloorToInt((float) AudioManager.Source.timeSamples / AudioManager.Clip.samples *
                             RenderingManager.FrameCount);

        private void Awake()
        {
            // Set up singleton
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(this);
            DontDestroyOnLoad(gameObject);

            // Set up HyperCore
            coreChange.SetTotalTime(startupAudio.length);

            // Set up AudioManager
            AudioManager.Source = gameObject.AddComponent<AudioSource>();
            AudioManager.DefaultAudio = startupAudio;
            AudioManager.PurgeFftCache();

            // Set up TimelineManager
            TimelineManager.Timeline = timelineSlider;

            // Set up MainRenderer
            HyperCore.ConnectMainCameraChanged(MainRenderer.ConnectCamera);

            gameObject.AddComponent<AudioListener>();
        }

        private void Update()
        {
            // Handle keyboard control over the timeline
            if (Input.GetButtonDown("TimelineLeft"))
                AudioManager.SkipTime(-arrowSkipAmount);
            if (Input.GetButtonDown("TimelineRight"))
                AudioManager.SkipTime(arrowSkipAmount);
            if (Input.GetButtonDown("PlayPause")) AudioManager.TogglePlay();

            AudioManager.UpdateAudioState();
            TimelineManager.UpdateTimelineState();
        }

        internal static void TimelineFrameUpdate(float value)
        {
            // Get frame amplitude
            float amplitude = AudioManager.GetMaxValueInSamplesFromSource(
                AudioManager.Source.timeSamples * AudioManager.Clip.channels,
                AudioManager.Clip.frequency / RenderingManager.Fps * AudioManager.Clip.channels);

            // Get spectrum data for frame, use cached if possible
            double[] specL, specR;
            if (!AudioManager.GetCachedFftFrame(CurrentFrame, out specL, 0))
                specL = AudioManager.GetSpectrumData(AudioManager.Source.timeSamples, 8192, 0);

            if (!AudioManager.GetCachedFftFrame(CurrentFrame, out specR, 1))
                specR = AudioManager.GetSpectrumData(AudioManager.Source.timeSamples, 8192, 1);

            // Apply data to a HyperUpdate
            UpdateHyperFrame(value, amplitude, specL, specR, 0f);
        }

        internal static void UpdateHyperFrame(float value, float amplitude,
            double[] spectrumLeft, double[] spectrumRight, float hyper)
        {
            coreChange.SetTime(HyperCore.TotalTime * value);
            HyperValues values = new HyperValues(amplitude, spectrumLeft, spectrumRight, hyper);

            coreChange.InvokeUpdate(values);

            MainRenderer.RenderFrame();
        }

        public void ImportAudioFromFile()
        {
            string path = StandaloneFileBrowser.OpenFilePanel("Open Audio File", "", new[]
            {
                new ExtensionFilter("Audio Files", "wav", "WAV", "mp3", "MP3"),
                new ExtensionFilter("All Files", "*"),
            }, false)[0];

            AudioManager.ImportAudioThreaded(path);
        }

        public void RenderFootage()
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Render To...", "", AudioManager.AudioTitle, new[]
            {
                new ExtensionFilter("Video File", "mp4"),
            });
            if (path == null || path.Equals("")) return;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Debug.LogError("Directory does not exist.");
                StatusManager.UpdateStatus("Directory does not exist.");
                return;
            }

            StartCoroutine(RenderingManager.RenderRoutine(path));
        }

        public void ImportMod()
        {
            string path = StandaloneFileBrowser.OpenFilePanel("Open Visualiser...", "",
                new[] {new ExtensionFilter("hvis")}, false)[0];

            if (!File.Exists(path))
                StatusManager.UpdateStatus("Error opening mod.");

            StartCoroutine(ModManager.LoadModAsync(path));
        }
    }
}