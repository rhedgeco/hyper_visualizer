using System;
using UnityEngine;

namespace HyperScripts.UI
{
    public class CursorController : MonoBehaviour
    {
        private static CursorController _instance;

        public enum CursorType
        {
            LeftSlide
        }

        [SerializeField] private Texture2D leftSlideCursor;
        [SerializeField] private Vector2 leftSlideCursorHotspot = Vector2.zero;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        public static void SetCursor(CursorType type)
        {
            switch (type)
            {
                case CursorType.LeftSlide:
                    Cursor.SetCursor(_instance.leftSlideCursor, _instance.leftSlideCursorHotspot, CursorMode.Auto);
                    break;
            }
        }

        public static void ResetCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}