using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Text))]
    public class AudioTitleController : MonoBehaviour
    {
        private Text text;
    
        private void Awake()
        {
            text = GetComponent<Text>();
        }

        private void Update()
        {
            text.text = AudioManager.AudioTitle;
        }
    }
}
