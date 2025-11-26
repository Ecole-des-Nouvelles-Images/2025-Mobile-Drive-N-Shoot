using System;
using System.Collections;
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
        private TerrainLeveling _terrainLeveling;

        private Random _random;
        
        public void Setup(MapManager mapManager, bool haveCheckPoint, bool haveItem)
        {
            _mapManager = mapManager;

            if (haveCheckPoint)
            {
                _checkpointGameObject.SetActive(true);
            }

            if (!haveItem)
            {
                StartCoroutine(AsyncGeneration());
            }
            else
            {
                _splineRoad.BuildRoad(true);
                _terrainLeveling.StartCoroutine(nameof(_terrainLeveling.AsyncTerrainLeveling));
            }
        }

        private void Awake()
        {
            _splineKnotHandler = GetComponentInChildren<SplineKnotHandler>();
            _splineRoad = GetComponentInChildren<DynamicSplineRoad>();
            _splineProps = GetComponentsInChildren<DynamicSplineProps>();
            _terrainLeveling = GetComponentInChildren<TerrainLeveling>();

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
            _splineKnotHandler.GenerateSpline(_mainSeed);
            yield return new WaitForSeconds(0.1f);
            
            _splineRoad.BuildRoad(true);
            yield return new WaitForSeconds(0.1f);
            
            foreach (var prop in _splineProps)
            {
                prop.SpawnProps(_mainSeed);
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