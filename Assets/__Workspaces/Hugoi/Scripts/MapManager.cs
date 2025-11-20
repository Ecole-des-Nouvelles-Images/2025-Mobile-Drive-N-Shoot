using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public class MapManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _mapModulePrefab;
        
        private Queue<GameObject> _mapModules = new();
        
        private void Awake()
        {
            GameObject newModule = Instantiate(_mapModulePrefab, transform);
            newModule.GetComponent<MapModuleHandler>().Setup(this);
            _mapModules.Enqueue(newModule);
        }
        
        public void SpawnMapModule()
        {
            Vector3 lastPos = _mapModules.Last().transform.position;
            
            GameObject newModule = Instantiate(_mapModulePrefab, lastPos + new Vector3(0, 0, 100), Quaternion.identity, transform);
            newModule.GetComponent<MapModuleHandler>().Setup(this);
            _mapModules.Enqueue(newModule);

            if (_mapModules.Count > 3)
            {
                Destroy(_mapModules.Dequeue());
            }
        }
    }
}