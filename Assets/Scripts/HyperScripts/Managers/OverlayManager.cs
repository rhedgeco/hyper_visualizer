using UnityEngine;

namespace HyperScripts.Managers
{
    public class OverlayManager : MonoBehaviour
    {
        private static LoadingOverlayManager _loadingOverlayStatic;
        [SerializeField] private LoadingOverlayManager loadingOverlay;

        public static LoadingOverlayManager Loading => _loadingOverlayStatic;

        private void Awake()
        {
            _loadingOverlayStatic = loadingOverlay;
        }
    }
}
