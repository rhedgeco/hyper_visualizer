using UnityEngine.UI;

namespace HyperCoreScripts
{
    public class TimelineManager
    {
        private static Slider timeline;
        
        public static Slider Timeline
        {
            get => timeline;
            internal set
            {
                timeline = value;
                timeline.onValueChanged.AddListener(RenderingManager.QuickRenderFrame);
            }
        }
        
        internal static void UpdateTimelineState()
        {
            SetTimelinePos((float) AudioManager.Source.timeSamples / AudioManager.Clip.samples);
        }
        
        internal static void SetTimelinePos(float value)
        {
            Timeline.value = value; // Has a listener that calls QuickRenderFrame
        }
    }
}