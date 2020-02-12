using HyperScripts;
using HyperScripts.Core;
using UnityEngine;

namespace Testing
{
    public class AmplitudeScaler : MonoBehaviour
    {
        [SerializeField] private float targetScale = 1.5f;
        [SerializeField] private float smoothTime = 0.5f;

        private Vector3 baseScale;
        private Vector3 targetScaleDelta;

        private float lastChangeAmp = 0;
        private float lastChangeTime = 0;
        
        private void Awake()
        {
            baseScale = transform.localScale;
            targetScaleDelta = baseScale * targetScale - baseScale;
            HyperCore.ConnectFrameUpdate(ScaleWithAmplitude);
        }

        private void ScaleWithAmplitude(HyperValues values)
        {
            Vector3 frameScaleTarget = baseScale + (targetScaleDelta * values.Amplitude);
            float deltaTime = HyperCore.Time - lastChangeTime;
            float smoothReset = Mathf.Clamp(deltaTime / smoothTime, 0f, 1f);

            //transform.localScale = frameScaleTarget;
        }
    }
}