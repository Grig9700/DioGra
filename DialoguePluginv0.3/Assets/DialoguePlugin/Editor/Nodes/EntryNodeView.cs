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
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
}
