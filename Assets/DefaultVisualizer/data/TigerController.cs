using hyper_engine;
using UnityEngine;

namespace DefaultVisualizer.data
{
    public class TigerController : MonoBehaviour
    {
        private float lastTime;
        private Vector3 originalScale;

        [SerializeField] private float shrinkTime = 0.2f;
        [SerializeField] private float scaleAmount = 0.1f;

        private void Awake()
        {
            HyperCore.ConnectFrameUpdate(FrameUpdate);
            originalScale = transform.localScale;
        }

        private void FrameUpdate(HyperValues values)
        {
            float deltaTime = HyperCore.Time - lastTime;
            lastTime = HyperCore.Time;

            Vector3 left = transform.localScale - originalScale;
            Vector3 max = (originalScale * (1 + scaleAmount)) - originalScale;
            float progress = left.y / max.y;
            float newProgress = progress - (deltaTime / shrinkTime);
            
            transform.localScale = Vector3.Lerp(originalScale, transform.localScale, newProgress);
            
            Vector3 targetScale = originalScale * (1 + (scaleAmount * values.Amplitude));
            if (targetScale.x > transform.localScale.x) transform.localScale = targetScale;
        }
    }
}