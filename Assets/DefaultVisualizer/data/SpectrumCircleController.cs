using System;
using HyperEngine;
using UnityEngine;

namespace DefaultVisualizer.data
{
    public class SpectrumCircleController : MonoBehaviour
    {
        private MeshFilter filter1;
        private MeshRenderer renderer1;
        private MeshFilter filter2;
        private MeshRenderer renderer2;

        private Vector3[] meshV;
        private Vector3[] meshVOriginal;

        [SerializeField] private Material material1;
        [SerializeField] private Material material2;
        [SerializeField] private float circleRadius = 2;
        [SerializeField] private float deltaRadius = 2;
        [SerializeField] private int circleSides = 3;
        [SerializeField] private float fadeSpeed = 0.1f;
        [SerializeField] private float intensity = 2;

        private void Awake()
        {
            filter1 = gameObject.AddComponent<MeshFilter>();
            renderer1 = gameObject.AddComponent<MeshRenderer>();
            GameObject g = new GameObject("black_outline", typeof(MeshFilter), typeof(MeshRenderer));
            filter2 = g.GetComponent<MeshFilter>();
            renderer2 = g.GetComponent<MeshRenderer>();
            g.transform.parent = transform;
            g.transform.localScale = Vector3.one * 1.1f;

            renderer1.material = material1;
            renderer2.material = material2;

            GenerateMeshCircle(circleSides, circleRadius);

            HyperCore.ConnectFrameUpdate(ApplySpectrumToMesh);
        }

        private void ApplySpectrumToMesh(HyperValues values)
        {
            if (meshV.Length > values.SpectrumLeft.Length) return;

            for (int i = 3; i <= meshV.Length / 2; i += 2)
            {
                int p = i;
                double spec = Math.Atan(Math.Abs(values.SpectrumLeft[i - 1])) * intensity;
                meshV[p] = Vector3.Lerp(meshV[p], meshVOriginal[p], fadeSpeed);
                float target = (float) (deltaRadius * spec + circleRadius);
                if ((meshVOriginal[p] * target).magnitude > meshV[p].magnitude) meshV[p] = meshVOriginal[p] * target;
                meshV[p - 1] = meshV[p];
            }

            for (int i = meshV.Length - 2; i > meshV.Length / 2 + 1; i -= 2)
            {
                int p = i;
                double spec = Math.Atan(Math.Abs(values.SpectrumRight[meshV.Length - i])) * intensity;
                meshV[p] = Vector3.Lerp(meshV[p], meshVOriginal[p], fadeSpeed);
                float target = (float) (deltaRadius * spec + circleRadius);
                if ((meshVOriginal[p] * target).magnitude > meshV[p].magnitude) meshV[p] = meshVOriginal[p] * target;
                meshV[p - 1] = meshV[p];
            }

            int p1 = 1;
            double spec1 = Math.Atan(Math.Abs(values.SpectrumLeft[0])) * intensity;
            meshV[p1] = Vector3.Lerp(meshV[p1], meshVOriginal[p1], fadeSpeed);
            float target1 = (float) (deltaRadius * Math.Abs(values.SpectrumLeft[0]) + circleRadius);
            if ((meshVOriginal[p1] * target1).magnitude > meshV[p1].magnitude) meshV[p1] = meshVOriginal[p1] * target1;

            p1 = meshV.Length - 1;
            spec1 = Math.Atan(Math.Abs(values.SpectrumLeft[0])) * intensity;
            meshV[p1] = Vector3.Lerp(meshV[p1], meshVOriginal[p1], fadeSpeed);
            target1 = (float) (deltaRadius * Math.Abs(values.SpectrumRight[0]) + circleRadius);
            if ((meshVOriginal[p1] * target1).magnitude > meshV[p1].magnitude) meshV[p1] = meshVOriginal[p1] * target1;

            p1 = meshV.Length / 2;
            spec1 = Math.Atan(Math.Abs(values.SpectrumLeft[meshV.Length / 2])) * intensity;
            meshV[p1] = Vector3.Lerp(meshV[p1], meshVOriginal[p1], fadeSpeed);
            target1 = (float) (deltaRadius * Math.Abs(values.SpectrumLeft[meshV.Length / 2]) + circleRadius);
            if ((meshVOriginal[p1] * target1).magnitude > meshV[p1].magnitude) meshV[p1] = meshVOriginal[p1] * target1;

            p1 = meshV.Length / 2 + 1;
            spec1 = Math.Atan(Math.Abs(values.SpectrumLeft[meshV.Length / 2])) * intensity;
            meshV[p1] = Vector3.Lerp(meshV[p1], meshVOriginal[p1], fadeSpeed);
            target1 = (float) (deltaRadius * Math.Abs(values.SpectrumRight[meshV.Length / 2]) + circleRadius);
            if ((meshVOriginal[p1] * target1).magnitude > meshV[p1].magnitude) meshV[p1] = meshVOriginal[p1] * target1;

            meshV[1] =
                meshV[meshV.Length - 1] = (meshV[1] + meshV[meshV.Length - 1]) / 2;
            meshV[meshV.Length / 2] =
                meshV[meshV.Length / 2 + 1] = (meshV[meshV.Length / 2] + meshV[meshV.Length / 2 + 1]) / 2;

            meshV[0] = Vector3.zero;
            filter1.mesh.vertices = meshV;
            filter2.mesh.vertices = meshV;
        }

        private void GenerateMeshCircle(int sides, float rad)
        {
            if (sides < 3) return;
            Mesh mesh = new Mesh();
            float angle = (2 * Mathf.PI) / sides;
            float offset = Mathf.PI / 2;

            Vector3[] vertices = new Vector3[sides * 2 + 1];
            Vector2[] uv = new Vector2[sides * 2 + 1];
            int[] triangles = new int[sides * 3];

            vertices[0] = new Vector3(0, 0);
            uv[0] = new Vector2(0.5f, 0);
            for (int i = 0; i < sides; i++)
            {
                vertices[(i * 2) + 1] =
                    new Vector3(Mathf.Cos((i + 0) * angle + offset) * rad, Mathf.Sin((i + 0) * angle + offset) * rad);
                vertices[(i * 2) + 2] =
                    new Vector3(Mathf.Cos((i + 1) * angle + offset) * rad, Mathf.Sin((i + 1) * angle + offset) * rad);

                uv[i * 2 + 1] = new Vector2(0, 1);
                uv[i * 2 + 2] = new Vector2(1, 1);

                triangles[(i * 3) + 0] = (i * 2) + 2;
                triangles[(i * 3) + 1] = (i * 2) + 1;
                triangles[(i * 3) + 2] = 0;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            filter1.mesh = mesh;
            filter2.mesh = mesh;

            meshV = vertices;
            meshVOriginal = new Vector3[vertices.Length];
            Array.Copy(vertices, meshVOriginal, vertices.Length);
        }
    }
}