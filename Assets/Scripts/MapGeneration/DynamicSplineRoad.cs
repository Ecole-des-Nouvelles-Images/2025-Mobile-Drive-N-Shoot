using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace MapGeneration
{
    [RequireComponent(typeof(SplineContainer))]
    public class DynamicSplineRoad : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Min(2)] private int _resolution = 50;
        [SerializeField] [Min(0.01f)] private float _width = 2f;
        [SerializeField] [Min(0.01f)] private float _textureTiling = 1f;
        [SerializeField] private Material _materialRoad;
        
        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;
        private int _splinesCount;
        
        private Mesh _currentRoadMesh;
        private Mesh _currentBridgeMesh;
        [SerializeField] private List<Mesh> _splineRoadMeshes;

        [ContextMenu("DebugBuildRoad")]
        public void DebugBuildRoad()
        {
            BuildRoad(true);
        }
        
        public void BuildRoad(bool createBridge)
        {
            if (!_splineContainer) return;
            if (_splineContainer.Splines.Count <= 0) return;
            
            _splineRoadMeshes.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            _splinesCount = _splineContainer.Splines.Count;

            _resolution = Mathf.Max(2, _resolution);

            Vector3[] verts = new Vector3[_resolution * 2];
            Vector2[] uvs = new Vector2[_resolution * 2];
            int[] tris = new int[(_resolution - 1) * 6];

            float step = 1f / (_resolution - 1);

            // --- Calcul de la longueur cumulée ---
            float[] distances = new float[_resolution];
            distances[0] = 0f;

            for (int i = 0; i < _splinesCount; i++)
            {
                _currentRoadMesh = new Mesh();
                _currentRoadMesh.name = "RoadMesh";
                
                _splineContainer.Evaluate(i, 0f, out float3 p0, out _, out _);
                Vector3 prevPos = p0;

                for (int j = 1; j < _resolution; j++)
                {
                    float t = j * step;
                    _splineContainer.Evaluate(i, t, out float3 pos, out _, out _);
                    distances[j] = distances[j - 1] + Vector3.Distance(prevPos, pos);
                    prevPos = pos;
                }

                // --- Génération des vertices et UV ---
                for (int j = 0; j < _resolution; ++j)
                {
                    float t = j * step;
                    _splineContainer.Evaluate(i, t, out float3 pos, out float3 tan, out float3 up);

                    Vector3 position = pos;
                    Vector3 forward = math.normalize(tan);
                    Vector3 right = Vector3.Cross(up, forward).normalized;

                    Vector3 leftPos = position - right * (_width * 0.5f);
                    Vector3 rightPos = position + right * (_width * 0.5f);

                    leftPos = transform.InverseTransformPoint(leftPos);
                    rightPos = transform.InverseTransformPoint(rightPos);

                    int vi = j * 2;
                    verts[vi] = leftPos;
                    verts[vi + 1] = rightPos;

                    // --- UV FIXÉES ---
                    float uvY = distances[j] * _textureTiling;
                    uvs[vi] = new Vector2(0, uvY);
                    uvs[vi + 1] = new Vector2(1, uvY);
                }

                // --- Génération des triangles ---
                int ti = 0;
                for (int j = 0; j < _resolution - 1; ++j)
                {
                    int vi = j * 2;

                    tris[ti++] = vi;
                    tris[ti++] = vi + 2;
                    tris[ti++] = vi + 1;

                    tris[ti++] = vi + 1;
                    tris[ti++] = vi + 2;
                    tris[ti++] = vi + 3;
                }
                
                _currentRoadMesh.vertices = verts;
                _currentRoadMesh.uv = uvs;
                _currentRoadMesh.triangles = tris;
                _currentRoadMesh.RecalculateNormals();
                _currentRoadMesh.RecalculateBounds();
                
                GameObject go = new GameObject("RoadMesh");
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.tag = "Road";
                go.AddComponent<MeshRenderer>().material = _materialRoad;
                go.AddComponent<MeshFilter>().mesh = _currentRoadMesh;
                go.AddComponent<MeshCollider>().sharedMesh = _currentRoadMesh;
                
                _splineRoadMeshes.Add(_currentRoadMesh);
            }

            if (createBridge) CreateBridge();
        }
        
        private void CreateBridge()
        {
            if (_splineRoadMeshes.Count < 2) return;

            Mesh mesh0 = _splineRoadMeshes[0];
            Mesh mesh1 = _splineRoadMeshes[1];

            _currentBridgeMesh = CreateConnectionMesh(mesh0, mesh1);
            _currentBridgeMesh.name = "BridgeMesh";

            GameObject go = new GameObject("SplineBridge");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.tag = "Road";
            go.AddComponent<MeshRenderer>().material = _materialRoad;
            go.AddComponent<MeshFilter>().mesh = _currentBridgeMesh;
            go.AddComponent<MeshCollider>().sharedMesh = _currentBridgeMesh;
        }
        
        private Mesh CreateConnectionMesh(Mesh meshA, Mesh meshB)
        {
            // --- 1. Récupère la "ligne" de début du mesh B (index 1 dans ta liste) ---
            Vector3 b0 = meshB.vertices[0];
            Vector3 b1 = meshB.vertices[1];

            // --- 2. Cherche les vertices les plus proches dans mesh A (index 0) ---
            Vector3[] vertsA = meshA.vertices;

            float bestDist0 = float.MaxValue;
            float bestDist1 = float.MaxValue;

            Vector3 a0 = Vector3.zero;
            Vector3 a1 = Vector3.zero;

            foreach (var v in vertsA)
            {
                float dist0 = Vector3.Distance(v, b0);
                if (dist0 < bestDist0)
                {
                    bestDist0 = dist0;
                    a0 = v;
                }

                float dist1 = Vector3.Distance(v, b1);
                if (dist1 < bestDist1)
                {
                    bestDist1 = dist1;
                    a1 = v;
                }
            }

            // --- 3. Construction du quad entre (a0,a1) → (b0,b1) ---
            Mesh connectionMesh = new Mesh();

            Vector3[] verts = new Vector3[4];
            int[] tris = new int[6];
            Vector2[] uvs = new Vector2[4];

            // gauche meshA, droite meshA, gauche meshB, droite meshB
            verts[0] = a0;
            verts[1] = a1;
            verts[2] = b0;
            verts[3] = b1;

            // Triangles
            tris[0] = 0;
            tris[1] = 2;
            tris[2] = 1;

            tris[3] = 1;
            tris[4] = 2;
            tris[5] = 3;

            // UV simples
            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(0, 1);
            uvs[3] = new Vector2(1, 1);

            connectionMesh.vertices = verts;
            connectionMesh.triangles = tris;
            connectionMesh.uv = uvs;
            connectionMesh.RecalculateNormals();
            connectionMesh.RecalculateBounds();

            return connectionMesh;
        }
    }
}