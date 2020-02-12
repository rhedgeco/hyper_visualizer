using UnityEngine;

namespace HyperScripts.Core
{
    // Placeholder object to connect camera to main rendering handler in HyperVisualizer
    [RequireComponent(typeof(Camera))]
    public class MainModCamera : MonoBehaviour
    {
        public Camera MainCamera => GetComponent<Camera>();
    }
}
