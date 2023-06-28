using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public class DialogueNode : GraphNode
{
    public DialogueCharacter speaker;
    public int expressionSelector;
    public string dialogueText;
    
    public override NodeReturn Run(DialogueManager manager)
    {
        if (speaker.expressions != null && speaker.expressions?[expressionSelector].image != null)
            manager.Character.style.backgroundImage = new StyleBackground(speaker.expressions[expressionSelector].image);
        
        manager.Name.text = speaker.name;
        manager.Text.text = dialogueText;
        
        manager.SetTargetNode(IsNullOrEmpty() ? null : children?.First());
        
        return NodeReturn.PrepNext;
    }

    public override void Clear(){}
}
