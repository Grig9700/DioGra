using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ScriptNodeCalls : ScriptableObject
{
    public List<MonoScript> calls = new List<MonoScript>();
    
    [HideInInspector] 
    public List<int> selectedMethod = new List<int>();
    
    [HideInInspector]
    public bool expandCalls = true;
    [HideInInspector] 
    public List<List<ScriptableObject>> parameters = new List<List<ScriptableObject>>();
    [HideInInspector]
    public string methodName;

    // public UnityAction actionScripts;
    //
    // public UnityEvent eventScripts;
    //
    // public void InvokeScripts()
    // {
    //     eventScripts.Invoke();
    // }
    
// #if UNITY_EDITOR
//     public static ScriptableObject CreateValue<T>(string valueName, ScriptableObject assetTarget)
//     {
//         var scriptableValue = CreateInstance<ScriptableValue<T>>();
//         scriptableValue.name = valueName;
//
//         AssetDatabase.AddObjectToAsset(scriptableValue, assetTarget);
//         AssetDatabase.SaveAssets();
//         
//         EditorUtility.SetDirty(scriptableValue);
//         EditorUtility.SetDirty(assetTarget);
//            
//         return scriptableValue;
//     }
// #endif
}

