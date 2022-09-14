using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;

    [MenuItem("Window/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>("Dialogue Graph");
        //window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
    }

    private void ConstructGraph()
    {
        _graphView = new DialogueGraphView()
        {
            name = "Dialogue Graph"
        };
        
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        
        var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node"); });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        
        rootVisualElement.Add(toolbar);
    }
    
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}