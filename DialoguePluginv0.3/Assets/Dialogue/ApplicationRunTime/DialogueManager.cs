using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public RunMode runMode;
    public DialogueContainer container;

    private List<GameObject> _buttons;
    private SceneLayout _scene;
    //private string _getNodeByGUID;
    private Stack<GraphNode> _previousNodes;

    private GraphNode _currentNode;

    private GameObject _canvas;
    private void OnEnable()
    {
        _buttons = new List<GameObject>();
        _previousNodes = new Stack<GraphNode>();
        _canvas = GameObject.Find("Canvas");
        bool eventSystemExists = GameObject.Find("EventSystem");
        
        if (_canvas == null)
            Debug.LogError("Missing Canvas");
        
        if (!eventSystemExists)
            Debug.LogError("Missing Event System");
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
            LoadNewDialogue("bot"); //write name of fixed dialogue to test system
        }
    }
    
    public void Back()
    {
        if (_previousNodes.Count <= 1)
            return;
        
        //Debug.Log($"[Dev] back button management options need implementing");
        
        ClearButtons();
        _currentNode = _previousNodes.Pop();
        
        Next();
    }

    public void Next()
    {
        if (_buttons.Count > 0)
            return;

        switch (_currentNode)
        {
            case EntryNode entryNode:
                GetAndStartNext();
                break;
            
            case DialogueNode dialogueNode:
                _scene.nameField.text = dialogueNode.speaker;
                _scene.textField.text = dialogueNode.dialogueText;
                GetNext();
                break;
            
            case ChoiceNode choiceNode:
                var buttonsToMake = choiceNode.children;//container.NodeLinks.Where(x => x.baseNodeGUID == _getNodeByGUID).ToList();
                var height = _scene.buttonPrefab.GetComponent<RectTransform>().rect.height;
                for (int i = 0; i < buttonsToMake.Count(); i++)
                {
                    int index = i;
                    var obj = Instantiate(_scene.buttonPrefab, _scene.viewPort.transform);
                    obj.GetComponent<RectTransform>().transform.localPosition = new Vector2(91.5f, -100f + i * -height);
                    obj.GetComponent<Button>().onClick.AddListener(() => { Button(buttonsToMake[index]);});
                    obj.GetComponentInChildren<Text>().text = choiceNode.childPortName[index];
                    _buttons.Add(obj);
                }
                break;
            
            case IfNode ifNode:
                if (ifNode.comparisonTarget == null)
                {
                    Debug.LogError($"If node did not contain a comparison target");
                }

                if (ifNode.RunComparison())
                {
                    GetAndStartNext(ifNode.childPortName[0] == "True" ? ifNode.children[0] : ifNode.children[1]);
                }
                else
                {
                    GetAndStartNext(ifNode.childPortName[0] == "False" ? ifNode.children[0] : ifNode.children[1]);
                }
                break;
            
            case ScriptNode scriptNode:
                //scriptNode.CreateActions(this);
                //scriptNode.CallActions();
                scriptNode.InvokeFunctionCalls();
                GetAndStartNext();
                break;
            
            default:
                Debug.LogError("Entered Default on Next", this);
                break;
        }
    }

    private void GetNext(GraphNode childNode = null)
    {
        _previousNodes.Push(_currentNode);
        _currentNode = childNode == null ? _currentNode.children.First() : childNode;
    }
    private void GetAndStartNext(GraphNode childNode = null)
    {
        _previousNodes.Push(_currentNode);
        _currentNode = childNode == null ? _currentNode.children.First() : childNode;
        Next();
    }
    
    private void GetNodeByGuid(string getNodeByGUID, bool getStartNode = false)
    {
        GraphNode tempNode = getStartNode ? container.GraphNodes.First(node => node.GUID == getNodeByGUID) 
            : _currentNode.children.First(node => node.GUID == getNodeByGUID);
        
        if (tempNode == null)
        {
            Debug.LogWarning($"No node by GUID {getNodeByGUID} was found");
            return;
        }
        if (!getStartNode)
            _previousNodes.Push(_currentNode);
        _currentNode = tempNode;
    }
    
    public void Button(GraphNode child)
    {
        ClearButtons();
        GetAndStartNext(child);
    }
    
    public void Skip()
    {
        Debug.LogError($"[Dev] Skip not implemented");
    }
    
    private void ClearButtons()
    {
        foreach (var obj in _buttons)
        {
            DestroyImmediate(obj);
        }
        _buttons.Clear();
    }
    
    private void EndDialogue(bool loadCall = false)
    {
        if (_scene != null)
            DestroyImmediate(_scene.gameObject);
        
        ClearButtons();
        
        // if (!loadCall)
        //     Debug.Log($"Dialogue Ended");
    }

    public void LoadNewDialogue(string filename)
    {
        EndDialogue(true);

        DialogueContainer tempContainer =
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>($"{filename}.asset", "/Dialogues");

        if (tempContainer == null)
        {
            //Debug.Log($"[Dev] {filePath.First()}"); //Writes out the location of the file if it was found
            Debug.LogError($"[Designer][Writer] The dialogue named {filename} that you tried to load does not exist");
            return;
        }

        //Debug.Log($"{tempContainer.name}");
        container = tempContainer;

        bool sceneExists = false;
        GameObject obj = null;
        
        if (container.SceneLayoutPrefab == null)
        {
            Debug.LogWarning($"No scene layout present in Dialogue, searching Resource/Scene Layouts");
            obj = FindAndLoadResource.FindAndLoadFirstInResourceFolder<GameObject>("?cene*", "/SceneLayouts"); //finds a Scene Layout
            if (obj != null)
                sceneExists = true;
            else
            {
                Debug.LogError($"No Scene Layouts were found");
                return;
            }
        }
        
        _scene = sceneExists ? Instantiate(obj, _canvas.transform).GetComponent<SceneLayout>() : 
            Instantiate(container.SceneLayoutPrefab, _canvas.transform).GetComponent<SceneLayout>();
        PrepareScene();
        
        GetNodeByGuid("StartPoint",true);
        
        Next();
    }

    private void PrepareScene()
    {
        //Need to account for different scene comps
        _scene.backButton.GetComponent<Button>().onClick.AddListener(Back);
        _scene.nextButton.GetComponent<Button>().onClick.AddListener(() => { Next(); });
        _scene.skipButton.GetComponent<Button>().onClick.AddListener(Skip);
    }
}

public enum RunMode
{
    FedName,
    ContainerDialogue
}