using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;

namespace __Workspaces.Hugoi.Scripts
{
    [ExecuteAlways]
    [RequireComponent( typeof(SplineContainer))]
    public class DynamicSplineProps : MonoBehaviour
    {
        [Header("Settings")]
        public uint Seed;
        [Min(2)] public int Resolution = 50;
        [Min(0.01f)] public float Width = 2f;
        public float HeightOffset;
        [Range(0f, 90f)] public float RotationOffset;
        public float PositionOffset;
        public Vector2 ScaleOffsetMinMax;
        
        [Header("Prefabs")]
        [SerializeField] private List<GameObject> _props = new();
        [SerializeField] private Transform _transformParent;
        
        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;

        private Random _random;
        private List<GameObject> _propsSpawn = new();
        private int _lastResolution;
        private float _lastWidth;
        private int _lastSplineIndex;
        private float _lastHeightOffset;
        private float _lastRotationOffset;
        private float _lastPositionOffset;
        private Vector2 _lastScaleOffset;
        
        private void OnEnable()
        {
            
            Spline.Changed += OnSplineChanged;
        }

        private void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }
        
        private void OnSplineChanged(Spline s, int knotIndex, SplineModification modification)
        {
            if (!IsOurSpline(s)) return;
            SpawnProps();
        }
        
        private bool IsOurSpline(Spline s)
        {
            if (_splineContainer == null) return false;
            if (_splineContainer.Splines.Count <= _splineIndex) return false;
            return _splineContainer.Splines[_splineIndex] == s;
        }
        
        private void Update()
        {
            if (Resolution != _lastResolution ||
                Width != _lastWidth ||
                _splineIndex != _lastSplineIndex ||
                HeightOffset != _lastHeightOffset ||
                RotationOffset != _lastRotationOffset ||
                PositionOffset != _lastPositionOffset ||
                ScaleOffsetMinMax != _lastScaleOffset)
            {
                SpawnProps();
            }
        }

        private void SpawnProps()
        {
            _random = new Random(Seed);
            
            foreach (var prop in _propsSpawn)
            {
                DestroyImmediate(prop);
            }
            _propsSpawn.Clear();
            
            if (_splineContainer == null) return;
            if (_splineContainer.Splines.Count <= _splineIndex) return;

            _lastResolution = Resolution;
            _lastWidth = Width;
            _lastSplineIndex = _splineIndex;
            _lastHeightOffset = HeightOffset;
            _lastRotationOffset = RotationOffset;
            _lastPositionOffset = PositionOffset;
            _lastScaleOffset = ScaleOffsetMinMax;
            
            float step = 1f / (Resolution - 1);
            
            for (int i = 0; i < Resolution; ++i)
            {
                float t = i * step;
                _splineContainer.Evaluate(_splineIndex, t, out float3 pos, out float3 tan, out float3 up);

                Vector3 position = pos;
                Vector3 forward = math.normalize(tan);
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Vector3 leftPos = position - right * (Width * 0.5f);
                Vector3 rightPos = position + right * (Width * 0.5f);

                leftPos = transform.InverseTransformPoint(leftPos);
                rightPos = transform.InverseTransformPoint(rightPos);

                leftPos += new Vector3(_random.NextFloat(-PositionOffset, PositionOffset) + transform.position.x, HeightOffset, _random.NextFloat(-PositionOffset, PositionOffset) + transform.position.z);
                rightPos += new Vector3(_random.NextFloat(-PositionOffset, PositionOffset) + transform.position.x, HeightOffset, _random.NextFloat(-PositionOffset, PositionOffset) + transform.position.z);

                GameObject leftObj = Instantiate(_props[_random.NextInt(0, _props.Count - 1)], leftPos, Quaternion.Euler(0, _random.NextFloat(-RotationOffset, RotationOffset), 0), _transformParent);
                GameObject rightObj = Instantiate(_props[_random.NextInt(0, _props.Count - 1)], rightPos, Quaternion.Euler(0, _random.NextFloat(-RotationOffset, RotationOffset), 0), _transformParent);
                
                float scaleModifier = _random.NextFloat(ScaleOffsetMinMax.x, ScaleOffsetMinMax.y);
                leftObj.transform.localScale *= scaleModifier;
                rightObj.transform.localScale *= scaleModifier;
                
                _propsSpawn.Add(leftObj);
                _propsSpawn.Add(rightObj);
            }
        }
    }
}
