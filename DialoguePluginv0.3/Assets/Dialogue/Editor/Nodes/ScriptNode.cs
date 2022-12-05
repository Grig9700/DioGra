using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ScriptNode : GraphNode
{
    public ScriptNode(GraphNodeData nodeData)
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
    
    public UnityEvent scripts;

    private void InvokeScripts()
    {
        scripts.Invoke();
    }
}
