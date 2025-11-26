using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace MapGeneration
{
    [ExecuteAlways]
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
        
        private Mesh _currentMesh;
        // private List<MeshCollider> _meshCollider;
        
        private int _lastResolution;
        private float _lastWidth;
        private int _lastSplineIndex;
        private float _lastTextureTiling;
        
        private void Awake()
        {
            // _meshCollider = GetComponent<MeshCollider>();
            
            // EnsureMesh();
        }
        
        // private void OnEnable()
        // {
        //     EnsureMesh();
        //     Spline.Changed += OnSplineChanged;
        //     Rebuild(true);
        // }
        //
        // private void OnDisable()
        // {
        //     Spline.Changed -= OnSplineChanged;
        // }
        
        // private void EnsureMesh()
        // {
        //     if (_currentMesh == null)
        //     {
        //         _currentMesh = new Mesh();
        //         _currentMesh.name = "RoadMesh";
        //         GetComponent<MeshFilter>().sharedMesh = _currentMesh;
        //     }
        // }
        
        // private void OnSplineChanged(Spline s, int knotIndex, SplineModification modification)
        // {
        //     if (!IsOurSpline(s)) return;
        //     Rebuild();
        // }
        //
        // private bool IsOurSpline(Spline s)
        // {
        //     if (_splineContainer == null) return false;
        //     if (_splineContainer.Splines.Count <= _splineIndex) return false;
        //     return _splineContainer.Splines[_splineIndex] == s;
        // }
        
        // private void Update()
        // {
        //     if (_resolution != _lastResolution ||
        //         _width != _lastWidth ||
        //         _textureTiling != _lastTextureTiling)
        //     {
        //         Rebuild();
        //     }
        // }
        
        [ContextMenu("Rebuild")]
        public void Rebuild()
        {
            if (!_splineContainer) return;
            if (_splineContainer.Splines.Count <= 0) return;
            
            _lastResolution = _resolution;
            _lastWidth = _width;
            
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
                _currentMesh = new Mesh();
                _currentMesh.name = "RoadMesh";
                
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
                
                _currentMesh.vertices = verts;
                _currentMesh.uv = uvs;
                _currentMesh.triangles = tris;
                _currentMesh.RecalculateNormals();
                _currentMesh.RecalculateBounds();
                
                GameObject go = new GameObject("RoadMesh");
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.AddComponent<MeshRenderer>().material = _materialRoad;
                go.AddComponent<MeshFilter>().mesh = _currentMesh;
                go.AddComponent<MeshCollider>().sharedMesh = _currentMesh;
            }
        }
    }
}



// if (!_splineContainer) return;
//             if (_splineContainer.Splines.Count <= _splineIndex) return;
//             
//             _lastResolution = _resolution;
//             _lastWidth = _width;
//             _lastSplineIndex = _splineIndex;
//
//             _resolution = Mathf.Max(2, _resolution);
//
//             Vector3[] verts = new Vector3[_resolution * 2];
//             Vector2[] uvs = new Vector2[_resolution * 2];
//             int[] tris = new int[(_resolution - 1) * 6];
//
//             float step = 1f / (_resolution - 1);
//
//             // --- Calcul de la longueur cumulée ---
//             float[] distances = new float[_resolution];
//             distances[0] = 0f;
//
//             _splineContainer.Evaluate(_splineIndex, 0f, out float3 p0, out _, out _);
//             Vector3 prevPos = p0;
//
//             for (int i = 1; i < _resolution; i++)
//             {
//                 float t = i * step;
//                 _splineContainer.Evaluate(_splineIndex, t, out float3 pos, out _, out _);
//                 distances[i] = distances[i - 1] + Vector3.Distance(prevPos, pos);
//                 prevPos = pos;
//             }
//
//             // --- Génération des vertices et UV ---
//             for (int i = 0; i < _resolution; ++i)
//             {
//                 float t = i * step;
//                 _splineContainer.Evaluate(_splineIndex, t, out float3 pos, out float3 tan, out float3 up);
//
//                 Vector3 position = pos;
//                 Vector3 forward = math.normalize(tan);
//                 Vector3 right = Vector3.Cross(up, forward).normalized;
//
//                 Vector3 leftPos = position - right * (_width * 0.5f);
//                 Vector3 rightPos = position + right * (_width * 0.5f);
//
//                 leftPos = transform.InverseTransformPoint(leftPos);
//                 rightPos = transform.InverseTransformPoint(rightPos);
//
//                 int vi = i * 2;
//                 verts[vi] = leftPos;
//                 verts[vi + 1] = rightPos;
//
//                 // --- UV FIXÉES ---
//                 float uvY = distances[i] * _textureTiling;
//                 uvs[vi] = new Vector2(0, uvY);
//                 uvs[vi + 1] = new Vector2(1, uvY);
//             }
//
//             // --- Génération des triangles ---
//             int ti = 0;
//             for (int i = 0; i < _resolution - 1; ++i)
//             {
//                 int vi = i * 2;
//
//                 tris[ti++] = vi;
//                 tris[ti++] = vi + 2;
//                 tris[ti++] = vi + 1;
//
//                 tris[ti++] = vi + 1;
//                 tris[ti++] = vi + 2;
//                 tris[ti++] = vi + 3;
//             }
//
//             _mesh.Clear();
//             _mesh.vertices = verts;
//             _mesh.uv = uvs;
//             _mesh.triangles = tris;
//             _mesh.RecalculateNormals();
//             _mesh.RecalculateBounds();
//             
//             _meshCollider.sharedMesh = _mesh;