using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> {}

    private Editor _editor;
    
    public InspectorView()
    {
        
    }

    internal void UpdateSelection(DialogueContainer dialogueContainer)
    {
        Clear();
        
        Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(dialogueContainer);
        var container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
        Add(container);
    }
}
