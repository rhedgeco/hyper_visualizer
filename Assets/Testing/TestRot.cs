using HyperCoreScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Testing
{
    public class TestRot : MonoBehaviour
    {
        [SerializeField] private Vector3 spin;

        private void Awake()
        {
            HyperCore.ConnectFrameUpdate(RotateObject);
        }

        private void RotateObject(HyperValues values)
        {
            transform.rotation = Quaternion.identity;
            transform.Rotate(spin * HyperCore.Time);
        }
    }
}