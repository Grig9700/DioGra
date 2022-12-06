using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        SetPosition(new Rect(node.position, defaultNodeSize));
        
        GenerateInputPort();
        
        
        IfNode ifNode = Node as IfNode;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(ifNode);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        outputContainer.Add(container);
    }
}
