using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Button))]
    public class HidePanelButton : MonoBehaviour
    {
        private Button btn;

        [SerializeField] private RectTransform hidePanel;

        private void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(TogglePanel);
        }

        private void TogglePanel()
        {
            hidePanel.gameObject.SetActive(!hidePanel.gameObject.activeSelf);
        }
    }
}