using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace __Workspaces.Hugoi.Scripts
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SplineContainer))]
    public class DynamicSplineRoad : MonoBehaviour
    {
        public SplineContainer Spline;
        public int SplineIndex = 0;

        [Min(2)] public int Resolution = 50;

        [Min(0.01f)] public float Width = 2f;

        [Min(0.01f)] public float TextureTiling = 1f;

        private Mesh _mesh;

        private int _lastResolution;
        private float _lastWidth;
        private int _lastSplineIndex;
        private float _lastTextureTiling;

        private void Awake()
        {
            EnsureMesh();
        }

        private void OnEnable()
        {
            EnsureMesh();
            UnityEngine.Splines.Spline.Changed += OnSplineChanged;
            Rebuild(true);
        }

        private void OnDisable()
        {
            UnityEngine.Splines.Spline.Changed -= OnSplineChanged;
        }

        private void EnsureMesh()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "RoadMesh";
                GetComponent<MeshFilter>().sharedMesh = _mesh;
            }
        }

        private void OnSplineChanged(Spline s, int knotIndex, SplineModification modification)
        {
            if (!IsOurSpline(s)) return;
            Rebuild();
        }

        private bool IsOurSpline(Spline s)
        {
            if (Spline == null) return false;
            if (Spline.Splines.Count <= SplineIndex) return false;
            return Spline.Splines[SplineIndex] == s;
        }

        private void Update()
        {
            if (Resolution != _lastResolution ||
                Width != _lastWidth ||
                SplineIndex != _lastSplineIndex ||
                TextureTiling != _lastTextureTiling)
            {
                Rebuild();
            }
        }

        private void Rebuild(bool force = false)
        {
            if (Spline == null) return;
            if (Spline.Splines.Count <= SplineIndex) return;

            _lastResolution = Resolution;
            _lastWidth = Width;
            _lastSplineIndex = SplineIndex;

            Resolution = Mathf.Max(2, Resolution);

            Vector3[] verts = new Vector3[Resolution * 2];
            Vector2[] uvs = new Vector2[Resolution * 2];
            int[] tris = new int[(Resolution - 1) * 6];

            float step = 1f / (Resolution - 1);

            // --- Calcul de la longueur cumulée ---
            float[] distances = new float[Resolution];
            distances[0] = 0f;

            Spline.Evaluate(SplineIndex, 0f, out float3 p0, out _, out _);
            Vector3 prevPos = p0;

            for (int i = 1; i < Resolution; i++)
            {
                float t = i * step;
                Spline.Evaluate(SplineIndex, t, out float3 pos, out _, out _);
                distances[i] = distances[i - 1] + Vector3.Distance(prevPos, pos);
                prevPos = pos;
            }

            // --- Génération des vertices et UV ---
            for (int i = 0; i < Resolution; ++i)
            {
                float t = i * step;
                Spline.Evaluate(SplineIndex, t, out float3 pos, out float3 tan, out float3 up);

                Vector3 position = pos;
                Vector3 forward = math.normalize(tan);
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Vector3 leftPos = position - right * (Width * 0.5f);
                Vector3 rightPos = position + right * (Width * 0.5f);

                leftPos = transform.InverseTransformPoint(leftPos);
                rightPos = transform.InverseTransformPoint(rightPos);

                int vi = i * 2;
                verts[vi] = leftPos;
                verts[vi + 1] = rightPos;

                // --- UV FIXÉES ---
                float uvY = distances[i] * TextureTiling;
                uvs[vi] = new Vector2(0, uvY);
                uvs[vi + 1] = new Vector2(1, uvY);
            }

            // --- Génération des triangles ---
            int ti = 0;
            for (int i = 0; i < Resolution - 1; ++i)
            {
                int vi = i * 2;

                tris[ti++] = vi;
                tris[ti++] = vi + 2;
                tris[ti++] = vi + 1;

                tris[ti++] = vi + 1;
                tris[ti++] = vi + 2;
                tris[ti++] = vi + 3;
            }

            _mesh.Clear();
            _mesh.vertices = verts;
            _mesh.uv = uvs;
            _mesh.triangles = tris;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}
