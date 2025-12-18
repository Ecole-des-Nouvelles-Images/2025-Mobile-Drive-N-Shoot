using System;
using System.Collections;
using InGameHandlers;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace MapGeneration
{
    public class MapModuleHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _isRandomSeed;
        [SerializeField] private uint _mainSeed;

        [Header("References")]
        [SerializeField] private GameObject _checkpointGameObject;
        [SerializeField] private Collider _colliderCantGoBack;

        private MapManager _mapManager;
        private SplineKnotHandler _splineKnotHandler;
        private DynamicSplineRoad _splineRoad;
        private DynamicSplineProps[] _splineProps;
        private DynamicSplineProps[] _splineEnemies;
        private TerrainLeveling _terrainLeveling;
        private EnemiesSpawnerHandler _enemiesSpawnerHandler;

        private Random _random;
        private bool _haveItem;

        private Transform _cachedTransform;
        private Vector3 _cachedPosition;

        private static readonly WaitForSeconds _waitStep = new WaitForSeconds(0.2f);

        public void Setup(MapManager mapManager, bool haveCheckPoint, bool haveItem, int difficulty)
        {
            _mapManager = mapManager;
            _haveItem = haveItem;

            _enemiesSpawnerHandler.Setup(difficulty);

            if (haveCheckPoint && _checkpointGameObject != null)
                _checkpointGameObject.SetActive(true);

            StartCoroutine(AsyncGeneration());
        }

        public void Destruction()
        {
            for (int i = 0; i < _splineProps.Length; i++)
                _splineProps[i].DestroyProps();

            for (int i = 0; i < _splineEnemies.Length; i++)
                _splineEnemies[i].DestroyProps();

            Destroy(gameObject);
        }
        
        private void Awake()
        {
            _cachedTransform = transform;
            _cachedPosition = _cachedTransform.position;

            _splineKnotHandler = GetComponentInChildren<SplineKnotHandler>(true);
            _splineRoad = GetComponentInChildren<DynamicSplineRoad>(true);
            _terrainLeveling = GetComponentInChildren<TerrainLeveling>(true);
            _enemiesSpawnerHandler = GetComponentInChildren<EnemiesSpawnerHandler>(true);

            var props = GetComponentsInChildren<DynamicSplineProps>(true);
            int propsCount = props.Length;

            int propIndex = 0;
            int enemyIndex = 0;

            for (int i = 0; i < propsCount; i++)
            {
                if (props[i].CompareTag("SplineProps"))
                    propIndex++;
                else if (props[i].CompareTag("SplineEnemies"))
                    enemyIndex++;
            }

            _splineProps = new DynamicSplineProps[propIndex];
            _splineEnemies = new DynamicSplineProps[enemyIndex];

            propIndex = 0;
            enemyIndex = 0;

            for (int i = 0; i < propsCount; i++)
            {
                if (props[i].CompareTag("SplineProps"))
                    _splineProps[propIndex++] = props[i];
                else if (props[i].CompareTag("SplineEnemies"))
                    _splineEnemies[enemyIndex++] = props[i];
            }

            if (_isRandomSeed)
                GenerateRandom();
            else
                _random = new Random(_mainSeed);
        }

        private IEnumerator AsyncGeneration()
        {
            if (!_haveItem)
            {
                _splineKnotHandler.GenerateSpline(_mainSeed);
                yield return _waitStep;
            }

            _splineRoad.BuildRoad(true);
            yield return _waitStep;

            if (!_haveItem)
            {
                for (int i = 0; i < _splineProps.Length; i++)
                {
                    _splineProps[i].Setup(_cachedPosition);
                    _splineProps[i].StartCoroutine(_splineProps[i].AsyncSpawnProps(_mainSeed));
                    yield return _waitStep;
                }
            }

            for (int i = 0; i < _splineEnemies.Length; i++)
            {
                _splineEnemies[i].Setup(_cachedPosition);
                _splineEnemies[i].StartCoroutine(_splineEnemies[i].AsyncSpawnProps(_mainSeed));
                yield return _waitStep;
            }

            _terrainLeveling.StartCoroutine(_terrainLeveling.AsyncTerrainLeveling());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _mapManager.SpawnMapModule();
                GetComponent<Collider>().enabled = false;
                _colliderCantGoBack.enabled = true;
            }
        }

        private void GenerateRandom()
        {
            _random = new Random((uint)Environment.TickCount);
            _mainSeed = _random.NextUInt(0, 99999);
            _random = new Random(_mainSeed);
        }
    }
}