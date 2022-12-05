using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DialogueNode : GraphNode
{
    public DialogueNode(GraphNodeData nodeData)
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
    
    public string speaker;
    public string dialogueText;
}
