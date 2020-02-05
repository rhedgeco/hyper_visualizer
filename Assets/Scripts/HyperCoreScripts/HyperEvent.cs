using UnityEngine.Events;

namespace HyperCoreScripts
{
    public class HyperEvent : UnityEvent<HyperValues>
    {
        // do nothing
    }

    public class HyperValues
    {
        public float Time { get; }
        
        public float Amplitude { get; }

        public float[] Spectrum { get; }

        public float HyperValue { get; }

        public HyperValues(float time, float amplitude, float[] spectrum, float hyperValue)
        {
            Time = time;
            Amplitude = amplitude;
            Spectrum = spectrum;
            HyperValue = hyperValue;
        }
    }
}