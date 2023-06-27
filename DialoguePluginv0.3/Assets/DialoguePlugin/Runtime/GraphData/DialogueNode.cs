using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class DialogueNode : GraphNode
{
    public DialogueCharacter speaker;
    public int expressionSelector;
    public string dialogueText;

    public override NodeReturn Run(SceneLayout scene, DialogueManagerLegacy managerLegacy)
    {
        if (speaker.expressions == null || speaker.expressions?[expressionSelector].image == null)
            scene.dialogueCharacter.gameObject.SetActive(false);
        else
        {
            scene.dialogueCharacter.gameObject.SetActive(true);
            scene.dialogueCharacter = speaker.expressions[expressionSelector].image;
        }
        
        scene.nameField.text = speaker.name;
        scene.textField.text = dialogueText;
        
        managerLegacy.SetTargetNode(IsNullOrEmpty() ? null : children?.First());
        
        return NodeReturn.PrepNext;
    }

    public override void Clear(){}
}
