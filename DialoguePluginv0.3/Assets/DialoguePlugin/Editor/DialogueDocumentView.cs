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
    private ListView _document;
    
    
    private VisualTreeAsset _baseEntry;
    private VisualTreeAsset _dialogueEntry;
    private VisualTreeAsset _multipleOutcomeEntry;
        
        
    private List<DialogueCharacter> _characters;

    public void PopulateView(DialogueContainer container)
    {
        _container = container;
        _trace = new List<GraphNode>();
        _document = this.Q<ListView>();
        _baseEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/BaseEntry.uxml");
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
        _document.makeItem = () =>
        {
            var newEntry = _baseEntry.Instantiate();
            return newEntry;
        };

        _document.bindItem = (element, i) =>
        {
            switch (_trace[i])
            {
                case DialogueNode dialogueNode:
                    DialogueEntry(element, dialogueNode);
                    break;
                
                case ChoiceNode choiceNode:
                    ChoiceEntry(element, choiceNode);
                    break;
                
                case IfNode ifNode:
                    IfEntry(element, ifNode);
                    break;

                default:
                    Debug.LogError($"{_trace[i]} is not implemented");
                    break;
            }

        };

        _document.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        
        _document.itemsSource = _trace;
    }

    private void DialogueEntry(VisualElement element, DialogueNode node)
    {
        element.Add(_dialogueEntry.Instantiate());
            
        element.Q<TextField>().value = node.dialogueText;
            
        var characterSelector = element.Q<DropdownField>();
        characterSelector.choices = _characters.Select(character => character.name).ToList();

        for (var j = 0; j < characterSelector.choices.Count; j++)
        {
            if (characterSelector.choices[j] != node.speaker.name)
                continue;
            characterSelector.index = j;
            break;
        }
    }

    private void ChoiceEntry(VisualElement element, ChoiceNode node)
    {
        element.Add(_multipleOutcomeEntry.Instantiate());
        
        element.Q<TextField>().value = node.outputOptions[0];
    }
    
    private void IfEntry(VisualElement element, IfNode node)
    {
        element.Add(_multipleOutcomeEntry.Instantiate());

        element.Q<TextField>().value = node.children.Any() ? 
            $"{node.comparisonTarget.name} {node.numComp[node.numTracker]} {node.comparisonValue} is: {_trace.Contains(node.children[0])}" : 
            $"{node.comparisonTarget.name} {node.numComp[node.numTracker]} {node.comparisonValue} is: True";
    }

    private bool IfEntryOutput(IfNode node)
    {
        return !node.children.Any() || _trace.Contains(node.children[0]);
    }
}
