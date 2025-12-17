using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace MapGeneration
{
    public class DynamicSplineRoad : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Min(2)] private int _resolution = 50;
        [SerializeField, Min(0.01f)] private float _width = 2f;
        [SerializeField, Min(0.01f)] private float _textureTiling = 1f;
        [SerializeField] private Material _materialRoad;

        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;

        private readonly List<Mesh> _splineRoadMeshes = new();

        private Vector3[] _verts;
        private Vector2[] _uvs;
        private int[] _tris;
        private float[] _distances;

        private const string ROAD_TAG = "Road";

        [ContextMenu("DebugBuildRoad")]
        public void DebugBuildRoad()
        {
            BuildRoad(true);
        }

        public void BuildRoad(bool createBridge)
        {
            if (!_splineContainer || _splineContainer.Splines.Count == 0)
                return;

            ClearChildren();
            _splineRoadMeshes.Clear();

            _resolution = Mathf.Max(2, _resolution);
            AllocateBuffers();

            float step = 1f / (_resolution - 1);

            for (int s = 0; s < _splineContainer.Splines.Count; s++)
            {
                Mesh mesh = new Mesh { name = "RoadMesh" };

                BuildDistances(s, step);
                BuildVertices(s, step);
                BuildTriangles();

                mesh.vertices = _verts;
                mesh.uv = _uvs;
                mesh.triangles = _tris;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                CreateRoadObject(mesh);
                _splineRoadMeshes.Add(mesh);
            }

            if (createBridge)
                CreateBridge();
        }

        private void AllocateBuffers()
        {
            int vCount = _resolution * 2;
            int tCount = (_resolution - 1) * 6;

            _verts ??= new Vector3[vCount];
            _uvs ??= new Vector2[vCount];
            _tris ??= new int[tCount];
            _distances ??= new float[_resolution];
        }

        private void BuildDistances(int splineIndex, float step)
        {
            _distances[0] = 0f;

            _splineContainer.Evaluate(splineIndex, 0f, out float3 prev, out _, out _);

            for (int i = 1; i < _resolution; i++)
            {
                float t = i * step;
                _splineContainer.Evaluate(splineIndex, t, out float3 pos, out _, out _);
                _distances[i] = _distances[i - 1] + math.distance(prev, pos);
                prev = pos;
            }
        }

        private void BuildVertices(int splineIndex, float step)
        {
            float halfWidth = _width * 0.5f;

            for (int i = 0; i < _resolution; i++)
            {
                float t = i * step;
                _splineContainer.Evaluate(splineIndex, t, out float3 pos, out float3 tan, out float3 up);

                Vector3 position = pos;
                Vector3 forward = math.normalize(tan);
                Vector3 right = Vector3.Cross(up, forward).normalized;

                int vi = i * 2;

                _verts[vi] = transform.InverseTransformPoint(position - right * halfWidth);
                _verts[vi + 1] = transform.InverseTransformPoint(position + right * halfWidth);

                float uvY = _distances[i] * _textureTiling;
                _uvs[vi] = new Vector2(0, uvY);
                _uvs[vi + 1] = new Vector2(1, uvY);
            }
        }

        private void BuildTriangles()
        {
            int ti = 0;
            for (int i = 0; i < _resolution - 1; i++)
            {
                int vi = i * 2;

                _tris[ti++] = vi;
                _tris[ti++] = vi + 2;
                _tris[ti++] = vi + 1;

                _tris[ti++] = vi + 1;
                _tris[ti++] = vi + 2;
                _tris[ti++] = vi + 3;
            }
        }

        private void CreateRoadObject(Mesh mesh)
        {
            GameObject go = new GameObject("RoadMesh");
            go.transform.SetParent(transform, false);
            go.tag = ROAD_TAG;

            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = _materialRoad;
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }

        private void CreateBridge()
        {
            if (_splineRoadMeshes.Count < 2) return;

            Mesh a = _splineRoadMeshes[0];
            Mesh b = _splineRoadMeshes[1];

            Mesh bridge = CreateConnectionMesh(a, b);
            bridge.name = "BridgeMesh";

            CreateRoadObject(bridge);
        }

        private Mesh CreateConnectionMesh(Mesh a, Mesh b)
        {
            Vector3[] verts = new Vector3[4]
            {
                a.vertices[^2],
                a.vertices[^1],
                b.vertices[0],
                b.vertices[1]
            };

            int[] tris = { 0, 2, 1, 1, 2, 3 };
            Vector2[] uvs =
            {
                new(0,0), new(1,0), new(0,1), new(1,1)
            };

            Mesh m = new Mesh
            {
                vertices = verts,
                triangles = tris,
                uv = uvs
            };

            m.RecalculateNormals();
            m.RecalculateBounds();
            return m;
        }
    }
}