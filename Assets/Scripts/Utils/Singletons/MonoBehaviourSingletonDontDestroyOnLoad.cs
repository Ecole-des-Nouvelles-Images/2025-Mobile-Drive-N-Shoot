using UnityEngine;

namespace Utils.Singletons
{
    public abstract class MonoBehaviourSingletonDontDestroyOnLoad<T> : MonoBehaviour 
        where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _initialized;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                // Try find existing instance first
                _instance = FindAnyObjectByType<T>();

                if (_instance != null)
                {
                    MoveToDontDestroyOnLoad(_instance.gameObject);
                    return _instance;
                }

                // Create new instance
                var obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
                MoveToDontDestroyOnLoad(obj);

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_initialized) return;

            if (_instance == null)
            {
                _instance = this as T;
                MoveToDontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _initialized = true;
        }

        private static void MoveToDontDestroyOnLoad(GameObject obj)
        {
            DontDestroyOnLoad(obj);
        }
    }
}