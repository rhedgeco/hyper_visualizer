using System;
using HyperScripts.Managers;
using Plugins.Free.FFT;

namespace HyperScripts.Threading
{
    public class FftCachingWorker : GeneralThreadWorker
    {
        private float[] samples;
        private int frameCount;
        private int samplesPerFrame;
        private int smoothing;

        public FftCachingWorker(float[] samples, int frameCount, int samplesPerFrame, int smoothing)
        {
            this.samples = samples;
            this.frameCount = frameCount;
            this.samplesPerFrame = samplesPerFrame;
            this.smoothing = smoothing;
        }

        #region ThreadCallbacks

        internal override void ThreadCallbackStart()
        {
            // do nothing
        }

        protected override void ThreadCallbackUpdate()
        {
            StatusManager.UpdateStatus($"Caching Fft frames {(int) (StateProgress * 100)}%");
        }

        protected override void ThreadCallbackError()
        {
            StatusManager.UpdateStatus("Caching Thread Interrupted.");
        }

        protected override void ThreadCallbackClosed()
        {
            StatusManager.UpdateStatus($"Fft Caching Completed.");
        }

        #endregion

        protected override void ThreadBody()
        {
            AudioManager.PurgeFftCache();
            AudioManager.CanCache = false;
            StateProgress = 0;
            int frameDelta = samples.Length / frameCount / 2;
            LomontFFT fft = new LomontFFT();
            for (int i = 0; i < frameCount; i++)
            {
                if (AudioManager.CanCache)
                    throw new Exception("Cache purged before completed."); // Cancel thread if caching is reset
                double[] cacheL = Windowing.HackyRyanWindow(samples, i * frameDelta, samplesPerFrame, 0, smoothing);
                double[] cacheR = Windowing.HackyRyanWindow(samples, i * frameDelta, samplesPerFrame, 1, smoothing);
                fft.RealFFT(cacheL, true);
                fft.RealFFT(cacheR, true);
                AudioManager.AddFftCache(new[] {cacheL, cacheR});
                StateProgress = (float) i / frameCount;
            }
        }
    }
}