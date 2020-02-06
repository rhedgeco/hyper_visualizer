using System;
using HyperCoreScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(InputField))]
    public class FpsFieldController : MonoBehaviour
    {
        private InputField input;

        private void Awake()
        {
            input = GetComponent<InputField>();
            input.onValueChanged.AddListener(SetFps);
        }

        private void SetFps(string fpsText)
        {
            int fps = 0;
            if (!fpsText.Equals("")) fps = int.Parse(fpsText);
            HyperCoreManager.Fps = fps;
        }
    }
}