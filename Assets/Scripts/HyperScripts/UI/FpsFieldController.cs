using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(InputField))]
    public class FpsFieldController : MonoBehaviour
    {
        private InputField input;

        private void Awake()
        {
            input = GetComponent<InputField>();
            input.onValueChanged.AddListener(SetFpsChanged);
            input.onEndEdit.AddListener(SetFpsEndChange);
        }

        private void SetFpsChanged(string fpsText)
        {
            if (fpsText.Equals(""))
            {
                RenderingManager.Fps = 1;
                return;
            }

            SetFpsEndChange(fpsText);
        }

        private void SetFpsEndChange(string fpsText)
        {
            int fps = 1;
            if (!fpsText.Equals("")) fps = int.Parse(fpsText);
            if (fps > 99) fps = 99;
            if (fps < 1) fps = 1;

            input.text = "" + fps;
            RenderingManager.Fps = fps;
        }
    }
}