using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphNode : Node
{
    public string GUID;
    public bool entryPoint = false;
    
    public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }
}
