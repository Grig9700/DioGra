using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class BackgroundNodeView : GraphNodeView
{
    private IMGUIContainer _container;
    
    public BackgroundNodeView(GraphNode node)
    {
        Setup(node, "background");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        UnityEngine.Object.DestroyImmediate(editor);
        
        var backgroundNode = Node as BackgroundNode;
        editor = Editor.CreateEditor(backgroundNode);
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
