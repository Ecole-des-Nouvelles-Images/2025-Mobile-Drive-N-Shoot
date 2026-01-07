using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using Utils.Game;

namespace MapGeneration
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainLeveling : MonoBehaviour
    {
        [Header("Terrain Data Settings")]
        [SerializeField] private int _heightmapResolution;
        [SerializeField] private Vector3 _terrainSize;

        [Header("Leveling Settings")]
        [SerializeField] private float _raiseAmount = 1f;
        [SerializeField] private float _rayHeight = 10f;
        [SerializeField] private string _roadTag = "Road";

        [Header("ASync Settings")]
        [SerializeField] private int _yieldEveryYRows = 8;

        [Header("References")]
        [SerializeField] private Terrain _terrain;
        [SerializeField] private NavMeshSurface _navMeshSurface;

        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;

        private TerrainData _terrainData;
        private Transform _terrainTransform;
        private Vector3 _terrainPos;
        private NavMeshData _navMeshData;

        private static readonly RaycastHit[] _raycastHitsBuffer = new RaycastHit[8];

        [ContextMenu("DebugLevelingTerrain")]
        public void DebugLevelingTerrain()
        {
            StartCoroutine(AsyncTerrainLeveling());
        }

        public IEnumerator AsyncTerrainLeveling()
        {
            CreateIndependentTerrain();

            int res = _terrainData.heightmapResolution;
            float[,] heights = new float[res, res];

            float invResMinus1 = 1f / (res - 1);
            Vector3 size = _terrainData.size;

            for (int y = 0; y < res; y++)
            {
                float ny = y * invResMinus1;
                float worldZ = _terrainPos.z + ny * size.z;

                for (int x = 0; x < res; x++)
                {
                    float nx = x * invResMinus1;
                    float worldX = _terrainPos.x + nx * size.x;

                    Vector3 rayOrigin = new Vector3(
                        worldX,
                        _terrainPos.y + _rayHeight,
                        worldZ
                    );

                    int hitCount = Physics.RaycastNonAlloc(
                        rayOrigin,
                        Vector3.down,
                        _raycastHitsBuffer,
                        _rayHeight
                    );

                    bool hitRoad = false;
                    for (int i = 0; i < hitCount; i++)
                    {
                        if (_raycastHitsBuffer[i].collider.CompareTag(_roadTag))
                        {
                            hitRoad = true;
                            break;
                        }
                    }

                    if (!hitRoad)
                        heights[y, x] = _raiseAmount;
                }

                if (_yieldEveryYRows > 0 && y % _yieldEveryYRows == 0)
                    yield return null;
            }

            _terrainData.SetHeights(0, 0, heights);
            yield return null;

            BakeNavMeshSurface();
            yield return null;

            EventBus.OnModuleFinishedGeneration?.Invoke();
        }

        private void CreateIndependentTerrain()
        {
            _terrainTransform = _terrain.transform;
            _terrainPos = _terrainTransform.position;

            _terrainData = new TerrainData
            {
                heightmapResolution = _heightmapResolution,
                size = _terrainSize
            };

            _terrain.terrainData = _terrainData;

            float[,] heights = new float[_heightmapResolution, _heightmapResolution];
            _terrainData.SetHeights(0, 0, heights);
        }

        private void BakeNavMeshSurface()
        {
            if (_navMeshSurface) _navMeshSurface.BuildNavMesh();
        }
        
        // private void AsyncBakeNavMeshSurface()
        // {
        //     _navMeshData = new NavMeshData();
        //     _navMeshSurface.navMeshData = _navMeshData;
        //     _navMeshSurface.UpdateNavMesh(_navMeshData);
        // }
    }
}
