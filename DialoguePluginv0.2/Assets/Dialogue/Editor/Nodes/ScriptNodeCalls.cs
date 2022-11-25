using System;
using System.Collections;
using System.Collections.Generic;
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

    // public UnityAction actionScripts;
    //
    // public UnityEvent eventScripts;
    //
    // public void InvokeScripts()
    // {
    //     eventScripts.Invoke();
    // }
}

