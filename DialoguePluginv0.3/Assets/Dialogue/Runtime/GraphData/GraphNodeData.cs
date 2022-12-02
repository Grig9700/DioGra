using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class GraphNodeData : ScriptableObject
{
    public string nodeName;
    public string GUID;
    public Vector2 position;


    public string speaker;
    public string dialogueText;

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


public enum DialogueGraphNodeType
{
    DialogueNode,
    ChoiceNode,
    IfNode
}