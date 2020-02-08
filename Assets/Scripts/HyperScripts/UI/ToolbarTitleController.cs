using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ToolbarTitleController : MonoBehaviour
    {
        private Button btn;

        [SerializeField] private string url;

        private void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(delegate
            {
                Application.OpenURL(url);
            });
        }
    }
}
