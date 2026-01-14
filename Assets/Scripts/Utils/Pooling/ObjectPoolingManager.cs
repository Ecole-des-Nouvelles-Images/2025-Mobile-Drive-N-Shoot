using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Utils.Pooling
{
    public class ObjectPoolingManager : MonoBehaviour
    {
        private bool _dontDestroyOnLoad; // Note: À exposer en [SerializeField] si besoin
        private static Transform[] _poolContainers;
        private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools = new();
        private static Dictionary<GameObject, GameObject> _cloneToPrefabMap = new();

        public enum PoolType { VisualFX, GameObjects, SoundFX }

        private void Awake()
        {
            // Nettoyage pour éviter les fuites mémoire lors du changement de scène (statique)
            _objectPools.Clear();
            _cloneToPrefabMap.Clear();
            SetupEmpties();
        }

        void SetupEmpties()
        {
            GameObject holder = new GameObject("Empty Pool");
            if (_dontDestroyOnLoad) DontDestroyOnLoad(holder);

            // On initialise le tableau selon la taille de l'enum
            _poolContainers = new Transform[System.Enum.GetValues(typeof(PoolType)).Length];
            
            _poolContainers[(int)PoolType.VisualFX] = CreateContainer("Visual FX", holder.transform);
            _poolContainers[(int)PoolType.GameObjects] = CreateContainer("GameObjects", holder.transform);
            _poolContainers[(int)PoolType.SoundFX] = CreateContainer("SoundFX", holder.transform);
        }

        private Transform CreateContainer(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            return go.transform;
        }

        static void ReleaseObject(GameObject obj) => obj.SetActive(false);
        static void DestroyObject(GameObject obj) => _cloneToPrefabMap.Remove(obj);

        public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
        {
            if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
            {
                Transform targetParent = _poolContainers[(int)poolType];
                if (obj.transform.parent != targetParent)
                {
                    obj.transform.SetParent(targetParent);
                }

                if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                {
                    pool.Release(obj);
                }
            }
        }

        #region Internal Logic

        private static ObjectPool<GameObject> GetOrCreatePool(GameObject prefab, Transform initialParent, PoolType poolType)
        {
            if (!_objectPools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool<GameObject>(
                    createFunc: () => {
                        // Optimisation: Instantiate direct au bon endroit
                        GameObject obj = Instantiate(prefab, _poolContainers[(int)poolType]);
                        _cloneToPrefabMap[obj] = prefab;
                        return obj;
                    },
                    actionOnGet: (obj) => { }, 
                    actionOnRelease: ReleaseObject,
                    actionOnDestroy: DestroyObject,
                    collectionCheck: false // Gain de performance
                );
                _objectPools.Add(prefab, pool);
            }
            return pool;
        }

        #endregion

        #region Public API (Optimized)

        public static T SpawnObject<T>(GameObject objectToSpawn, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects) where T : Object
        {
            var pool = GetOrCreatePool(objectToSpawn, null, poolType);
            GameObject obj = pool.Get();

            Transform t = obj.transform;
            t.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject)) return obj as T;
            
            // TryGetComponent est plus rapide que GetComponent
            if (obj.TryGetComponent<T>(out T component)) return component;
            
            return null;
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, position, rotation, poolType);
        }

        public static T SpawnObject<T>(T typePrefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, position, rotation, poolType);
        }

        // Overload with Parent
        public static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Vector3 localPosition, Quaternion localRotation, PoolType poolType = PoolType.GameObjects) where T : Object
        {
            var pool = GetOrCreatePool(objectToSpawn, parent, poolType);
            GameObject obj = pool.Get();

            Transform t = obj.transform;
            t.SetParent(parent);
            t.localPosition = localPosition;
            t.localRotation = localRotation;
            t.localScale = Vector3.one;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject)) return obj as T;
            if (obj.TryGetComponent<T>(out T component)) return component;
            
            return null;
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Vector3 localPosition, Quaternion localRotation, PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, parent, localPosition, localRotation, poolType);
        }

        public static T SpawnObject<T>(T typePrefab, Transform parent, Vector3 localPosition, Quaternion localRotation, PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, parent, localPosition, localRotation, poolType);
        }

        #endregion
    }
}