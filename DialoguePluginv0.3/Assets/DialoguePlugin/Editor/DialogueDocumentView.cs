using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueDocumentView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<DialogueDocumentView, VisualElement.UxmlTraits> {}
    
    public DialogueContainer Container;
    private List<GraphNode> trace;

    public void PopulateView(DialogueContainer container)
    {
        Container = container;
        trace = new List<GraphNode>();
        
        TraceDialogue(container.graphNodes.First(node => node.entryNode));
    }

    private void TraceDialogue(GraphNode node)
    {
        while (node.children.Any())
        {
            node = node.children[0];

            if (node is DialogueNode or ChoiceNode or IfNode)
            {
                trace.Add(node);
            }
        }
    }
}
