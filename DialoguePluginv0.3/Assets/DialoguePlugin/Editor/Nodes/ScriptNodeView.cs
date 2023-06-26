using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[Serializable]
public sealed class ScriptNodeView : GraphNodeView
{
    private IMGUIContainer _container;
    
    public ScriptNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;
        AddToClassList("script");
        
        style.left = node.position.x;
        style.top = node.position.y;
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var scriptNode = Node as ScriptNode;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(scriptNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(_container);
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }

    protected override void ToggleCollapse()
    {
        base.ToggleCollapse();
        if(!expanded)
            inspector.Remove(_container);
        else
            inspector.Add(_container);
        MarkDirtyRepaint();
    }
}
