using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EntryNodeView : GraphNodeView
{
    
    
    public EntryNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        SetPosition(new Rect(node.position, defaultNodeSize));
        
        capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Deletable;
        
        GenerateOutputPort();
    }
}
