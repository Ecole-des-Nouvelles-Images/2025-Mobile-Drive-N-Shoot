using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Utils.Pooling
{
    /// <summary>
    /// Generic Object Pooling manager that can be used for any gameobject, you can add any new categoris if neccessary
    /// It can be used en lieu of the normal intanciate method, and will create a pool for any gameobject you want pooled.
    /// It has an Activate, Deactivate and Destroy method
    /// And can be used free of a parent or with one.
    /// </summary>
    public class ObjectPoolingManager : MonoBehaviour
    {
        private bool _dontDestroyOnLoad;

        private GameObject _emptyHolder;

        //Types of objects we can pool
        //Add new Game objects here for a new pool, then add to the Enum, SetupEmpties and the Switch case
        private static GameObject _visualFXEmpty;
        private static GameObject _gameObjectEmpty;
        private static GameObject _soundFXEmpty;
    
        private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
        private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

        //Add to the enum if you want a specific space for objects to be pooled
        public enum PoolType
        {
            VisualFX,
            GameObjects,
            SoundFX
        }

        private void Awake()
        {
            _objectPools = new();
            _cloneToPrefabMap = new ();
        
            SetupEmpties();
        }

        void SetupEmpties()
        {
            _emptyHolder = new GameObject("Empty Pool");
        
            _visualFXEmpty =  new GameObject("Visual FX");
            _visualFXEmpty.transform.SetParent(_emptyHolder.transform);
        
            _gameObjectEmpty =  new GameObject("GameObjects");
            _gameObjectEmpty.transform.SetParent(_emptyHolder.transform);
        
            _soundFXEmpty = new GameObject("SoundFX");
            _soundFXEmpty.transform.SetParent(_emptyHolder.transform);
        
            if (_dontDestroyOnLoad) DontDestroyOnLoad(_visualFXEmpty.transform.root);
        }

        static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.VisualFX:
                    return _visualFXEmpty;
            
                case PoolType.GameObjects:
                    return _gameObjectEmpty;
            
                case PoolType.SoundFX:
                    return _soundFXEmpty;
            
                default:
                    return null;
            }
        }

        static void GetObject(GameObject obj)
        {
            //Optional logic for when you call the object
        }

        static void ReleaseObject(GameObject obj)
        {
            obj.SetActive(false);
        }

        static void DestroyObject(GameObject obj)
        {
            if (_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Remove(obj);
            }
        }
    
        public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
        {
            if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
            {
                GameObject parentObject = SetParentObject(poolType);

                if (obj.transform.parent != parentObject.transform)
                {
                    obj.transform.SetParent(parentObject.transform);
                }

                if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                {
                    pool.Release(obj);
                }
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} doesn't have a pool");
            }
        }
        /// <summary>
        /// SpawnObject looks up the pools dictionary if it has the key (GameObject) you want to spawn.
        /// If not, a new pool is created for it.
        ///
        /// You can also pass any component type (SFX, Rigidbody, VFX, etc...)
        /// </summary>
        #region NoParent

        static void CreatePool(GameObject prefab, Vector3 position, Quaternion rotation,
            PoolType poolType = PoolType.GameObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, position, rotation, poolType),
                actionOnGet: GetObject,
                actionOnRelease: ReleaseObject,
                actionOnDestroy: DestroyObject
            );
        
            _objectPools.Add(prefab, pool);
        }
    
        static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation,
            PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);
        
            GameObject obj = Instantiate(prefab, position, rotation);
        
            prefab.SetActive(true);

            GameObject parent = SetParentObject(poolType);
            obj.transform.SetParent(parent.transform);
        
            return obj;
        }
    
        static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects) where T : Object
        {
            if (!_objectPools.ContainsKey(objectToSpawn)) CreatePool(objectToSpawn, spawnPosition, spawnRotation, poolType);

            GameObject obj = _objectPools[objectToSpawn].Get();

            if (obj)
            {
                if (!_cloneToPrefabMap.ContainsKey(obj))
                {
                    _cloneToPrefabMap.Add(obj, objectToSpawn);
                }
            
                obj.transform.position = spawnPosition;
                obj.transform.rotation = spawnRotation;
                obj.SetActive(true);
            
                if (typeof(T) == typeof(GameObject)) return obj as T;

                T component = obj.GetComponent<T>();
                if (!component)
                {
                    Debug.LogError($"Object {objectToSpawn.name} doesn't have a component of type {typeof(T)}");
                    return null;
                }
            
                return component;
            }
        
            return null;
        }

        public static T SpawnObject<T>(T typePrefab, Vector3 spawnPosition, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, spawnPosition, spawnRotation, poolType);
        }    
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, spawnPosition, spawnRotation, poolType);
        }

        #endregion

        #region SetParent
    
        static void CreatePool(GameObject prefab, Transform parent, Quaternion rotation,
            PoolType poolType = PoolType.GameObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, parent, rotation, poolType),
                actionOnGet: GetObject,
                actionOnRelease: ReleaseObject,
                actionOnDestroy: DestroyObject
            );
        
            _objectPools.Add(prefab, pool);
        }
    
        static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rotation,
            PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);
        
            GameObject obj = Instantiate(prefab, parent);
        
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = Vector3.one;
        
            prefab.SetActive(true);
        
            return obj;
        }

        static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects) where T : Object
        {
            if (!_objectPools.ContainsKey(objectToSpawn)) CreatePool(objectToSpawn, parent, spawnRotation, poolType);

            GameObject obj = _objectPools[objectToSpawn].Get();

            if (obj)
            {
                if (!_cloneToPrefabMap.ContainsKey(obj))
                {
                    _cloneToPrefabMap.Add(obj, objectToSpawn);
                }
            
                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = spawnRotation;
                obj.SetActive(true);
            
                if (typeof(T) == typeof(GameObject)) return obj as T;

                T component = obj.GetComponent<T>();
                if (!component)
                {
                    Debug.LogError($"Object {objectToSpawn.name} doesn't have a component of type {typeof(T)}");
                    return null;
                }
            
                return component;
            }
        
            return null;
        }
    
        public static T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, parent, spawnRotation, poolType);
        }    
        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, parent, spawnRotation, poolType);
        }

        #endregion
    
    }
}