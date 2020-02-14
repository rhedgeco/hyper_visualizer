using HyperEngine;
using UnityEngine;

namespace DefaultVisualizer.data
{
    public class PetalController : MonoBehaviour
    {
        private Material material;
        private Vector3 randomRotAxis;
        private float _lastTime = 0;

        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private float addSpeed = 10f;
        [SerializeField] private float fadeDepth = 30f;
        [SerializeField] private float fadeFalloff = 10f;
        [SerializeField] private float maxClose = 20f;
        [SerializeField] private float maxDepth = 60f;

        private void Awake()
        {
            HyperCore.ConnectFrameUpdate(MovePetal);
            material = GetComponent<MeshRenderer>().material;
            randomRotAxis = new Vector3(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
        }

        private void MovePetal(HyperValues values)
        {
            float deltaTime = HyperCore.Time - _lastTime;
            transform.position += new Vector3(0, 0, (-baseSpeed + (-addSpeed * values.Amplitude)) * deltaTime);
            transform.rotation *= Quaternion.Euler(randomRotAxis * deltaTime);
            _lastTime = HyperCore.Time;

            while (transform.position.z < -maxClose)
                transform.position = new Vector3(transform.position.x, transform.position.y,
                    transform.position.z + maxDepth + maxClose);

            while (transform.position.z > maxDepth)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y,
                                    transform.position.z - maxDepth - maxClose);
            }
        }
    }
}