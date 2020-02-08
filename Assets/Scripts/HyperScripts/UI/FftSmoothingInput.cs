using HyperScripts.Managers;

namespace HyperScripts.UI
{
    public class FftSmoothingInput : InputFieldIntLimiter
    {
        protected new void Awake()
        {
            base.Awake();
            ConfirmedEvent.AddListener(UpdateFftSmoothing);
        }

        private static void UpdateFftSmoothing(int value)
        {
            AudioManager.FftSmoothing = value;
        }
    }
}