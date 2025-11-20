using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class MapModuleHandler : MonoBehaviour
    {
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
            _splineKnotHandler.GenerateSpline();
            _splineRoad.Rebuild();
            foreach (var prop in _splineProps)
            {
                prop.SpawnProps();
            }
            _terrainLeveling.RaiseTerrain();
        }

        public void Setup(MapManager mapManager)
        {
            _mapManager = mapManager;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _mapManager.SpawnMapModule();
            }
        }
    }
}