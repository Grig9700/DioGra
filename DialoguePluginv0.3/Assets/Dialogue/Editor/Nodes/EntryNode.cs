using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryNode : GraphNode
{
    public EntryNode(GraphNodeData nodeData)
    {
        NodeData = nodeData;
        title = nodeData.name;
        viewDataKey = nodeData.GUID;

        style.left = nodeData.position.x;
        style.top = nodeData.position.y;
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        NodeData.position.x = newPos.xMin;
        NodeData.position.y = newPos.yMin;
    }
}
