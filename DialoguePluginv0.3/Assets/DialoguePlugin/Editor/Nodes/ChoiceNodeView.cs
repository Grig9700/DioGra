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
        Setup(node, "choice");
        
        _graphView = graphView;
        
        GenerateInputPort();
        GenerateMultiOutputButton(_graphView, node);
        
        var choiceNode = Node as ChoiceNode;
        foreach (var choicePortName in choiceNode.childPortName)
        {
            AddChoicePort(graphView, node, choicePortName);
        }
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
}
