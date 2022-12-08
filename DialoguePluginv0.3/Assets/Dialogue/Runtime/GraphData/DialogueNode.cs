using System;
using UnityEngine;

[Serializable]
public class DialogueNode : GraphNode
{
    public Character speaker;
    public int expressionSelector;
    public string dialogueText;
}
