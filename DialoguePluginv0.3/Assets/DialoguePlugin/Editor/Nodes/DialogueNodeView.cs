using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class DialogueNodeView : GraphNodeView
{
    public DialogueNodeView(GraphNode node)
    {
        Setup(node, "dialogue");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var dialogueNode = Node as DialogueNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(dialogueNode);
        var container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(container);
    }
}
