using UnityEngine;

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
}
