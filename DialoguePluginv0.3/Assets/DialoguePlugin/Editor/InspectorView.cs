using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

[UxmlElement]
public partial class InspectorView : VisualElement
{
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
