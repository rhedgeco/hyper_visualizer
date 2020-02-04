using UnityEngine;

namespace HyperCore
{
    public class MainRenderer : MonoBehaviour
    {
        private void Update()
        {
            HyperCoreManager.PushFrame(1);
        }
    }
}