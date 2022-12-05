using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow //: ScriptableObject, ISearchWindowProvider
{
    /*private DialogueGraphView _graphView;
    private EditorWindow _window;
    private Texture2D _indentIcon;

    public void Initialize(EditorWindow window, DialogueGraphView graphView)
    {
        _window = window;
        _graphView = graphView;

        _indentIcon = new Texture2D(1, 1);
        _indentIcon.SetPixel(0,0,new Color(0,0,0,0));
        _indentIcon.Apply();
    }*/
    
    /*public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Nodes"), 1),
            new SearchTreeEntry(new GUIContent("Dialogue Node", _indentIcon))
            {
                userData = new DialogueNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("Choice Node", _indentIcon))
            {
                userData = new ChoiceNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("If Node", _indentIcon))
            {
                userData = new IfNode(), level = 2
            },
            new SearchTreeEntry(new GUIContent("Script Node", _indentIcon))
            {
                userData = new ScriptNode(), level = 2
            },
            //new SearchTreeEntry(new GUIContent("'Tis a bop"))
        };
        return tree;
    }*/

    /*public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
             context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        
        switch (searchTreeEntry.userData)
        {
            case DialogueNode dialogueNode:
                _graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;
            case ChoiceNode choiceNode:
                _graphView.CreateNode("Choice Node", localMousePosition);
                return true;
            case IfNode ifNode:
                _graphView.CreateNode("If Node", localMousePosition);
                return true;
            case ScriptNode scriptNode:
                _graphView.CreateNode("Script Node", localMousePosition);
                return true;
            default:
                return false;
        }
    }*/
}
