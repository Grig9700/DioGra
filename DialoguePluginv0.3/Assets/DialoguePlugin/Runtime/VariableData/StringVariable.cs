using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class StringVariable : VariableObject
{
    [SerializeField] private string baseValue = "";
    
    public string Value { get; private set; }
    
    public StringVariable()
    {
        Value = baseValue;
    }

    public override void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public override void SetValue(string value, bool ignoreInvoke = false)
    {
        Value = value;
        if (ignoreInvoke)
            return;
        valueChanged.Invoke();
    }

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create/Variables/String Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<StringVariable>("New String Variable", "Variables");
    }
    
#endif
}
