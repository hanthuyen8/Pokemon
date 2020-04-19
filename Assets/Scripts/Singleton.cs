using UnityEngine;

namespace Game
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject go = new GameObject();
                            _instance = go.AddComponent<T>();
                            go.name = "(singleton)" + typeof(T).ToString();

                            DontDestroyOnLoad(go);

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + go +
                            "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            //Debug.Log("[Singleton] Using instance already created: " +  _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}
