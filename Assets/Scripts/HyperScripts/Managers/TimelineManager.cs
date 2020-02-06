using UnityEngine.UI;

namespace HyperScripts.Managers
{
    public static class TimelineManager
    {
        private static TimelineSlider _timeline;

        public static TimelineSlider Timeline
        {
            get => _timeline;
            internal set
            {
                _timeline = value;
                _timeline.OnRetargetSlider.RemoveAllListeners();
                _timeline.onValueChanged.RemoveAllListeners();
                _timeline.onValueChanged.AddListener(HyperCoreRuntime.UpdateHyperFrame);
                _timeline.OnRetargetSlider.AddListener(AudioManager.UpdateAudioToTimeline);
            }
        }

        internal static void UpdateTimelineState()
        {
            SetTimelinePos((float) AudioManager.Source.timeSamples / AudioManager.Clip.samples);
        }
        
        internal static void SetTimelinePos(float value)
        {
            Timeline.value = value;
        }
    }
}