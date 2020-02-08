using HyperScripts.Managers;

namespace UI
{
    public class FftSmoothingInput : InputFieldIntLimiter
    {
        protected new void Awake()
        {
            base.Awake();
            ConfirmedEvent.AddListener(UpdateFftSmoothing);
        }

        private void UpdateFftSmoothing(int value)
        {
            AudioManager.FftSmoothing = value;
        }
    }
}