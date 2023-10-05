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
    
    public bool Value { get; private set; }
    
    public BoolVariable()
    {
        Value = baseValue;
    }

    public override void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public override void SetValue(bool value, bool ignoreInvoke = false)
    {
        Value = value;
        if (ignoreInvoke)
            return;
        valueChanged.Invoke();
    }

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create/Variables/Bool Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<BoolVariable>("New Bool Variable", "Variables");
    }
    
#endif
}
