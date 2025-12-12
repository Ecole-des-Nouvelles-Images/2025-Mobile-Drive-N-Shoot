using UnityEngine;

namespace Utils.Singletons
{
    public abstract class MonoBehaviourSingletonDontDestroyOnLoad<T> : MonoBehaviour 
        where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                // Try find existing instance first
                _instance = FindAnyObjectByType<T>();

                if (_instance != null)
                {
                    DontDestroyOnLoad(_instance.gameObject);
                    return _instance;
                }

                // Create new instance
                var obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                // First instance → keep it
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                // Another instance already exists → destroy this one
                Destroy(gameObject);
            }
        }
    }
}