using HyperScripts;
using UnityEngine;

namespace Testing
{
    public class SpectrumViewer : MonoBehaviour
    {
        [SerializeField] private GameObject cube;
        [SerializeField] private float size = 4;

        private GameObject[] cubes = new GameObject[128];

        private void Awake()
        {
            HyperCore.ConnectFrameUpdate(SpecCubes);
            transform.localScale = new Vector3(18f / cubes.Length, 1, 1);
            transform.position = new Vector3((18f/cubes.Length)*cubes.Length/-2,
                transform.position.y,
                transform.position.z
            );
            for (int i = 0; i < cubes.Length; i++)
            {
                GameObject c = Instantiate(cube, transform);
                c.transform.localPosition = new Vector3(i, 0, 0);
                c.SetActive(true);
                cubes[i] = c;
            }
        }

        private void SpecCubes(HyperValues values)
        {
            for (int i = 0; i < cubes.Length/2; i++)
            {
                float specTime = Mathf.Abs((float) values.SpectrumLeft[i*2]);
                cubes[i].transform.localScale =
                    new Vector3(1, 1 + (size * specTime), 1);
            }
            
            for (int i = 0; i < cubes.Length/2; i++)
            {
                float specTime = Mathf.Abs((float) values.SpectrumRight[i*2]);
                cubes[cubes.Length - 1 - i].transform.localScale =
                    new Vector3(1, 1 + (size * specTime), 1);
            }
        }
    }
}