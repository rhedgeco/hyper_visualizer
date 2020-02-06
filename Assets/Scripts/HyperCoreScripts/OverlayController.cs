using System;
using UnityEngine;

namespace UI
{
    public class OverlayController : MonoBehaviour
    {
        private static LoadingOverlayController _loadingOverlayStatic;
        [SerializeField] private LoadingOverlayController loadingOverlay;

        public static LoadingOverlayController Loading => _loadingOverlayStatic;

        private void Awake()
        {
            _loadingOverlayStatic = loadingOverlay;
        }
    }
}
