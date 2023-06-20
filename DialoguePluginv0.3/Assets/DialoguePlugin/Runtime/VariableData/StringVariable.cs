using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class StringVariable : VariableObject
{
    [SerializeField] private string baseValue;

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create Variable/String Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<FloatVariable>("New String Variable", "Variables");
    }
    
#endif
    
    public string Value { get; private set; }

    private void OnValidate()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public void SetValue(string value)
    {
        Value = value;
        valueChanged.Invoke();
    }
}
