using System;
using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Button))]
    public class PreCacheButtonController : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(AudioManager.CacheFftThreaded);
        }

        private void Update()
        {
            button.interactable = AudioManager.CanCache;
        }
    }
}