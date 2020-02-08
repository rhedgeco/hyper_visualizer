using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Button))]
    public class PlayButtonController : MonoBehaviour
    {
        private Button button;

        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite playIcon;
        [SerializeField] private Color playColor;
        [SerializeField] private Sprite pauseIcon;
        [SerializeField] private Color pauseColor;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(AudioManager.TogglePlay);
        }

        private void Update()
        {
            if (AudioManager.Playing)
            {
                iconImage.sprite = pauseIcon;
                iconImage.color = pauseColor;
            }
            else
            {
                iconImage.sprite = playIcon;
                iconImage.color = playColor;
            }
        }
    }
}