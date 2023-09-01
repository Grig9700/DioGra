using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class EventNodeView : GraphNodeView
{
    public EventNodeView(GraphNode node)
    {
        Setup(node, "event");
        
        GenerateInputPort();
        GenerateOutputPort();
        
        var eventNode = Node as EventNode;
        Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(eventNode);
        var container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        inspector.Add(container);
    }
}
