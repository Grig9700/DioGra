using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class FloatVariable : VariableObject
{
    [SerializeField] private float baseValue;
    
    public float Value { get; private set; }

    public FloatVariable()
    {
        Value = baseValue;
    }

    public override void ResetToDefault()
    {
        Value = baseValue;
    }

    public override void SetValue(float value, bool ignoreInvoke = false)
    {
        Value = value;
        if (ignoreInvoke)
            return;
        valueChanged.Invoke();
    }

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create/Variables/Float Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<FloatVariable>("New Float Variable", "Variables");
    }
    
#endif
}
