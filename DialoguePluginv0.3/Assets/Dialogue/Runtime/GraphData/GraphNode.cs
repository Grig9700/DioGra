using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class GraphNode : ScriptableObject
{
    public bool entryNode;
    //public string nodeName;
    public string GUID;
    public Vector2 position;
    
    //public List<GraphNodeLink> Links = new List<GraphNodeLink>();
    public List<GraphNode> children = new List<GraphNode>();

    //public string speaker;
    //public string dialogueText;

    public ExposedVariableType VariableType;
    public ExposedProperties Property;

    public DialogueGraphNodeType dialogueGraphNodeType;

    public enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }
}

/*public struct GraphNodeLink
{
    public string parent;
    public string child;
}*/

public enum DialogueGraphNodeType
{
    DialogueNode,
    ChoiceNode,
    IfNode
}