using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if ((Object)_instance == (Object)null)
                {
                    _instance = Object.FindFirstObjectByType<T>();
                    if (Object.FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
                    {
                        return _instance;
                    }
                    if ((Object)_instance == (Object)null)
                    {
                        GameObject gameObject = new GameObject();
                        _instance = gameObject.AddComponent<T>();
                        gameObject.name = "(singleton) " + typeof(T).ToString() + Object.FindObjectsByType<T>(FindObjectsSortMode.None).Length;
                        Object.DontDestroyOnLoad(gameObject);
                    }
                }
                return _instance;
            }
        }
    }
}