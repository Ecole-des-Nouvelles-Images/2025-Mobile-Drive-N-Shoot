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
        public SplineContainer Spline;
        public int SplineIndex = 0;
        
        [SerializeField] private List<GameObject> _props = new();
        
        [Min(2)] public int Resolution = 50;
        [Min(0.01f)] public float Width = 2f;

        public uint Seed;
        public Transform Parent;
        public float HeightOffset;
        [Range(0f, 90f)] public float RotationOffset;
        public float PositionOffset;
        public Vector2 ScaleOffsetMinMax;

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
            
            UnityEngine.Splines.Spline.Changed += OnSplineChanged;
        }

        private void OnDisable()
        {
            UnityEngine.Splines.Spline.Changed -= OnSplineChanged;
        }
        
        private void OnSplineChanged(Spline s, int knotIndex, SplineModification modification)
        {
            if (!IsOurSpline(s)) return;
            SpawnProps();
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
            
            if (Spline == null) return;
            if (Spline.Splines.Count <= SplineIndex) return;

            _lastResolution = Resolution;
            _lastWidth = Width;
            _lastSplineIndex = SplineIndex;
            _lastHeightOffset = HeightOffset;
            _lastRotationOffset = RotationOffset;
            _lastPositionOffset = PositionOffset;
            _lastScaleOffset = ScaleOffsetMinMax;
            
            float step = 1f / (Resolution - 1);
            
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

                leftPos += new Vector3(_random.NextFloat(-PositionOffset, PositionOffset) + transform.position.x, HeightOffset, _random.NextFloat(-PositionOffset, PositionOffset) + transform.position.z);
                rightPos += new Vector3(_random.NextFloat(-PositionOffset, PositionOffset) + transform.position.x, HeightOffset, _random.NextFloat(-PositionOffset, PositionOffset) + transform.position.z);

                GameObject leftObj = Instantiate(_props[0], leftPos, Quaternion.Euler(0, _random.NextFloat(-RotationOffset, RotationOffset), 0), Parent);
                GameObject rightObj = Instantiate(_props[0], rightPos, Quaternion.Euler(0, _random.NextFloat(-RotationOffset, RotationOffset), 0), Parent);
                
                float scaleModifier = _random.NextFloat(ScaleOffsetMinMax.x, ScaleOffsetMinMax.y);
                leftObj.transform.localScale *= scaleModifier;
                rightObj.transform.localScale *= scaleModifier;
                
                _propsSpawn.Add(leftObj);
                _propsSpawn.Add(rightObj);
            }
        }
    }
}
