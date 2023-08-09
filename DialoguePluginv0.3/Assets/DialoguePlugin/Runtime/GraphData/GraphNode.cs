using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GraphNode : ScriptableObject
{
    public bool entryNode;
    [HideInInspector]
    public string GUID;
    [HideInInspector]
    public Vector2 position;
    
    [HideInInspector]
    public List<GraphNode> children = new List<GraphNode>();
    [HideInInspector]
    public List<string> childPortName = new List<string>();

    public abstract NodeReturn Run(DialogueManager manager);
    public abstract void Clear();
    
    protected bool IsNullOrEmpty()
    {
        return children?.Count <= 0 || children == null;
    }
}

public enum NodeReturn
{
    Wait,
    Next,
    PrepNext,
    End
}