using HyperCoreScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI
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
            button.onClick.AddListener(HyperCoreManager.TogglePlay);
        }

        private void Update()
        {
            if (HyperCoreManager.Playing)
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