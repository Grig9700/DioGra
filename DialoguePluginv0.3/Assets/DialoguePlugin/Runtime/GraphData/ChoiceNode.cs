using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ChoiceNode : GraphNode
{
    private List<VisualElement> _buttons = new List<VisualElement>();

    private DialogueManager _manager;

    public override NodeReturn Run(DialogueManager manager)
    {
        if (IsNullOrEmpty())
            return NodeReturn.End;

        _manager = manager;
        
        ClearButtons();
        
        for (var i = 0; i < children.Count; i++)
        {
            var index = i;
            var button = new Button(() => { Button(children[index]); })
            {
                text = childPortName[index],
                name = "choiceButton"
            };
            manager.ButtonField.Add(button);
            _buttons.Add(button);
        }

        return NodeReturn.Wait;
    }
    
    public override void Clear()
    {
        _manager = null;
        ClearButtons();
    }

    private void Button(GraphNode child)
    {
        ClearButtons();
        
        _manager.SetTargetNode(child, true);
    }
    
    private void ClearButtons()
    {
        foreach (var obj in _buttons)
            _manager.ButtonField.Remove(obj);
        
        _buttons.Clear();
    }
}
