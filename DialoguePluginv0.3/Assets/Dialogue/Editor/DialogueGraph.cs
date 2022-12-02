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
    
    private ExposedVariableType _exposedVariableType;
    //private ButtonWithMenu typeButton;
    
    private enum ExposedVariableType
    {
        Bool,
        Float,
        Int,
        String
    }

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

    public void CreateBlackBoardElements()
    {
        EnumField typeField = new EnumField(_exposedVariableType);
        typeField.RegisterValueChangedCallback(evt => _exposedVariableType = (ExposedVariableType)evt.newValue);
        _blackboard.Add(typeField);
        //_blackboard.Add(new BlackboardSection{ title = "Exposed Variables"});
        _blackboard.title = "Exposed Variables";
        _blackboard.scrollable = true;
    }
    
    private void GenerateBlackboard()
    {
        _blackboard = new Blackboard(_graphView);
        
        CreateBlackBoardElements();
        
        _blackboard.addItemRequested = blackboard1 => { AddBlackboardProperty(_exposedVariableType);};
        _blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (_graphView.ExposedPropertiesList.Any(x => x.name == newValue))
            {
                Debug.LogError("That property name already exists");
                return;
            }

            var propertyIndex = _graphView.ExposedPropertiesList.FindIndex(x => x.name == oldPropertyName);
            _graphView.ExposedPropertiesList[propertyIndex].name = newValue;
            ((BlackboardField)element).text = newValue;
            //blackboardField.RegisterCallback<ContextualMenuPopulateEvent>(MyMenuPopulateCB);
        };
        _blackboard.moveItemRequested = (blackboard, i, arg3) =>
        {
            
        };
        
        _blackboard.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Blackboard = _blackboard;
        _graphView.Add(_blackboard);
    }

    private void AddBlackboardProperty(ExposedVariableType current)
    {
        switch (current)
        {
            case ExposedVariableType.Bool:
                _graphView.AddPropertyToBlackboard(new ExposedProperty<bool>()); 
                break;
            case ExposedVariableType.Float:
                _graphView.AddPropertyToBlackboard(new ExposedProperty<float>()); 
                break;
            case ExposedVariableType.Int:
                _graphView.AddPropertyToBlackboard(new ExposedProperty<int>()); 
                break;
            case ExposedVariableType.String:
                _graphView.AddPropertyToBlackboard(new ExposedProperty<string>()); 
                break;
        }
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
        //_graphView.StretchToParentSize();
        //rootVisualElement.Add(_graphView);
        
        // _graphView = new DialogueGraphView(this)
        // {
        //     name = "Dialogue Graph"
        // };
        //
        // _graphView.StretchToParentSize();
        // rootVisualElement.Add(_graphView);
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
        _toolbar.Add(new Button( ClearGraph) {text = "Clear"});
        
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

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
            saveUtility.SaveData(_filename);
        //else
        //    saveUtility.LoadData(_filename, this);
    }

    private void ClearGraph()
    {
        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        saveUtility.Clear();
        CreateBlackBoardElements();
    }
    
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
