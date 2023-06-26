using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class ChoiceNodeView : GraphNodeView
{
    private DialogueGraphView _graphView;
    
    public ChoiceNodeView(GraphNode node, DialogueGraphView graphView)
    {
        _graphView = graphView;
        
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;
        AddToClassList("choice");
        
        style.left = node.position.x;
        style.top = node.position.y;
        
        GenerateInputPort();
        GenerateMultiOutputButton(_graphView);
        
        var choiceNode = Node as ChoiceNode;
        foreach (var choicePortName in choiceNode.outputOptions)
        {
            AddChoicePort(graphView, choicePortName);
        }
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
}
