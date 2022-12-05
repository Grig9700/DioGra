using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphNodeView : Node
{
    public GraphNode Node;
    
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