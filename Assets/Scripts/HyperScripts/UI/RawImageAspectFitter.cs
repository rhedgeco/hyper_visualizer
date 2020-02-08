using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RawImage))]
    [ExecuteInEditMode]
    public class RawImageAspectFitter : MonoBehaviour
    {
        private RectTransform _parentRect;
        private RectTransform _selfRect;
        private RawImage _rawImage;
        private float _ratio;

        private void Awake()
        {
            _parentRect = transform.parent.GetComponent<RectTransform>();
            _selfRect = GetComponent<RectTransform>();
            _rawImage = GetComponent<RawImage>();
        }

        private void Update()
        {
            Texture mainTexture = _rawImage.mainTexture;
            _ratio = (float) mainTexture.height / mainTexture.width;

            if (!_parentRect) return;
            
            Rect rect = _parentRect.rect;
            float spaceWidth = rect.width;
            float spaceHeight = rect.height;
            float spaceRatio = spaceHeight / spaceWidth;

            _selfRect.anchoredPosition = new Vector2(0, 0);
            _selfRect.pivot = new Vector2(0.5f, 0.5f);
            _selfRect.anchorMax = new Vector2(0.5f, 0.5f);
            _selfRect.anchorMin = new Vector2(0.5f, 0.5f);

            float targetWidth, targetHeight;
            if (spaceRatio < _ratio)
            {
                targetHeight = _parentRect.rect.size.y;
                targetWidth = targetHeight / _ratio;
            }
            else
            {
                targetWidth = _parentRect.rect.size.x;
                targetHeight = targetWidth * _ratio;
            }
                
            _selfRect.sizeDelta = new Vector2(targetWidth, targetHeight);
        }
    }
}