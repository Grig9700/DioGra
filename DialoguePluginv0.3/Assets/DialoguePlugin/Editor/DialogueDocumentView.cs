using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class DialogueDocumentView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<DialogueDocumentView, VisualElement.UxmlTraits> {}
    
    private DialogueContainer _container;
    private List<GraphNode> _trace;
    private ScrollView _document;
    private VisualTreeAsset _dialogueEntry;
    private VisualTreeAsset _multipleOutcomeEntry;
    private List<DialogueCharacter> _characters;

    public void PopulateView(DialogueContainer container)
    {
        _container = container;
        _trace = new List<GraphNode>();
        _document = this.Q<ScrollView>();
        _dialogueEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/DialogueEntry.uxml");
        _multipleOutcomeEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/MultipleOutcomeEntry.uxml");
        _characters = FindAssets.GetAllInstances<DialogueCharacter>();
        
        TraceDialogue(_container.graphNodes.First(node => node.entryNode));
        
        AddEntries();
    }

    private void TraceDialogue(GraphNode node)
    {
        int iterator = 0;
        while (node.children.Any())
        {
            node = node.children[0];

            if (node is DialogueNode or ChoiceNode or IfNode)
            {
                _trace.Add(node);
            }

            iterator++;
            if (iterator > 10000)
                break;
        }
    }

    private void AddEntries()
    {
        for (int i = 0; i < _trace.Count; i++)
        {
            switch (_trace[i])
            {
                case DialogueNode dialogueNode:
                    _document.Add(DialogueEntry(dialogueNode));
                    break;
                
                case ChoiceNode choiceNode:
                    _document.Add(ChoiceEntry(choiceNode));
                    break;
                
                case IfNode ifNode:
                    _document.Add(IfEntry(ifNode));
                    break;

                default:
                    Debug.LogError($"{_trace[i]} is not implemented");
                    break;
            }
        }
    }

    private VisualElement DialogueEntry(DialogueNode node)
    {
        var element = _dialogueEntry.Instantiate();
            
        element.Q<TextField>().value = node.dialogueText;
        
        var characterSelector = element.Q<DropdownField>("characterSelect");
        characterSelector.choices = _characters.Select(character => character.name).ToList();
        characterSelector.RegisterValueChangedCallback(evt => OnCharacterChange(evt, node, element));

        if (!node.speaker || !characterSelector.choices.Any())
            return element;
        var charIndex = GetIndexOfDropdownChoice(characterSelector.choices, node.speaker.name);
        characterSelector.index = charIndex;
        
        var expressionSelector = element.Q<DropdownField>("expressionSelect");
        expressionSelector.choices = _characters[charIndex].expressions.Select(expression => expression.emotion).ToList();

        if (!expressionSelector.choices.Any() || node.expressionSelector < 0)
            return element;
        expressionSelector.index = node.expressionSelector;
        expressionSelector.RegisterValueChangedCallback(evt => OnExpressionChange(evt, node, element));
        
        var display = element.Q<VisualElement>("expressionDisplay");
        display.style.backgroundImage = new StyleBackground(node.speaker.expressions[node.expressionSelector].image);

        return element;
    }

    private VisualElement ChoiceEntry(ChoiceNode node, int index = 0)
    {
        var element = _multipleOutcomeEntry.Instantiate();
        
        element.Q<TextField>().value = node.outputOptions.Any() ? node.outputOptions[index] : $"No choice options have been created";

        IndexChildPort(element, node, index);

        return element;
    }
    
    private VisualElement IfEntry(IfNode node, int index = 0)
    {
        var element = _multipleOutcomeEntry.Instantiate();

        element.Q<TextField>().value = node.children.Any() ? 
            $"{node.comparisonTarget.name} {node.numComp[node.numTracker]} {node.comparisonValue} is: {_trace.Contains(node.children[index])}" : 
            $"{node.comparisonTarget.name} {node.numComp[node.numTracker]} {node.comparisonValue} is: True";
        
        IndexChildPort(element, node, index);

        return element;
    }

    private void IndexChildPort(VisualElement element, GraphNode node, int index)
    {
        var selector = element.Q<DropdownField>();
        selector.choices = node.childPortName;
        selector.index = index;
    }
    
    private int GetIndexOfDropdownChoice(IReadOnlyList<string> choices, string choice)
    {
        for (var i = 0; i < choices.Count; i++)
        {
            if (choices[i] != choice)
                continue;
            return i;
        }

        return 0;
    }

    private void OnCharacterChange(ChangeEvent<string> evt, DialogueNode node, VisualElement element)
    {
        node.speaker = _characters.First(c => c.name == evt.newValue);

        node.expressionSelector = 0;
        var expressionSelector = element.Q<DropdownField>("expressionSelect");
        expressionSelector.UnregisterValueChangedCallback(expressionEvt => OnExpressionChange(expressionEvt, node, element));
        expressionSelector.choices = node.speaker.expressions.Select(expression => expression.emotion).ToList();
        expressionSelector.index = node.expressionSelector;
        expressionSelector.RegisterValueChangedCallback(expressionEvt => OnExpressionChange(expressionEvt, node, element));
        
        element.Q<VisualElement>("expressionDisplay").style.backgroundImage = new StyleBackground(node.speaker.expressions[node.expressionSelector].image);
    }

    private void OnExpressionChange(ChangeEvent<string> evt, DialogueNode node, VisualElement element)
    {
        node.expressionSelector =
            GetIndexOfDropdownChoice(node.speaker.expressions.Select(expression => expression.emotion).ToList(), evt.newValue);
        
        element.Q<VisualElement>("expressionDisplay").style.backgroundImage = new StyleBackground(node.speaker.expressions[node.expressionSelector].image);
    }

    private void ChangeChoice(IfNode node, bool nextChoice)
    {
        var choice = 0;
        for (var i = 0; i < node.children.Count; i++)
        {
            if (!_trace.Contains(node.children[i]))
                continue;
            choice = i;
            
            ClearOldBranch(node);
            
            //_trace.Add();
            
            //TraceDialogue();
            
            break;
        }
    }

    private void ChangeChoice(ChoiceNode node)
    {
        ClearOldBranch(node);
        
        
    }

    private void ClearOldBranch(GraphNode node)
    {
        var index = 0;
        for (var i = 0; i < _trace.Count; i++)
        {
            if (_trace[i] != node)
                continue;
            index = i;
            break;
        }

        for (var i = _trace.Count; i >= index; i--)
        {
            _trace.Remove(_trace[i]);
        }
    }
}
