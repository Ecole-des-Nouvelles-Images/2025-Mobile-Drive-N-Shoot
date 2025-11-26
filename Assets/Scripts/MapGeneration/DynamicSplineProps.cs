using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;

namespace MapGeneration
{
    [RequireComponent( typeof(SplineContainer))]
    public class DynamicSplineProps : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private uint _seed;
        [SerializeField] [Min(2)] private int _density = 50;
        [SerializeField] [Min(0.01f)] private float _width = 2f;
        [SerializeField] private float _heightOffset;
        [SerializeField] [Range(0f, 90f)] private float _rotationOffset;
        [SerializeField] private float _positionOffset;
        [SerializeField] private Vector2 _scaleOffsetMinMax;
        
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
        
        // private void OnEnable()
        // {
        //     Spline.Changed += OnSplineChanged;
        // }
        //
        // private void OnDisable()
        // {
        //     Spline.Changed -= OnSplineChanged;
        // }
        //
        // private void OnSplineChanged(Spline s, int knotIndex, SplineModification modification)
        // {
        //     if (!IsOurSpline(s)) return;
        //     SpawnProps();
        // }
        //
        // private bool IsOurSpline(Spline s)
        // {
        //     if (_splineContainer == null) return false;
        //     if (_splineContainer.Splines.Count <= _splineIndex) return false;
        //     return _splineContainer.Splines[_splineIndex] == s;
        // }
        //
        // private void Update()
        // {
        //     if (_density != _lastResolution ||
        //         _width != _lastWidth ||
        //         _splineIndex != _lastSplineIndex ||
        //         _heightOffset != _lastHeightOffset ||
        //         _rotationOffset != _lastRotationOffset ||
        //         _positionOffset != _lastPositionOffset ||
        //         _scaleOffsetMinMax != _lastScaleOffset)
        //     {
        //         SpawnProps();
        //     }
        // }

        public void SpawnProps(uint seed)
        {
            _seed = seed;
            _random = new Random(_seed);
            
            foreach (var prop in _propsSpawn)
            {
                DestroyImmediate(prop);
            }
            _propsSpawn.Clear();
            
            if (_splineContainer == null) return;
            if (_splineContainer.Splines.Count <= _splineIndex) return;

            _lastResolution = _density;
            _lastWidth = _width;
            _lastSplineIndex = _splineIndex;
            _lastHeightOffset = _heightOffset;
            _lastRotationOffset = _rotationOffset;
            _lastPositionOffset = _positionOffset;
            _lastScaleOffset = _scaleOffsetMinMax;
            
            float step = 1f / (_density - 1);
            
            for (int i = 0; i < _density; ++i)
            {
                float t = i * step;
                _splineContainer.Evaluate(_splineIndex, t, out float3 pos, out float3 tan, out float3 up);

                Vector3 position = pos;
                Vector3 forward = math.normalize(tan);
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Vector3 leftPos = position - right * (_width * 0.5f);
                Vector3 rightPos = position + right * (_width * 0.5f);

                leftPos = transform.InverseTransformPoint(leftPos);
                rightPos = transform.InverseTransformPoint(rightPos);

                leftPos += new Vector3(_random.NextFloat(-_positionOffset, _positionOffset) + transform.position.x, _heightOffset, _random.NextFloat(-_positionOffset, _positionOffset) + transform.position.z);
                rightPos += new Vector3(_random.NextFloat(-_positionOffset, _positionOffset) + transform.position.x, _heightOffset, _random.NextFloat(-_positionOffset, _positionOffset) + transform.position.z);

                GameObject leftObj = Instantiate(_props[_random.NextInt(0, _props.Count - 1)], leftPos, Quaternion.Euler(0, _random.NextFloat(-_rotationOffset, _rotationOffset), 0), _transformParent);
                GameObject rightObj = Instantiate(_props[_random.NextInt(0, _props.Count - 1)], rightPos, Quaternion.Euler(0, _random.NextFloat(-_rotationOffset, _rotationOffset), 0), _transformParent);
                
                float scaleModifier = _random.NextFloat(_scaleOffsetMinMax.x, _scaleOffsetMinMax.y);
                leftObj.transform.localScale *= scaleModifier;
                rightObj.transform.localScale *= scaleModifier;
                
                _propsSpawn.Add(leftObj);
                _propsSpawn.Add(rightObj);
            }
        }
    }
}
