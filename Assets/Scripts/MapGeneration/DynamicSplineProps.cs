using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;

namespace MapGeneration
{
    public class DynamicSplineProps : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private uint _seed;
        [SerializeField] private TypeSpawningProps _typeSpawning;
        [SerializeField] [Min(2)] private int _density = 50;
        [SerializeField] [Range(0f, 1f)] private float _spawnChance = 1f;
        [SerializeField] [Min(0.01f)] private float _width = 2f;
        [SerializeField] private bool _localHeightOffset;
        [SerializeField] private float _heightOffset;
        [SerializeField] private bool _rotationBasedOnSplineDir;
        [SerializeField] [Range(0f, 90f)] private float _rotationOffset;
        [SerializeField] private float _positionOffset;
        [SerializeField] private Vector2 _scaleOffsetMinMax;
        
        [Header("Prefabs")]
        [SerializeField] private List<GameObject> _props = new();
        [SerializeField] private Transform _transformParent;
        
        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;
        private int _splinesCount;

        private Random _random;
        private List<GameObject> _propsSpawn = new();
        private int _randomForOnSideSpawn;

        private void Awake()
        {
            _randomForOnSideSpawn = Mathf.RoundToInt(Mathf.Lerp(100f, 2f, _spawnChance));
            Debug.Log(_randomForOnSideSpawn);
        }

        [ContextMenu("SpawnProps")]
        public void DebugSpawnProps()
        {
            SpawnProps(_seed);
        }
        
        public void SpawnProps(uint seed)
        {
            _seed = seed;
            _random = new Random(_seed);
            
            foreach (var prop in _propsSpawn)
            {
                Destroy(prop);
            }
            _propsSpawn.Clear();
            
            if (!_splineContainer) return;
            if (_splineContainer.Splines.Count <= 0) return;
            
            _splinesCount = _splineContainer.Splines.Count;
            
            float step = 1f / (_density - 1);

            for (int i = 0; i < _splinesCount; i++)
            {
                for (int j = 0; j < _density; ++j)
                {
                    if (_random.NextFloat(0f, 1f) > _spawnChance) continue;
                    
                    float t = j * step;
                    _splineContainer.Evaluate(i, t, out float3 pos, out float3 tan, out float3 up);
                    
                    Vector3 position = pos;
                    Vector3 forward = math.normalize(tan);
                    Vector3 right = Vector3.Cross(up, forward).normalized;
                    
                    // ROTATION
                    float leftRotationY;
                    float rightRotationY;
                    if (_rotationBasedOnSplineDir)
                    {
                        float splineYaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

                        leftRotationY = splineYaw + _random.NextFloat(-_rotationOffset, _rotationOffset);
                        rightRotationY = splineYaw + 180f + _random.NextFloat(-_rotationOffset, _rotationOffset);
                    }
                    else
                    {
                        leftRotationY = _random.NextFloat(-_rotationOffset, _rotationOffset);
                        rightRotationY = _random.NextFloat(-_rotationOffset, _rotationOffset);
                    }
                    
                    int randomSide = 0;
                    if (_typeSpawning == TypeSpawningProps.OneSideRandom)
                    {
                        randomSide = _random.NextInt(0, 2);
                    }
                    
                    if (_typeSpawning == TypeSpawningProps.BothSide)
                    {
                        Vector3 leftPos = position - right * (_width * 0.5f);
                        Vector3 rightPos = position + right * (_width * 0.5f);

                        leftPos = transform.InverseTransformPoint(leftPos);
                        rightPos = transform.InverseTransformPoint(rightPos);

                        leftPos += new Vector3(_random.NextFloat(-_positionOffset, _positionOffset) + transform.position.x, _heightOffset, _random.NextFloat(-_positionOffset, _positionOffset) + transform.position.z);
                        rightPos += new Vector3(_random.NextFloat(-_positionOffset, _positionOffset) + transform.position.x, _heightOffset, _random.NextFloat(-_positionOffset, _positionOffset) + transform.position.z);

                        if (!_localHeightOffset)
                        {
                            leftPos.y = _heightOffset;
                            rightPos.y = _heightOffset;
                        }
                    
                        GameObject leftObj = Instantiate(_props[_random.NextInt(0, _props.Count)], leftPos, Quaternion.Euler(0, leftRotationY, 0), _transformParent);
                        GameObject rightObj = Instantiate(_props[_random.NextInt(0, _props.Count)], rightPos, Quaternion.Euler(0, rightRotationY, 0), _transformParent);
                
                        float leftScaleModifier = _random.NextFloat(_scaleOffsetMinMax.x, _scaleOffsetMinMax.y);
                        float rightScaleModifier = _random.NextFloat(_scaleOffsetMinMax.x, _scaleOffsetMinMax.y);
                        leftObj.transform.localScale *= leftScaleModifier;
                        rightObj.transform.localScale *= rightScaleModifier;
                                    
                        _propsSpawn.Add(leftObj);
                        _propsSpawn.Add(rightObj);
                    }
                    else if (_typeSpawning == TypeSpawningProps.Left || randomSide == 0)
                    {
                        Vector3 leftPos = position - right * (_width * 0.5f);

                        leftPos = transform.InverseTransformPoint(leftPos);

                        leftPos += new Vector3(_random.NextFloat(-_positionOffset, _positionOffset) + transform.position.x, _heightOffset, _random.NextFloat(-_positionOffset, _positionOffset) + transform.position.z);

                        if (!_localHeightOffset)
                        {
                            leftPos.y = _heightOffset;
                        }
                    
                        GameObject leftObj = Instantiate(_props[_random.NextInt(0, _props.Count)], leftPos, Quaternion.Euler(0, leftRotationY, 0), _transformParent);
                
                        float leftScaleModifier = _random.NextFloat(_scaleOffsetMinMax.x, _scaleOffsetMinMax.y);
                        leftObj.transform.localScale *= leftScaleModifier;
                                    
                        _propsSpawn.Add(leftObj);
                    }
                    else if (_typeSpawning == TypeSpawningProps.Right || randomSide == 1)
                    {
                        Vector3 rightPos = position + right * (_width * 0.5f);

                        rightPos = transform.InverseTransformPoint(rightPos);

                        rightPos += new Vector3(_random.NextFloat(-_positionOffset, _positionOffset) + transform.position.x, _heightOffset, _random.NextFloat(-_positionOffset, _positionOffset) + transform.position.z);

                        if (!_localHeightOffset)
                        {
                            rightPos.y = _heightOffset;
                        }
                    
                        GameObject rightObj = Instantiate(_props[_random.NextInt(0, _props.Count)], rightPos, Quaternion.Euler(0, rightRotationY, 0), _transformParent);
                
                        float rightScaleModifier = _random.NextFloat(_scaleOffsetMinMax.x, _scaleOffsetMinMax.y);
                        rightObj.transform.localScale *= rightScaleModifier;
                                    
                        _propsSpawn.Add(rightObj);
                    }
                }
            }
        }

        public void SetDensity(int density)
        {
            _density = density;
        }
    }

    public enum TypeSpawningProps
    {
        BothSide,
        Left,
        Right,
        OneSideRandom
    }
}