using System;
using HyperEngine;
using HyperScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultVisualizer
{
    [RequireComponent(typeof(Camera))]
    public class DefaultCamera : MonoBehaviour
    {
        private Vector3 initialPos;

        [SerializeField] private float shakeDistance = 1f;
        [SerializeField] private float shakeIntensity = 0.2f;
        
        private void Awake()
        {
            HyperCore.ConnectFrameUpdate(CameraShake);
            initialPos = transform.position;
        }

        private void Start()
        {
            MainRenderer.ConnectDefaultCamera(GetComponent<Camera>()); // This cannot be done in a mod
        }

        private void CameraShake(HyperValues values)
        {
            Vector3 randomPos = initialPos + Random.insideUnitSphere * shakeDistance;
            transform.position = Vector3.Lerp(transform.position, randomPos, values.Amplitude * shakeIntensity);
        }
    }
}
