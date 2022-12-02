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
    public List<List<ValueType>> parameters = new List<List<ValueType>>();
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
}

