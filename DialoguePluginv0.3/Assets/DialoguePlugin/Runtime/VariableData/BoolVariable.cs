using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class BoolVariable : VariableObject
{
    [SerializeField] private bool baseValue;

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create Variable/Bool Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<FloatVariable>("New Bool Variable", "Variables");
    }
    
#endif
    
    public bool Value { get; private set; }

    private void OnValidate()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public void SetValue(bool value)
    {
        Value = value;
        valueChanged.Invoke();
    }
}
