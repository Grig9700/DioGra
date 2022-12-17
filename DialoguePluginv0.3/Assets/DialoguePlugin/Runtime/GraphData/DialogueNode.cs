using System;
using UnityEngine;

[Serializable]
public class DialogueNode : GraphNode
{
    public DialogueCharacter speaker;
    public int expressionSelector;
    public string dialogueText;
}
