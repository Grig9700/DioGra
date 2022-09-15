using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _filename = "New Narrative";
    private MiniMap _miniMap;
    private bool _showMiniMap;

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
        GenerateMiniMap(true);
    }

    private void GenerateMiniMap(bool showMap)
    {
        if (showMap)
        {
            _showMiniMap = true;
            _miniMap = new MiniMap { anchored = true };
            _miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(_miniMap);
        }
        else
        {
            _showMiniMap = false;
            _graphView.Remove(_miniMap);
        }
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

        toolbar.Add(new Button(() => GenerateMiniMap(!_showMiniMap)) { text = "Show/Hide Map" });

        var filenameTextField = new TextField("File Name:");
        filenameTextField.SetValueWithoutNotify(_filename);
        filenameTextField.MarkDirtyRepaint();
        filenameTextField.RegisterValueChangedCallback(evt => _filename = evt.newValue);
        toolbar.Add(filenameTextField);
        
        toolbar.Add(new Button( () => RequestDataOperation(true)) {text = "Save"});
        toolbar.Add(new Button( () => RequestDataOperation(false)) {text = "Load"});
        
        var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node"); });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_filename))
        {
            Debug.LogError("Invalid filename, please enter functional filename");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
            saveUtility.SaveData(_filename);
        else
            saveUtility.LoadData(_filename);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
