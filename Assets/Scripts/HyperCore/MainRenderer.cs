using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace HyperCore
{
    [RequireComponent(typeof(Camera))]
    public class MainRenderer : MonoBehaviour
    {
        private Camera _mainCamera;
        
        [SerializeField] private RawImage _imageDisplay;

        public int Width => _mainCamera.targetTexture.width;
        public int Height => _mainCamera.targetTexture.height;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
            _mainCamera.targetTexture = new RenderTexture(1920,1080,24,RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            _imageDisplay.texture = _mainCamera.targetTexture;
        }

        private void Start()
        {
            RenderFrame();
        }

        public void RenderFrame()
        {
            _mainCamera.Render();
        }

        public Texture2D GetFrame(bool forceRender = false)
        {
            if(forceRender) RenderFrame();
            return RenderTextureToTexture2D(_mainCamera.targetTexture);
        }

        // Converts a RenderTexture to Texture2D
        private Texture2D RenderTextureToTexture2D(RenderTexture rendTex)
        {
            Texture2D tex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.RGBA32, false);
            RenderTexture.active = rendTex;
            tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
    }
}