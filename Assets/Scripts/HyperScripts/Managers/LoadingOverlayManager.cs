using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.Managers
{
    public class LoadingOverlayManager : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private Slider loadingBar;
        [SerializeField] private Text percent;

        public void StartLoading(string message)
        {
            gameObject.SetActive(true);
            UpdateLoading(message,0);
        }

        public void UpdateLoading(string message, float value)
        {
            text.text = message;
            UpdateLoading(value);
        }

        public void UpdateLoading(float value)
        {
            loadingBar.value = value;
            percent.text = $"{(int) (value * 100)}%";
        }

        public void EndLoading()
        {
            gameObject.SetActive(false);
            text.text = "";
            loadingBar.value = 0;
        }
    }
}