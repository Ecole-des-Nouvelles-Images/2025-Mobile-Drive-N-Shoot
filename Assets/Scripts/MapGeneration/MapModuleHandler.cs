using System;
using System.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace MapGeneration
{
    public class MapModuleHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _checkpointGameObject;
        
        private uint _mainSeed;
        
        private MapManager _mapManager;
        private SplineKnotHandler _splineKnotHandler;
        private DynamicSplineRoad _splineRoad;
        private DynamicSplineProps[] _splineProps;
        private TerrainLeveling _terrainLeveling;

        private Random _random;
        
        public void Setup(MapManager mapManager, bool haveCheckPoint)
        {
            _mapManager = mapManager;

            if (haveCheckPoint)
            {
                _checkpointGameObject.SetActive(true);
            }
        }

        private void Awake()
        {
            _splineKnotHandler = GetComponentInChildren<SplineKnotHandler>();
            _splineRoad = GetComponentInChildren<DynamicSplineRoad>();
            _splineProps = GetComponentsInChildren<DynamicSplineProps>();
            _terrainLeveling = GetComponentInChildren<TerrainLeveling>();

            _random = GenerateRandom();
        }

        private void Start()
        {
            StartCoroutine(AsyncGeneration());
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