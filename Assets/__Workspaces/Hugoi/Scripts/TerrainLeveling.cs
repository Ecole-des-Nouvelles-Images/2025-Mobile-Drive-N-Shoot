using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainLeveling : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject SplineMesh;
    }
}
