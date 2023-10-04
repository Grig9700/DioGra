using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class FloatVariable : VariableObject
{
    [SerializeField] private float baseValue;

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create Variable/Float Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<FloatVariable>("New Float Variable", "Variables");
    }
    
#endif
    
    public float Value { get; private set; }

    private void OnValidate()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public void SetValue(float value, bool ignoreInvoke = false)
    {
        Value = value;
        if (ignoreInvoke)
            return;
        valueChanged.Invoke();
    }
}
