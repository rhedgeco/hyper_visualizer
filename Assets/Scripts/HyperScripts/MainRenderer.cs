using System;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts
{
    public class MainRenderer : MonoBehaviour
    {
        private static MainRenderer _instance;

        private static Camera _mainCamera;
        private static int _width = 2000;
        private static int _height = 2000;
        public static int Width => _width;
        public static int Height => _height;

        [SerializeField] private RawImage _imageDisplay;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(_instance);
            DontDestroyOnLoad(_instance);
        }

        internal static void RenderFrame()
        {
            RenderFrame(_mainCamera);
        }

        internal static void RenderFrame(Camera cam)
        {
            try
            {
                cam.Render();
            }
            catch (Exception)
            {
                // do nothing
                // Sometimes post processing errors out on initial load. That is okay.
            }
        }

        internal static Texture2D GetFrame(bool forceRender = false)
        {
            if (forceRender) RenderFrame();
            return RenderTextureToTexture2D(_mainCamera.targetTexture);
        }

        internal static void ResizeFrame(Vector2Int size)
        {
            _width = size.x;
            _height = size.y;
            RenderTexture newTex = new RenderTexture(Width, Height, 24, RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            _mainCamera.targetTexture = newTex;
            _instance._imageDisplay.texture = _mainCamera.targetTexture;
            _mainCamera.Render();
        }

        // Converts a RenderTexture to Texture2D
        private static Texture2D RenderTextureToTexture2D(RenderTexture rendTex)
        {
            Texture2D tex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.RGBA32, false);
            RenderTexture.active = rendTex;
            tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
            tex.Apply();
            return tex;
        }

        internal static void ConnectCamera(Camera camera)
        {
            _mainCamera = camera;
            _mainCamera.targetTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            _instance._imageDisplay.texture = _mainCamera.targetTexture;
            _mainCamera.enabled = false;

            RenderFrame();
        }

        internal static void ConnectDefaultCamera(Camera camera)
        {
            _mainCamera = camera;
            _mainCamera.targetTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            _instance._imageDisplay.texture = _mainCamera.targetTexture;
            _mainCamera.enabled = false;

            RenderFrame();
        }
    }
}