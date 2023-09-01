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
            inspector.Remove(_container);
        else
            inspector.Add(_container);
        MarkDirtyRepaint();
    }
}
