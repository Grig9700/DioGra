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
