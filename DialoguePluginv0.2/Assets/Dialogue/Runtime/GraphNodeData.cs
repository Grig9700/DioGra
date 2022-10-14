using System;
using UnityEngine;

[Serializable]
public class GraphNodeData
{
    public string nodeName;
    public string GUID;
    public Vector2 position;
    
    public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }
}
