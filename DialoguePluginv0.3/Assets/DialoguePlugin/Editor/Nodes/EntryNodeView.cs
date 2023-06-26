using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public sealed class EntryNodeView : GraphNodeView
{
    public EntryNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;
        AddToClassList("entry");
        
        style.left = node.position.x;
        style.top = node.position.y;
        
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
