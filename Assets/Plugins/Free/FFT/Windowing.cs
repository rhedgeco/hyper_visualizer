using System;

namespace Plugins.Free.FFT
{
    public static class Windowing
    {
        public static double[] HackyRyanWindow(float[] samples, int startIndex, int length, int channel, float sharpness = 1)
        {
            double[] spec = new double[length];
            for (int i = 0; i < length; i++)
            {
                int index = (startIndex * 2) + (i * 2) + channel;
                double x = 0d;
                if (index < samples.Length - 1)
                {
                    x = samples[index];
                    x *= Math.Pow((-Math.Cos(i * (2d * Math.PI) / length) + 1d) / 2d, sharpness);
                }
                spec[i] = x;
            }

            return spec;
        }
    }
}