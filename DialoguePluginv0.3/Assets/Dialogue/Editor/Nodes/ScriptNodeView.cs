using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[Serializable]
public class ScriptNodeView : GraphNodeView
{
    //public UnityEvent scripts;
    
    
    public ScriptNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        style.left = node.position.x;
        style.top = node.position.y;
        
        GenerateInputPort();
        GenerateOutputPort();
        
        ScriptNode scriptNode = Node as ScriptNode;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(scriptNode);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        outputContainer.Add(container);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
    
    // private void InvokeScripts()
    // {
    //     scripts.Invoke();
    // }
}
