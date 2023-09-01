using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class DialogueNodeView : GraphNodeView
{
    private readonly IMGUIContainer _container;
    
    public DialogueNodeView(GraphNode node)
    {
        Setup(node, "dialogue");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var dialogueNode = Node as DialogueNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(dialogueNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(_container);
    }
}
