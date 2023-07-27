using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts
{

    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        private static T instance;

        private static readonly object Lock = new object();

        [SerializeField] private bool isPersistent = false;

        public static bool IsNull
        {
            get
            {
                if (instance == null)
                {
                    return true;
                }

                return false;
            }
        }

        public static T Instance
        {
            get
            {
                if (Exiting)
                {
                    Debug.Log($"{nameof(Singleton)}<{typeof(T)}>: Cikis yapiliyor. Instance dondurulmeyecek.");
                    return null;
                }

                lock (Lock)
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    var instances = FindObjectsOfType<T>(true);
                    var instance_count = instances.Length;
                    
                    if (instance_count > 0)
                    {
                        if (instance_count == 1)
                        {
                            return instance = instances[0];
                        }

                        for (var i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i]);
                        }

                        return instance = instances[0];
                    }

                    return instance = new GameObject($"{typeof(T)} (Otomatik Olusturuldu)").AddComponent<T>();
                }
            }
        }

        private void Awake()
        {
            if (isPersistent)
            {
                var instances = FindObjectsOfType<T>(false);
                
                if (instances.Length > 1)
                {
                    for (var i = 1; i < instances.Length; i++)
                    {
                        Destroy(instances[i].gameObject);
                    }
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }
            }

            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
    }

    public abstract class Singleton : MonoBehaviour
    {
        protected static bool Exiting { get; private set; }

        private void OnApplicationQuit()
        {
            Exiting = true;
        }
    }

}