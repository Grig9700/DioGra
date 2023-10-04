using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class IntVariable : VariableObject
{
    [SerializeField] private int baseValue;

#if UNITY_EDITOR
    
    [MenuItem("Assets/Create Variable/Int Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<IntVariable>("New Int Variable", "Variables");
    }
    
#endif
    
    public int Value { get; private set; }

    private void OnValidate()
    {
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public void SetValue(int value, bool ignoreInvoke = false)
    {
        Value = value;
        if (ignoreInvoke)
            return;
        valueChanged.Invoke();
    }
}
