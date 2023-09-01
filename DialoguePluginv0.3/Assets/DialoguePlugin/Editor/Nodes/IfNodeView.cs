using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class IfNodeView : GraphNodeView
{
    private IMGUIContainer _container;
    
    public IfNodeView(GraphNode node)
    {
        Setup(node, "if");

        GenerateInputPort();

        var ifNode = Node as IfNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(ifNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(_container);
        
        GenerateOutputPort("True");
        GenerateOutputPort("False");
    }
}
