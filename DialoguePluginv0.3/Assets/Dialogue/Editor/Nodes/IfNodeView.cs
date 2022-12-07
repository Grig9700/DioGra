using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
        
        
        IfNode ifNode = Node as IfNode;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(ifNode);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        outputContainer.Add(container);
        
        GenerateOutputPort("True");
        GenerateOutputPort("False");
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
}
