using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public sealed class IfNodeView : GraphNodeView
{
    private IMGUIContainer _container;
    
    public IfNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;
        AddToClassList("if");
        
        style.left = node.position.x;
        style.top = node.position.y;

        GenerateInputPort();

        var ifNode = Node as IfNode;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(ifNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(_container);
        
        GenerateOutputPort("True");
        GenerateOutputPort("False");
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
