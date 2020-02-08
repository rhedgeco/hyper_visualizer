using System;
using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Slider))]
    public class CacheBarController : MonoBehaviour
    {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void Update()
        {
            slider.value = (float) AudioManager.CachedFftFrames / RenderingManager.FrameCount;
        }
    }
}