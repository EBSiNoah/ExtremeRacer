using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonManager : MonoBehaviour
{
    private static SingletonManager _instance;
    private Dictionary<System.Type, MonoBehaviour> _singletons = new Dictionary<System.Type, MonoBehaviour>();

    public static SingletonManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<SingletonManager>();
                singletonObject.name = typeof(SingletonManager).ToString() + " (Singleton)";
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this ;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterSingleton<T>(T instance) where T : MonoBehaviour
    {
        if (instance == null)
        {
            Debug.LogError("Cannot register null singleton of type " + typeof(T));
            return;
        }

        var type = typeof(T);
        if (!_singletons.ContainsKey(type))
        {
            _singletons[type] = instance;
            DontDestroyOnLoad(instance);
            Debug.Log($"register {instance}");
        }
        else
        {
            Debug.LogWarning("Singleton of type " + type + " is already registered.");
        }
    }

    public T GetSingleton<T>() where T : MonoBehaviour
    {
        var type = typeof(T);
        if (_singletons.TryGetValue(type, out MonoBehaviour singleton))
        {
            return (T)singleton;
        }
        else
        {
            Debug.LogError("No singleton of type " + type + " is registered.");
            throw new System.Exception("Singleton not found");
        }
    }

    public void DebugSingleton()
    {
        Debug.Log(_instance.name);
        Debug.Log(_singletons.Count);
        foreach (var s in _singletons)
            Debug.Log(s.Key.Name);
    }
}
