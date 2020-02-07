using System;
using UnityEngine;

namespace Plugins.Free.FFT
{
    public static class Windowing
    {
        public static double[] HackyRyanWindow(float[] samples, int startIndex, int length, int channel)
        {
            double[] spec = new double[length];
            for (int i = 0; i < length; i++)
            {
                double x = samples[(startIndex * 2) + (i * 2) + channel];
                x *= (-Math.Cos(i * (2d * Math.PI) / length) + 1d) / 2d;
                spec[i] = x;
            }

            return spec;
        }
    }
}