using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TempContainer : ScriptableObject
{
    public ExposedVariableType VariableType;
    public ExposedProperties Property;
    
    public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }
}
