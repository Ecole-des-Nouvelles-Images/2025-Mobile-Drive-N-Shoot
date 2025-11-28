using System;
using System.Collections;
using System.Linq;
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
        
        private MapManager _mapManager;
        private SplineKnotHandler _splineKnotHandler;
        private DynamicSplineRoad _splineRoad;
        private DynamicSplineProps[] _splineProps;
        private DynamicSplineProps[] _splineEnemies;
        private TerrainLeveling _terrainLeveling;
        private EnemiesSpawnerHandler _enemiesSpawnerHandler;

        private Random _random;
        private bool _haveItem;
        
        public void Setup(MapManager mapManager, bool haveCheckPoint, bool haveItem, int difficulty)
        {
            _mapManager = mapManager;
            _haveItem = haveItem;
            _enemiesSpawnerHandler.Setup(difficulty);

            if (haveCheckPoint)
            {
                _checkpointGameObject.SetActive(true);
            }

            StartCoroutine(AsyncGeneration());
        }

        private void Awake()
        {
            _splineKnotHandler = GetComponentInChildren<SplineKnotHandler>();
            _splineRoad = GetComponentInChildren<DynamicSplineRoad>();
            _splineProps = GetComponentsInChildren<DynamicSplineProps>(true)
                .Where(p => p.CompareTag("SplineProps"))
                .ToArray();
            _splineEnemies = GetComponentsInChildren<DynamicSplineProps>(true)
                .Where(p => p.CompareTag("SplineEnemies"))
                .ToArray();
            _terrainLeveling = GetComponentInChildren<TerrainLeveling>();
            _enemiesSpawnerHandler = GetComponentInChildren<EnemiesSpawnerHandler>();

            if (_isRandomSeed)
            {
                _random = GenerateRandom();
            }
            else
            {
                _random = new Random(_mainSeed);
            }
        }

        private IEnumerator AsyncGeneration()
        {
            if (!_haveItem)
            {
                _splineKnotHandler.GenerateSpline(_mainSeed);
                yield return new WaitForSeconds(0.1f);
            }
            
            _splineRoad.BuildRoad(true);
            yield return new WaitForSeconds(0.1f);

            if (!_haveItem)
            {
                foreach (var prop in _splineProps)
                {
                    prop.SpawnProps(_mainSeed);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            
            foreach (var enemy in _splineEnemies)
            {
                enemy.SpawnProps(_mainSeed);
                yield return new WaitForSeconds(0.1f);
            }
            
            _terrainLeveling.StartCoroutine(nameof(_terrainLeveling.AsyncTerrainLeveling));
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _mapManager.SpawnMapModule();
            }
        }
        
        private Random GenerateRandom()
        {
            _random = new Random((uint)Environment.TickCount);
            _mainSeed = _random.NextUInt(0, 99999);
            return _random = new Random(_mainSeed);
        }
    }
}