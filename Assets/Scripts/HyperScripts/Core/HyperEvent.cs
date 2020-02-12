using UnityEngine.Events;

namespace HyperScripts.Core
{
    public class HyperEvent : UnityEvent<HyperValues>
    {
        // do nothing
    }

    public class HyperValues
    {
        public float Amplitude { get; }

        public double[] SpectrumLeft { get; }
        
        public double[] SpectrumRight { get; }

        public float HyperValue { get; }

        public HyperValues(float amplitude, double[] spectrumLeft, double[] spectrumRight, float hyperValue)
        {
            Amplitude = amplitude;
            SpectrumLeft = spectrumLeft;
            SpectrumRight = spectrumRight;
            HyperValue = hyperValue;
        }
    }
}