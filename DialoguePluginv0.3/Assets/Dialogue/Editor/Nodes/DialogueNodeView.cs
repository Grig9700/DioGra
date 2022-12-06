using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

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

        //SetPosition(new Rect(node.position, defaultNodeSize));
        
        GenerateInputPort();
        GenerateOutputPort();
        
        
        DialogueNode dialogueNode = Node as DialogueNode;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(dialogueNode);
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        outputContainer.Add(container);
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }
}
