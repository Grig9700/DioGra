using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class IntVariable : VariableObject
{
    [SerializeField] private int baseValue;
    
    public int Value { get; private set; }
    
    public IntVariable()
    {
        Value = baseValue;
    }

    public override void ResetToDefault()
    {
        Value = baseValue;
    }
    
    public override void SetValue(int value, bool ignoreInvoke = false)
    {
        Value = value;
        if (ignoreInvoke)
            return;
        valueChanged.Invoke();
    }

#if UNITY_EDITOR


    [MenuItem("Assets/Create/Variables/Int Variable")]
    private static void MakeVariable()
    {
        CreateAssets.CreateScriptableObjectAsset<IntVariable>("New Int Variable", "Variables");
    }
    
#endif
}
