using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphNode : Node
{
    public GraphNodeData NodeData;
    
    public string GUID;
    public bool entryPoint = false;

    public Editor Editor;
    public ScriptNodeCalls call;
    
    
    public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }
}
