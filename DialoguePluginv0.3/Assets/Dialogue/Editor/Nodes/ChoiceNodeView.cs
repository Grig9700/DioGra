using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceNodeView : GraphNodeView
{
    private DialogueGraphView _graphView;
    
    public ChoiceNodeView(GraphNode node, DialogueGraphView graphView)
    {
        _graphView = graphView;
        
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        SetPosition(new Rect(node.position, defaultNodeSize));
        
        GenerateInputPort();
        GenerateMultiOutputButton(_graphView);
        
        ChoiceNode choiceNode = Node as ChoiceNode;
        foreach (var choicePortName in choiceNode.outputOptions)
        {
            AddChoicePort(graphView, choicePortName);
        }
    }
}
