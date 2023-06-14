using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChoiceNode : GraphNode
{
    public List<string> outputOptions = new List<string>();

    private List<GameObject> _buttons = new List<GameObject>();

    private DialogueManager _manager;

    public override NodeReturn Run(SceneLayout scene, DialogueManager manager)
    {
        if (IsNullOrEmpty())
            return NodeReturn.End;

        _manager = manager;
        
        ClearButtons();
                
        var buttonsToMake = children;//container.NodeLinks.Where(x => x.baseNodeGUID == _getNodeByGUID).ToList();
        var rectT = scene.buttonPrefab.GetComponent<RectTransform>().rect;
        var height = rectT.height + scene.buttonSpacing;
        for (int i = 0; i < buttonsToMake.Count; i++)
        {
            int index = i;
            var obj = Instantiate(scene.buttonPrefab, scene.viewPortContent.transform);
            obj.GetComponent<RectTransform>().transform.localPosition = new Vector2(rectT.width * 0.5f, -100 + i * -height);
            obj.GetComponent<Button>().onClick.AddListener(() => { Button(buttonsToMake[index]);});
            obj.GetComponentInChildren<Text>().text = childPortName[index];
            _buttons.Add(obj);
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
        {
            DestroyImmediate(obj);
        }
        _buttons.Clear();
    }
}
