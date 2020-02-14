using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultVisualizer.data
{
    public class PetalSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject petal;
        [SerializeField] private int petalCount = 30;
        [SerializeField] private float petalRadius = 20;
        [SerializeField] private float petalDepth = 30f;
        [SerializeField] private float petalClose = 20f;
        [SerializeField] private float petalSubtractRadius = 5;
        [SerializeField] private float petalMaxScale = 0.3f;
        [SerializeField] private float petalMinScale = 0.6f;

        private void Awake()
        {
            SpawnInitialPetals();
        }

        private void SpawnInitialPetals()
        {
            for (int i = 0; i < petalCount; i++)
            {
                GameObject g = Instantiate(petal, transform);

                // random radius from center
                g.transform.position = new Vector3(
                    Random.Range(-petalRadius, petalRadius),
                    Random.Range(-petalRadius, petalRadius),
                    Random.Range(-petalClose, petalDepth)
                );

                // expand to not cover center piece
                if(Mathf.Abs(g.transform.position.x) < petalSubtractRadius && 
                   Mathf.Abs(g.transform.position.y) < petalSubtractRadius)
                    Destroy(g);

                g.transform.rotation = Quaternion.Euler(
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f)
                );
                g.transform.localScale = Vector3.one * Random.Range(petalMinScale, petalMaxScale);
            }
        }
    }
}