using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Utils.Pooling;
using Random = Unity.Mathematics.Random;

namespace MapGeneration
{
    public class DynamicSplineProps : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private uint _seed;
        [SerializeField] private TypeSpawningProps _typeSpawning;
        [SerializeField] private bool _firstAndLastSpawn = true;
        [SerializeField, Min(2)] private int _density = 50;
        [SerializeField, Range(0f, 1f)] private float _spawnChance = 1f;
        [SerializeField, Min(0.01f)] private float _width = 2f;
        [SerializeField] private bool _localHeightOffset;
        [SerializeField] private float _heightOffset;
        [SerializeField] private TypeRotationProps _typeRotationProps;
        [SerializeField, Range(0f, 90f)] private float _rotationOffset;
        [SerializeField] private float _positionOffset;
        [SerializeField] private Vector2 _scaleOffsetMinMax;
        
        [Header("ASync Settings")]
        [SerializeField] private int _yieldEverySpawningProps = 2;
        [SerializeField] private int _yieldEveryDestroyingProps = 4;

        [Header("Prefabs")]
        [SerializeField] private List<GameObject> _props = new();
        [SerializeField] private Transform _transformParent;

        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;

        private MapModuleHandler _moduleParent;
        private Vector3 _modulePosition;
        private Random _random;
        private readonly List<GameObject> _propsSpawn = new();

        private Transform _cachedTransform;
        private Vector3 _cachedTransformPos;

        private void Awake()
        {
            _cachedTransform = transform;
        }

        [ContextMenu("SpawnProps")]
        public void DebugSpawnProps()
        {
            AsyncSpawnProps(_seed);
        }

        public void Setup(MapModuleHandler parent)
        {
            _moduleParent = parent;
            _modulePosition = parent.transform.position;
        }

        public IEnumerator AsyncSpawnProps(uint seed)
        {
            _seed = seed;
            _random = new Random(_seed);

            // Return objects to pool
            int spawnCount = _propsSpawn.Count;
            for (int i = 0; i < spawnCount; i++)
                ObjectPoolingManager.ReturnObjectToPool(_propsSpawn[i]);
            _propsSpawn.Clear();

            if (!_splineContainer || _splineContainer.Splines.Count == 0)
                yield break;
            
            _cachedTransformPos = _cachedTransform.position;

            int splineCount = _splineContainer.Splines.Count;
            int propsCount = _props.Count;
            float step = 1f / (_density - 1);
            float halfWidth = _width * 0.5f;
            int densityMinusOne = _density - 1;

            // Cache spawn conditions
            bool spawnFirstLast = _firstAndLastSpawn;
            bool isOneSideRandom = _typeSpawning == TypeSpawningProps.OneSideRandom;
            bool canSpawnLeft = _typeSpawning != TypeSpawningProps.Right;
            bool canSpawnRight = _typeSpawning != TypeSpawningProps.Left;

            for (int i = 0; i < splineCount; i++)
            {
                for (int j = 0; j < _density; j++)
                {
                    if (_random.NextFloat() > _spawnChance)
                        continue;

                    if (!spawnFirstLast && (j == 0 || j == densityMinusOne))
                        continue;

                    float t = j * step;
                    _splineContainer.Evaluate(i, t, out float3 pos, out float3 tan, out float3 up);

                    float3 position = pos - (float3)_modulePosition;
                    float3 forward = math.normalize(tan);
                    float3 right = math.cross(up, forward);

                    float splineYaw = math.atan2(forward.x, forward.z) * Mathf.Rad2Deg;

                    float leftRotY = splineYaw;
                    float rightRotY = splineYaw;

                    switch (_typeRotationProps)
                    {
                        case TypeRotationProps.Local:
                            leftRotY = _random.NextFloat(-_rotationOffset, _rotationOffset);
                            rightRotY = _random.NextFloat(-_rotationOffset, _rotationOffset);
                            break;

                        case TypeRotationProps.BasedOnSplineInvert:
                            leftRotY += _random.NextFloat(-_rotationOffset, _rotationOffset);
                            rightRotY += 180f + _random.NextFloat(-_rotationOffset, _rotationOffset);
                            break;

                        case TypeRotationProps.BasedOnSplineSameDirection:
                            leftRotY += 180f + _random.NextFloat(-_rotationOffset, _rotationOffset);
                            rightRotY = leftRotY;
                            break;
                    }

                    bool spawnLeft = canSpawnLeft;
                    bool spawnRight = canSpawnRight;

                    if (isOneSideRandom)
                    {
                        spawnLeft = _random.NextBool();
                        spawnRight = !spawnLeft;
                    }

                    if (spawnLeft)
                        SpawnProp(position - right * halfWidth, leftRotY, propsCount);

                    if (spawnRight)
                        SpawnProp(position + right * halfWidth, rightRotY, propsCount);

                    if (_propsSpawn.Count % _yieldEverySpawningProps == 0)
                        yield return null;
                }
            }
        }

        private void SpawnProp(Vector3 worldPos, float rotY, int propsCount)
        {
            Vector3 localPos = _cachedTransform.InverseTransformPoint(worldPos); 
            localPos.x += _random.NextFloat(-_positionOffset, _positionOffset) + _cachedTransformPos.x; 
            localPos.z += _random.NextFloat(-_positionOffset, _positionOffset) + _cachedTransformPos.z; 
            localPos.y = _localHeightOffset ? localPos.y + _heightOffset : _heightOffset;
            
            GameObject obj = ObjectPoolingManager.SpawnObject(
                _props[_random.NextInt(0, propsCount)],
                _transformParent, localPos,
                Quaternion.Euler(0f, rotY, 0f)
                );
            
            obj.transform.localScale *= _random.NextFloat(_scaleOffsetMinMax.x, _scaleOffsetMinMax.y); _propsSpawn.Add(obj);
        }

        public void SetDensity(int density)
        {
            _density = density;
        }

        public IEnumerator AsyncDestroyProps()
        {
            int count = _propsSpawn.Count;
            for (int i = 0; i < count; i++)
            {
                if (_propsSpawn[i] != null)
                {
                    ObjectPoolingManager.ReturnObjectToPool(_propsSpawn[i]);
                    
                    if (_propsSpawn.Count % _yieldEveryDestroyingProps == 0)
                        yield return null;
                }
            }
            _propsSpawn.Clear();
            _moduleParent.CountDestroyingProps++;
        }
    }

    public enum TypeSpawningProps
    {
        BothSide,
        Left,
        Right,
        OneSideRandom
    }

    public enum TypeRotationProps
    {
        Local,
        BasedOnSplineInvert,
        BasedOnSplineSameDirection
    }
}