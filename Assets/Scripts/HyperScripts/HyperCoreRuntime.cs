using System.IO;
using HyperScripts.Managers;
using SFB;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts
{
    public class HyperCoreRuntime : MonoBehaviour
    {
        private static HyperCoreRuntime _instance;
        
        [SerializeField] private Slider timelineSlider;
        [SerializeField] private AudioClip startupAudio;
        [SerializeField] private float arrowSkipAmount = 5f;

        private void Awake()
        {
            // Set up singleton
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(this);
            DontDestroyOnLoad(gameObject);

            // Set up HyperCore
            HyperCore.TotalTime = startupAudio.length;

            // Set up AudioManager
            AudioManager.Source = gameObject.AddComponent<AudioSource>();
            AudioManager.DefaultAudio = startupAudio;

            // Set up TimelineManager
            TimelineManager.Timeline = timelineSlider;
            TimelineManager.Timeline.onValueChanged.AddListener(QuickRenderFrame);
        }

        private void Start()
        {
            // Has to be played once for some reason, otherwise the 'time' cannot be modified
            AudioManager.Source.Play();
        }

        private void Update()
        {
            // Handle keyboard control over the timeline
            if (Input.GetButtonDown("TimelineLeft"))
                TimelineManager.Timeline.value -= arrowSkipAmount / AudioManager.Clip.length;
            if (Input.GetButtonDown("TimelineRight"))
                TimelineManager.Timeline.value += arrowSkipAmount / AudioManager.Clip.length;
            if (Input.GetButtonDown("PlayPause")) AudioManager.TogglePlay();
            
            AudioManager.UpdateAudioState();
            TimelineManager.UpdateTimelineState();
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

        internal static void UpdateHyperFrame()
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
                new ExtensionFilter("Audio Files", "wav", "WAV", "mp3", "MP3"),
                new ExtensionFilter("All Files", "*"),
            }, false)[0];
            
            StartCoroutine(AudioManager.ImportAudioRoutine(path));
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
    }
}