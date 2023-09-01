using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class IfNodeView : GraphNodeView
{
    public IfNodeView(GraphNode node)
    {
        Setup(node, "if");

        GenerateInputPort();
        
        GenerateOutputPort("True");
        GenerateOutputPort("False");

        var ifNode = Node as IfNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(ifNode);
        var container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(container);
    }
}
