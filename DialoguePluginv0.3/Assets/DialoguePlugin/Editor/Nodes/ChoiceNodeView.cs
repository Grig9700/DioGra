using UnityEngine;

public sealed class ChoiceNodeView : GraphNodeView
{
    public ChoiceNodeView(GraphNode node, DialogueGraphView graphView)
    {
        Setup(node, "choice");

        GenerateInputPort();
        GenerateMultiOutputButton(graphView, node);
        
        var choiceNode = Node as ChoiceNode;
        foreach (var choicePortName in choiceNode.childPortName)
        {
            AddChoicePort(graphView, node, choicePortName);
        }
    }
}
