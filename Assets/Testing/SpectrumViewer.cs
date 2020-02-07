using HyperScripts;
using UnityEngine;

namespace Testing
{
    public class SpectrumViewer : MonoBehaviour
    {
        [SerializeField] private GameObject cube;
        [SerializeField] private float size = 4;

        private GameObject[] cubes = new GameObject[512];

        private void Awake()
        {
            HyperCore.ConnectFrameUpdate(SpecCubes);
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
            for (int i = 0; i < cubes.Length / 2; i++)
            {
                float specTime = Mathf.Abs((float) values.SpectrumLeft[i]);
                cubes[i].transform.localScale =
                    new Vector3(1, 1 + (size * specTime), 1);
            }

            for (int i = cubes.Length - 1; i >= cubes.Length / 2; i--)
            {
                float specTime = Mathf.Abs((float) values.SpectrumRight[(cubes.Length - 1) - i]);
                cubes[i].transform.localScale =
                    new Vector3(1, 1 + (size * specTime), 1);
            }
        }
    }
}