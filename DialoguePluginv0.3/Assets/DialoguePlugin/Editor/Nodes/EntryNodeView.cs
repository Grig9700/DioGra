using UnityEditor.Experimental.GraphView;
using UnityEngine;

public sealed class EntryNodeView : GraphNodeView
{
    public EntryNodeView(GraphNode node)
    {
        Setup(node, "entry");
        
        capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Deletable;
        
        GenerateOutputPort();
    }
}
