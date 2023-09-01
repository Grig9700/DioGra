using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class EventNodeView : GraphNodeView
{
    private IMGUIContainer _container;
    
    public EventNodeView(GraphNode node)
    {
        Setup(node, "event");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var eventNode = Node as EventNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(eventNode);
        _container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(_container);
    }
}
