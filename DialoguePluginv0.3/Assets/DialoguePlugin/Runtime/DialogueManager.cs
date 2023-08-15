using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueManager : MonoBehaviour
{
    public VariableContainer variableContainer;
    public DialogueContainer container;
    public GameObject dialogueUI;
    
    private Stack<GraphNode> _previousNodes;

    private GraphNode _currentNode;
    private GraphNode _targetNode;

    public VisualElement Character;
    public Label Name;
    public Label Text;
    public VisualElement ButtonField;
    
    private VisualElement _root;
    
    private void OnEnable()
    {
        _previousNodes = new Stack<GraphNode>();
    }

    private void SetUp()
    {
        dialogueUI.SetActive(true);
        _root = dialogueUI.GetComponent<UIDocument>().rootVisualElement;
        Character = _root.Q<VisualElement>("character");
        Name = _root.Q<Label>("name");
        Text = _root.Q<Label>("text");
        ButtonField = _root.Q<VisualElement>("buttonField");
    }
    
    private void Update()
    {
        if (!Input.anyKey)
            return;
        
        if (Input.GetKeyDown(KeyCode.Tab) || 
            Input.GetKeyDown(KeyCode.RightArrow)||
            Input.GetKeyDown(KeyCode.D))
        {
            Next();
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace) || 
            Input.GetKeyDown(KeyCode.LeftArrow)||
            Input.GetKeyDown(KeyCode.A))
        {
            Back();
        }

        //Debugging function
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (container)
                LoadNewDialogue(container);
            else
                LoadNewDialogue("bot");
        }
    }

    public void Back()
    {
        if (_previousNodes.Count <= 1)
            return;
        
        //Debug.Log($"[Dev] back button management options need implementing");
        
        _currentNode.Clear();
        
        _currentNode = _previousNodes.Pop();
        
        Next();
    }

    public void Next()
    {
        if (_currentNode == null)
        {
            EndDialogue();
            return;
        }

        switch (_currentNode.Run(this))
        {
            case NodeReturn.End:
                EndDialogue();
                break;
            case NodeReturn.Next:
                GetAndStartNext();
                break;
            case NodeReturn.PrepNext:
                GetNext();
                break;
            case NodeReturn.Wait:
                break;
            default:
                Debug.LogError("Entered Default on Next", this);
                break;
        }
    }
    
    private void GetNext()
    {
        _previousNodes.Push(_currentNode);
        _currentNode = _targetNode;
    }
    private void GetAndStartNext()
    {
        _previousNodes.Push(_currentNode);
        _currentNode = _targetNode;
        Next();
    }

    public void SetTargetNode(GraphNode targetNode = null, bool nextImmediately = false)
    {
        _targetNode = targetNode;
        
        if (nextImmediately)
            GetAndStartNext();
    }

    private void GetNodeByGuid(string getNodeByGUID, bool getStartNode = false)
    {
        var tempNode = getStartNode ? container.graphNodes.First(node => node.GUID == getNodeByGUID) 
            : _currentNode.children.First(node => node.GUID == getNodeByGUID);
        
        if (tempNode == null)
        {
            Debug.LogWarning($"No node by GUID {getNodeByGUID} was found");
            EndDialogue();
            return;
        }
        if (!getStartNode)
            _previousNodes.Push(_currentNode);
        _currentNode = tempNode;
    }

    public void Skip()
    {
        Debug.LogError($"[Dev] Skip not implemented");
    }
    
    public void EndDialogue(bool loadCall = false)
    {
        dialogueUI.SetActive(false);
    }

    public void LoadNewDialogue(string filename)
    {
        EndDialogue(true);

        var tempContainer = FindAssets.GetResourceByName<DialogueContainer>(filename);

        if (tempContainer == null)
        {
            Debug.LogError($"[Designer][Writer] The dialogue named {filename} does not exist");
            return;
        }

        container = tempContainer;

        _currentNode = container.graphNodes.First(n => n.entryNode);
        SetUp();
        
        Next();
    }
    
    public void LoadNewDialogue(DialogueContainer tempContainer)
    {
        EndDialogue(true);
        
        container = tempContainer;
        
        _currentNode = container.graphNodes.First(n => n.entryNode);
        SetUp();
        
        Next();
    }
}
