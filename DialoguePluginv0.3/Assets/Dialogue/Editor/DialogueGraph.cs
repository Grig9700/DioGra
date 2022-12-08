using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool _showMiniMap = true;
    private bool _showBlackboard = true;
    private Blackboard _blackboard;
    private Toolbar _toolbar;

    public DialogueGraph(DialogueGraphView graphView)
    {
        _graphView = graphView;
    }
    
    /*[MenuItem("Window/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>("Dialogue Graph");
        //window.titleContent = new GUIContent("Dialogue Graph");
    }*/

    private void OnEnable()
    {
        _showBlackboard = true;
        _showMiniMap = true;
        GenerateToolbar();
        GenerateMiniMap();
        FixFunkyButtons();
    }

    private void FixFunkyButtons()
    {
        ShowHideMiniMap();
        ShowHideBlackboard();
    }
    
    private void GenerateMiniMap()
    {
        _miniMap = new MiniMap { anchored = true };
        var coords = _graphView.contentViewContainer.WorldToLocal(
            new Vector2(position.width - _miniMap.maxWidth - 10, 30));
        _miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
        _graphView.Add(_miniMap);
    }

    private void GenerateToolbar()
    {
        _toolbar = new Toolbar();

        _toolbar.Add(new Button(ShowHideBlackboard) { text = "S/H Board" });
        _toolbar.Add(new Button(ShowHideMiniMap) { text = "S/H Map" });

        var filenameTextField = new TextField("File Name:");
        filenameTextField.SetValueWithoutNotify(_filename);
        filenameTextField.MarkDirtyRepaint();
        filenameTextField.RegisterValueChangedCallback(evt => _filename = evt.newValue);
        _toolbar.Add(filenameTextField);
        
        _toolbar.Add(new Button( () => RequestDataOperation(true)) {text = "Save"});
        _toolbar.Add(new Button( () => RequestDataOperation(false)) {text = "Load"});
        
        //_toolbar.Add(new Button( GetListeners.GetListenerNumber()) {text = "Test Parameters"});
        
        // var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node"); });
        // nodeCreateButton.text = "Create Node";
        // toolbar.Add(nodeCreateButton);
        
        rootVisualElement.Add(_toolbar);
    }

    private void ShowHideBlackboard()
    {
        if (_graphView.Blackboard == null) return;
        _showBlackboard = !_showBlackboard;
        _graphView.Blackboard.visible = _showBlackboard;
    }

    private void ShowHideMiniMap()
    {
        if (_miniMap == null) return;
        _showMiniMap = !_showMiniMap;
        _miniMap.visible = _showMiniMap;
    }
    
    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_filename))
        {
            Debug.LogError("Invalid filename, please enter functional filename");
            return;
        }

        //var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        //if (save)
        //    saveUtility.SaveData(_filename);
        //else
        //    saveUtility.LoadData(_filename, this);
    }
    
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
