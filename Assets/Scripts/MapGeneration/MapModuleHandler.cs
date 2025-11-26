using System.Collections;
using UnityEngine;

namespace MapGeneration
{
    public class MapModuleHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _checkpointGameObject;
        
        private MapManager _mapManager;
        private SplineKnotHandler _splineKnotHandler;
        private DynamicSplineRoad _splineRoad;
        private DynamicSplineProps[] _splineProps;
        private TerrainLeveling _terrainLeveling;

        private void Awake()
        {
            _splineKnotHandler = GetComponentInChildren<SplineKnotHandler>();
            _splineRoad = GetComponentInChildren<DynamicSplineRoad>();
            _splineProps = GetComponentsInChildren<DynamicSplineProps>();
            _terrainLeveling = GetComponentInChildren<TerrainLeveling>();
        }

        private void Start()
        {
            StartCoroutine(DelayGeneration());
        }

        private IEnumerator DelayGeneration()
        {
            _splineKnotHandler.GenerateSpline();
            yield return new WaitForSeconds(0.1f);
            
            _splineRoad.Rebuild();
            yield return new WaitForSeconds(0.1f);
            
            foreach (var prop in _splineProps)
            {
                prop.SpawnProps();
                yield return new WaitForSeconds(0.1f);
            }
            
            _terrainLeveling.StartCoroutine(nameof(_terrainLeveling.RaiseTerrain));
        }

        public void Setup(MapManager mapManager, bool haveCheckPoint)
        {
            _mapManager = mapManager;

            if (haveCheckPoint)
            {
                _checkpointGameObject.SetActive(true);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _mapManager.SpawnMapModule();
            }
        }
    }
}