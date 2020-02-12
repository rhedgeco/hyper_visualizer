using System;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts.UI
{
    [RequireComponent(typeof(Slider))]
    [ExecuteInEditMode]
    public class HResizeBar : MonoBehaviour
    {
        private Slider slider;
        private RectTransform parent;

        [SerializeField] private RectTransform leftPanel;
        [SerializeField] private RectTransform rightPanel;
        [SerializeField] private int leftMinPixel = 100;
        [SerializeField] private int rightMinPixel = 100;

        private void Start()
        {
            slider = GetComponent<Slider>();
            parent = transform.parent.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!slider || !parent) Start();
            Rect r = parent.rect;
            if (r.width * slider.value < leftMinPixel) slider.value = leftMinPixel / r.width;
            if (r.width * (1 - slider.value) < rightMinPixel) slider.value = 1 - (rightMinPixel / r.width);

            if (!leftPanel || !rightPanel) return;
            leftPanel.anchorMin = new Vector2(0, 0);
            leftPanel.anchorMax = new Vector2(0, 1);
            rightPanel.anchorMin = new Vector2(1, 0);
            rightPanel.anchorMax = new Vector2(1, 1);

            leftPanel.sizeDelta = new Vector2(r.width * slider.value, leftPanel.sizeDelta.y);
            rightPanel.sizeDelta = new Vector2(r.width * (1 - slider.value), rightPanel.sizeDelta.y);
        }
    }
}