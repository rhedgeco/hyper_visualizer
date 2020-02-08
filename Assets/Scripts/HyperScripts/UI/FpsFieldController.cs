using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    public class FpsFieldController : InputFieldIntLimiter
    {
        private new void Awake()
        {
            base.Awake();
            
            ConfirmedEvent.AddListener(SetFps);
        }

        private void SetFps(int fps)
        {
            RenderingManager.Fps = fps;
        }
    }
}