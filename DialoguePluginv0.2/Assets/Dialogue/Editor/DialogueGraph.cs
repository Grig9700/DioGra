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
        GenerateMiniMap();
        GenerateBlackboard();
        FixFunkyButtons();
    }

    private void FixFunkyButtons()
    {
        ShowHideMiniMap();
        ShowHideBlackboard();
    }

    private void GenerateBlackboard()
    {
        var blackboard = new Blackboard(_graphView);
        blackboard.Add(new BlackboardSection{ title = "Exposed Variables"});
        blackboard.addItemRequested = blackboard1 => { _graphView.AddPropertyToBlackboard(new ExposedProperty<string>()); };
        blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (_graphView.ExposedPropertiesString.Any(x => x.name == newValue))
            {
                Debug.LogError("Property name of that type is already present");
                return;
            }

            var propertyIndex = _graphView.ExposedPropertiesString.FindIndex(x => x.name == oldPropertyName);
            _graphView.ExposedPropertiesString[propertyIndex].name = newValue;
            ((BlackboardField)element).text = newValue;
            //blackboardField.RegisterCallback<ContextualMenuPopulateEvent>(MyMenuPopulateCB);
        };

        blackboard.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Blackboard = blackboard;
        _graphView.Add(blackboard);
    }

    private void MyMenuPopulateCB(ContextualMenuPopulateEvent evt)
    {
        //evt.menu.AppendAction("Delete", MyAction(), DropdownMenuAction.AlwaysEnabled);
    }

    void MyAction()
    {
        
    }

    private void GenerateMiniMap()
    {
        _miniMap = new MiniMap { anchored = true };
        var coords = _graphView.contentViewContainer.WorldToLocal(
            new Vector2(position.width - _miniMap.maxWidth - 10, 30));
        _miniMap.SetPosition(new Rect(coords.x, coords.y, 200, 140));
        _graphView.Add(_miniMap);
    }

    private void ConstructGraph()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };
        
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        toolbar.Add(new Button(ShowHideBlackboard) { text = "S/H Board" });
        toolbar.Add(new Button(ShowHideMiniMap) { text = "S/H Map" });

        var filenameTextField = new TextField("File Name:");
        filenameTextField.SetValueWithoutNotify(_filename);
        filenameTextField.MarkDirtyRepaint();
        filenameTextField.RegisterValueChangedCallback(evt => _filename = evt.newValue);
        toolbar.Add(filenameTextField);
        
        toolbar.Add(new Button( () => RequestDataOperation(true)) {text = "Save"});
        toolbar.Add(new Button( () => RequestDataOperation(false)) {text = "Load"});
        toolbar.Add(new Button( ClearGraph) {text = "Clear"});
        
        // var nodeCreateButton = new Button(() => { _graphView.CreateNode("Dialogue Node"); });
        // nodeCreateButton.text = "Create Node";
        // toolbar.Add(nodeCreateButton);
        
        rootVisualElement.Add(toolbar);
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

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
            saveUtility.SaveData(_filename);
        else
            saveUtility.LoadData(_filename);
    }

    private void ClearGraph()
    {
        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        saveUtility.Clear();
    }
    
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
