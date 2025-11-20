using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class MapModuleHandler : MonoBehaviour
    {
        [SerializeField] private MapManager _mapManager;
        
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