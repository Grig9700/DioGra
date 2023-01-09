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
        CreateFolders();
        
        CreateVariable<FloatVariable>("Float");
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
    
    public void SetValue(float value)
    {
        Value = value;
        valueChanged.Invoke();
    }
}
