using System;
using UnityEngine;
using UnityEngine.UI;

namespace HyperScripts
{
    [RequireComponent(typeof(Camera))]
    public class MainRenderer : MonoBehaviour
    {
        private static MainRenderer _instance;

        private static Camera _mainCamera;
        public static int Width => _mainCamera.targetTexture.width;
        public static int Height => _mainCamera.targetTexture.height;

        [SerializeField] private RawImage _imageDisplay;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(_instance);
            DontDestroyOnLoad(_instance);

            ConnectCamera(GetComponent<Camera>());
        }

        private void Start()
        {
            RenderFrame();
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
            catch (NullReferenceException)
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
            RenderTexture newTex = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.Default,
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
            _mainCamera.targetTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            _instance._imageDisplay.texture = _mainCamera.targetTexture;
            _mainCamera.enabled = false;

            RenderFrame();
        }
    }
}