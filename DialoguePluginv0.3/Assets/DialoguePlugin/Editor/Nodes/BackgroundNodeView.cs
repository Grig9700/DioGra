using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class BackgroundNodeView : GraphNodeView
{
    private IMGUIContainer _container;
    
    public BackgroundNodeView(GraphNode node)
    {
        Setup(node, "background");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var backgroundNode = Node as BackgroundNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(backgroundNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(_container);
    }
}
