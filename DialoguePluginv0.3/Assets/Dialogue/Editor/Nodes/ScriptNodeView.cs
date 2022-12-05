using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ScriptNodeView : GraphNodeView
{
    public ScriptNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        style.left = node.position.x;
        style.top = node.position.y;
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
    
    public UnityEvent scripts;

    private void InvokeScripts()
    {
        scripts.Invoke();
    }
}
