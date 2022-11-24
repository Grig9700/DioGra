using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphNode : Node
{
    public string GUID;
    public bool entryPoint = false;

    public GraphNodeData NodeData;
    public Editor Editor;
    
    public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }
}
