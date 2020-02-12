using System;
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
            Debug.Log(widthField.GetValue() + ", " + heightField.GetValue());
        }
    }
}
