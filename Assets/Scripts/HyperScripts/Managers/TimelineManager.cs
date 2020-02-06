using HyperCoreScripts.Managers;
using UnityEngine.UI;

namespace HyperCoreScripts
{
    public static class TimelineManager
    {
        public static Slider Timeline { get; internal set; }

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