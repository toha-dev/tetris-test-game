using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectLoader
{
    private static Dictionary<string, Object> _prefabsCached =
        new Dictionary<string, Object>();

    private static Dictionary<string, GameObject> _sceneObjectsCached =
        new Dictionary<string, GameObject>();

    public static GameObject FindInScene(string objectName)
    {
        if (_sceneObjectsCached.ContainsKey(objectName))
            return _sceneObjectsCached[objectName];

        GameObject result = GameObject.Find(objectName);
        if (result != null)
            _sceneObjectsCached.Add(objectName, result);

        return result;
    }

    public static T LoadFromResources<T>(string objectName) where T : Object
    {
        if (_prefabsCached.ContainsKey(objectName))
            if (_prefabsCached[objectName] is T)
                return _prefabsCached[objectName] as T;

        T result = Resources.Load<T>(objectName);
        if (result != null)
            _prefabsCached.Add(objectName, result);

        return result;
    }

    public static void CacheAllSceneObjects()
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in objects)
            if (!_sceneObjectsCached.ContainsKey(obj.name))
                _sceneObjectsCached.Add(obj.name, obj);
        
    }

    public static void ClearCachedSceneObjects() => _sceneObjectsCached.Clear();
}
