using System;
using HyperScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Button))]
    public class ApplyResolutionButton : MonoBehaviour
    {
        private Button btn;

        [SerializeField] private InputFieldIntLimiter widthField;
        [SerializeField] private InputFieldIntLimiter heightField;

        private void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(ApplyResolution);
        }

        private void ApplyResolution()
        {
            Vector2Int size = new Vector2Int(widthField.GetValue(),heightField.GetValue());
            MainRenderer.ResizeFrame(size);
            StatusManager.UpdateStatus($"Updated resolution to {size.x}x{size.y}");
        }
    }
}
