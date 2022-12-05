using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DialogueNodeView : GraphNodeView
{
    public string speaker;
    public string dialogueText;
    
    
    public DialogueNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        style.left = node.position.x;
        style.top = node.position.y;
        
        GenerateInputPort();
        GenerateOutputPort();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
    
}
