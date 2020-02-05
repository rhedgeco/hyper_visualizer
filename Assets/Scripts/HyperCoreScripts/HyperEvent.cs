using UnityEngine.Events;

namespace HyperCoreScripts
{
    public class HyperEvent : UnityEvent<HyperValues>
    {
        // do nothing
    }

    public class HyperValues
    {
        public float Amplitude { get; }

        public float[] Spectrum { get; }

        public float HyperValue { get; }

        public HyperValues(float amplitude, float[] spectrum, float hyperValue)
        {
            Amplitude = amplitude;
            Spectrum = spectrum;
            HyperValue = hyperValue;
        }
    }
}