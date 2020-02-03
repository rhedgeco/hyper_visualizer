using UnityEngine;

namespace HyperCore
{
    public class HyperCore
    {
        public HyperEvent BeginFrame { get; } = new HyperEvent();

        public HyperEvent UpdateFrame { get; } = new HyperEvent();

        public HyperEvent EndFrame { get; } = new HyperEvent();

        public HyperCore()
        {
            UpdateFrame.AddListener(DebugHyperEvents);
        }

        private void DebugHyperEvents(HyperValues values)
        {
            Debug.Log($"Frame Time : {values.Time}" +
                      $"Frame Amplitude : {values.Amplitude}\n" +
                      $"Frame SpectrumCount : {values.Spectrum.Length}\n" +
                      $"Frame Hyper Value : {values.HyperValue}");
        }
    }
}