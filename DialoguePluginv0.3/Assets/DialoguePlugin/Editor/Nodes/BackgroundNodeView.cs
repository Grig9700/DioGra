using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class BackgroundNodeView : GraphNodeView
{
    public BackgroundNodeView(GraphNode node)
    {
        Setup(node, "background");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var backgroundNode = Node as BackgroundNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(backgroundNode);
        var container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(container);
    }
}
