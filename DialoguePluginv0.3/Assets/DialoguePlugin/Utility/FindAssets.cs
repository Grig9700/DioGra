using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FindAssets
{
    public static List<T> GetAllInstances<T>() where T : ScriptableObject
    {
        return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<T>)
            .ToList();
    }
    
    public static List<T> GetResourcesByName<T>(string name) where T : ScriptableObject
    {
        var temp = GetAllInstances<T>();

        return temp.Where(t => t.name == name).ToList();
    }
    
    public static T GetResourceByName<T>(string name, bool suppressNoFileFound = false) where T : ScriptableObject
    {
        var temps = GetResourcesByName<T>(name);

        if (temps.Any())
            return temps.First();
        
        if (!suppressNoFileFound)
            Debug.LogError($"No files by name : {name} of type {typeof(T).Name} were found");
        return null;
    }
}
