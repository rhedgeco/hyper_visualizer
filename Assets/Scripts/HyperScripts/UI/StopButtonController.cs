using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Button))]
    public class StopButtonController : MonoBehaviour
    {
        private Button btn;

        private void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(AudioManager.StopPlaying);
        }
    }
}