using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class DialogueNodeView : GraphNodeView
{
    public DialogueCharacter speaker;
    public string dialogueText;

    private IMGUIContainer _container;
    
    public DialogueNodeView(GraphNode node)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;
        AddToClassList("dialogue");
        
        style.left = node.position.x;
        style.top = node.position.y;
        
        GenerateInputPort();
        GenerateOutputPort();
        
        Object.DestroyImmediate(editor);
        
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialoguePlugin/Editor/Node.uss"); 
        //styleSheets.Add(styleSheet);
        
        var dialogueNode = Node as DialogueNode;
        editor = Editor.CreateEditor(dialogueNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        outputContainer.Add(_container);
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
            outputContainer.Remove(_container);
        else
            outputContainer.Add(_container);
        MarkDirtyRepaint();
    }
}
