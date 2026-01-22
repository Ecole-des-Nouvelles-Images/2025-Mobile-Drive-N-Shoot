using System.Collections.Generic;
using System.Linq;
using InGameHandlers;
using UnityEngine;

namespace MapGeneration
{
    public class MapManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _mapModuleMaxCount = 2;
        [SerializeField] private int _checkPointFrequency;
        [SerializeField] [Range(0f, 1f)] private float _itemChanceRate;
        [SerializeField] private int _pitty;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject _mapModulePrefab;
        [SerializeField] private List<GameObject> _mapModuleItemPrefabs;
        
        [Header("References")]
        [SerializeField] private DifficultyHandler _difficultyHandler;
        [SerializeField] private GameObject _cinematic;
        
        private Queue<GameObject> _mapModules = new();
        private Queue<bool> _isItemModule = new();
        private int _lastCheckPoint = 1;
        private bool _lastModuleContainsItem;
        private int _pittyCount = 0;
        
        private void Awake()
        {  
            GameObject newModule = Instantiate(_mapModulePrefab, transform);
            newModule.GetComponent<MapModuleHandler>().Setup(this, false, false, _difficultyHandler.Difficulty);
            _mapModules.Enqueue(newModule);
            _isItemModule.Enqueue(false);
            _lastCheckPoint++;
        }
        
        public void SpawnMapModule()
        {
            if (_mapModules.Count == 1) Destroy(_cinematic);
                
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
            
            if (_pittyCount >= _pitty || !_lastModuleContainsItem && Random.Range(0f, 1f) < _itemChanceRate)
            {
                int index = Random.Range(0, _mapModuleItemPrefabs.Count);
                newModule = Instantiate(_mapModuleItemPrefabs[index], lastPos + new Vector3(0, 0, 100), Quaternion.identity, transform);
                newModule.GetComponent<MapModuleHandler>().Setup(this, haveCheckPoint, true, _difficultyHandler.Difficulty);
                _lastModuleContainsItem = true;
                _pittyCount = 0;
                
                _isItemModule.Enqueue(true);
            }
            else
            {
                newModule = Instantiate(_mapModulePrefab, lastPos + new Vector3(0, 0, 100), Quaternion.identity, transform);
                newModule.GetComponent<MapModuleHandler>().Setup(this, haveCheckPoint, false, _difficultyHandler.Difficulty);
                _lastModuleContainsItem = false;
                _pittyCount++;
                
                _isItemModule.Enqueue(false);
            }
            
            _mapModules.Enqueue(newModule);

            if (_mapModules.Count > _mapModuleMaxCount)
            {
                bool haveItem = _isItemModule.Dequeue();
                GameObject oldModule = _mapModules.Dequeue();
                
                if (!haveItem)
                {
                    oldModule.GetComponent<MapModuleHandler>().Destruction();
                }
                else
                {
                    Destroy(oldModule);
                }
            }
        }
    }
}