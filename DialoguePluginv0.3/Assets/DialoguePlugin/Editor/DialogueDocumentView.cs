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
    
    
    private VisualTreeAsset _dialogueEntry;
    private VisualTreeAsset _baseEntry;
        
        
    private List<DialogueCharacter> _characters;

    public void PopulateView(DialogueContainer container)
    {
        _container = container;
        _trace = new List<GraphNode>();
        _document = this.Q<ListView>();
        _dialogueEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/DialogueEntry.uxml");
        _baseEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/BaseEntry.uxml");
        _characters = FindAssets.GetAllInstances<DialogueCharacter>();
        
        TraceDialogue(_container.graphNodes.First(node => node.entryNode));
        
        AddEntries();
    }

    private void TraceDialogue(GraphNode node)
    {
        while (node.children.Any())
        {
            node = node.children[0];

            if (node is DialogueNode or ChoiceNode or IfNode)
            {
                _trace.Add(node);
            }
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
                    break;

                default:
                    Debug.LogError($"{_trace[i]} is not implemented");
                    break;
            }

        };

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
}
