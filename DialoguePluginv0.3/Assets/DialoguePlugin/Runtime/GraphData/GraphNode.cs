using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public abstract class GraphNode : ScriptableObject
{
    public bool entryNode;
    //public string nodeName;
    [HideInInspector]
    public string GUID;
    [HideInInspector]
    public Vector2 position;
    
    //public List<GraphNodeLink> Links = new List<GraphNodeLink>();
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
    
    //[HideInInspector]
    //public ExposedVariableType VariableType;
    //[HideInInspector]
    //public ExposedProperties Property;

    //[HideInInspector]
    //public DialogueGraphNodeType dialogueGraphNodeType;

    /*public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }*/
}

public enum NodeReturn
{
    Wait,
    Next,
    PrepNext,
    End
}

/*public struct GraphNodeLink
{
    public string parent;
    public string child;
}*/

/*public enum DialogueGraphNodeType
{
    DialogueNode,
    ChoiceNode,
    IfNode
}*/