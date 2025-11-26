using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGeneration
{
    public class MapManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _checkPointFrequency;
        [SerializeField] [Range(0f, 1f)] private float _itemChanceRate;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _mapModulePrefab;
        
        private Queue<GameObject> _mapModules = new();
        private int _lastCheckPoint = 1;
        
        private void Awake()
        {
            GameObject newModule = Instantiate(_mapModulePrefab, transform);
            newModule.GetComponent<MapModuleHandler>().Setup(this, false);
            _mapModules.Enqueue(newModule);
            _lastCheckPoint++;
        }
        
        public void SpawnMapModule()
        {
            Vector3 lastPos = _mapModules.Last().transform.position;

            GameObject newModule;
            bool haveCheckPoint;

            if (_lastCheckPoint % _checkPointFrequency == 0)
            {
                haveCheckPoint = true;
                _lastCheckPoint = 1;
            }
            else
            {
                haveCheckPoint = false;
                _lastCheckPoint++;
            }
            
            if (Random.Range(0, 1) > _itemChanceRate)
            {
                newModule = Instantiate(_mapModulePrefab, lastPos + new Vector3(0, 0, 100), Quaternion.identity, transform);
            }
            else
            {
                // FAIRE SPAWN UN MAPMODULEPREFAB AVEC ITEM
                newModule = Instantiate(_mapModulePrefab, lastPos + new Vector3(0, 0, 100), Quaternion.identity, transform);
            }
            
            newModule.GetComponent<MapModuleHandler>().Setup(this, haveCheckPoint);
            
            _mapModules.Enqueue(newModule);

            if (_mapModules.Count > 3)
            {
                Destroy(_mapModules.Dequeue());
            }
        }
    }
}