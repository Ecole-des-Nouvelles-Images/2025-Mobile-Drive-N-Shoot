using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

namespace __Workspaces.Hugoi.Scripts
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainLeveling : MonoBehaviour
    {
        [Header("Terrain Data Settings")]
        [SerializeField] private int _heightmapResolution;
        [SerializeField] private Vector3 _terrainSize;
        
        [Header("Leveling Settings")]
        [SerializeField] private float raiseAmount = 1f;
        [SerializeField] private float rayHeight = 10f;
        [SerializeField] private string roadTag = "Road";

        [Header("References")]
        [SerializeField] private Terrain _terrain;
    
        [Header("Spline")]
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private int _splineIndex;

        private TerrainData _terrainData;
        private bool _alreadyCreateIndependentTerrain = false;

        [ContextMenu("RaiseTerrain")]
        public void RaiseTerrain()
        {
            CreateIndependentTerrain();

            int res = _terrainData.heightmapResolution;

            float[,] heights = new float[res, res];

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    Vector3 worldPos = HeightmapToWorldPosition(x, y);

                    Ray ray = new Ray(worldPos + Vector3.up * 2f, Vector3.down);
                    RaycastHit[] hits = Physics.RaycastAll(ray, rayHeight);
                    bool hitRoad = false;
                    foreach (var raycastHit in hits)
                    {
                        if (raycastHit.collider.CompareTag(roadTag))
                        {
                            hitRoad = true;
                            break;
                        }
                    }

                    if (!hitRoad) heights[y, x] = raiseAmount;
                }
            }

            _terrainData.SetHeights(0, 0, heights);
        }

        private void CreateIndependentTerrain()
        {
            _terrainData = new TerrainData();
            _terrainData.heightmapResolution = _heightmapResolution;
            _terrainData.size = _terrainSize;
            _terrain.terrainData = _terrainData;

            float[,] heights = new float[_terrainData.heightmapResolution, _terrainData.heightmapResolution];
            _terrainData.SetHeights(0, 0, heights);
            
            _alreadyCreateIndependentTerrain = true;
        }

        private Vector3 HeightmapToWorldPosition(int x, int y)
        {
            float nx = (float)x / (_terrainData.heightmapResolution - 1);
            float ny = (float)y / (_terrainData.heightmapResolution - 1);

            float worldX = _terrain.transform.position.x + nx * _terrainData.size.x;
            float worldZ = _terrain.transform.position.z + ny * _terrainData.size.z;
            float worldY = _terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + _terrain.transform.position.y;

            return new Vector3(worldX, worldY, worldZ);
        }
    }
}
