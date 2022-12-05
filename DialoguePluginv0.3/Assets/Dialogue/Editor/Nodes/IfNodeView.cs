using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IfNodeView : GraphNodeView
{
    public ExposedVariableType VariableType;
    public ExposedProperties Property;
    
    
    public IfNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        style.left = node.position.x;
        style.top = node.position.y;
        
        GenerateInputPort();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
    
}
