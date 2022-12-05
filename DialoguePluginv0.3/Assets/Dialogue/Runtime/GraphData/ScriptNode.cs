using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[Serializable]
public class ScriptNode : GraphNode
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
}
