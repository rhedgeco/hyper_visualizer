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

            _mainCamera = GetComponent<Camera>();
            _mainCamera.targetTexture = new RenderTexture(1920/2, 1080/2, 24, RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            _imageDisplay.texture = _mainCamera.targetTexture;
            _mainCamera.enabled = false;
        }

        private void Start()
        {
            RenderFrame();
        }

        internal static void RenderFrame()
        {
            _mainCamera.Render();
        }

        internal static Texture2D GetFrame(bool forceRender = false)
        {
            if (forceRender) RenderFrame();
            return RenderTextureToTexture2D(_mainCamera.targetTexture);
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
    }
}