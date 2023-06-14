using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public DialogueContainer container;
    
    private SceneLayout _scene;
    private Stack<GraphNode> _previousNodes;

    private GraphNode _currentNode;
    private GraphNode _targetNode;

    private GameObject _canvas;
    private void OnEnable()
    {
        _previousNodes = new Stack<GraphNode>();
        _canvas = GameObject.Find("Canvas");
        
        if (_canvas == null)
            Debug.LogError("Missing Canvas");
        
        if (!GameObject.Find("EventSystem"))
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

    private void Back()
    {
        if (_previousNodes.Count <= 1)
            return;
        
        //Debug.Log($"[Dev] back button management options need implementing");
        
        _currentNode.Clear();
        
        _currentNode = _previousNodes.Pop();
        
        Next();
    }

    private void Next()
    {
        if (_currentNode == null)
        {
            EndDialogue();
            return;
        }

        switch (_currentNode.Run(_scene, this))
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
        GraphNode tempNode = getStartNode ? container.GraphNodes.First(node => node.GUID == getNodeByGUID) 
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

    private void Skip()
    {
        Debug.LogError($"[Dev] Skip not implemented");
    }
    
    public void EndDialogue(bool loadCall = false)
    {
        if (_scene != null)
            DestroyImmediate(_scene.gameObject);
        
        // if (!loadCall)
        //     Debug.Log($"Dialogue Ended");
    }

    public void LoadNewDialogue(string filename)
    {
        EndDialogue(true);

        DialogueContainer tempContainer = //FindAssets.GetInstanceByName<DialogueContainer>(filename).First();
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>($"{filename}.asset", "/Dialogues");

        if (tempContainer == null)
        {
            //Debug.Log($"[Dev] {filePath.First()}"); //Writes out the location of the file if it was found
            Debug.LogError($"[Designer][Writer] The dialogue named {filename} that you tried to load does not exist");
            return;
        }

        //Debug.Log($"{tempContainer.name}");
        container = tempContainer;

        //bool sceneExists = false;
        //GameObject obj = null;
        
        if (container.SceneLayoutPrefab == null)
        {
            Debug.LogError($"No scene layout present in Dialogue");
            //obj = FindAssets.GetInstanceByName<GameObject>("SceneLayout").First();
            //FindAndLoadResource.FindAndLoadFirstInResourceFolder<GameObject>("?cene*", "/SceneLayouts"); //finds a Scene Layout
            // if (obj != null)
            //     sceneExists = true;
            // else
            // {
            //     Debug.LogError($"No Scene Layouts were found");
            //     return;
            // }
        }
        
        _scene = //sceneExists ? Instantiate(obj, _canvas.transform).GetComponent<SceneLayout>() : 
            Instantiate(container.SceneLayoutPrefab, _canvas.transform).GetComponent<SceneLayout>();
        PrepareScene();
        
        GetNodeByGuid("StartPoint",true);
        
        Next();
    }

    private void PrepareScene()
    {
        //Need to account for different scene comps
        _scene.backButton.GetComponent<Button>().onClick.AddListener(Back);
        _scene.nextButton.GetComponent<Button>().onClick.AddListener(Next);
        _scene.skipButton.GetComponent<Button>().onClick.AddListener(Skip);

        if (container.defaultBackground != null)
            _scene.background = container.defaultBackground;
        else
            _scene.background.gameObject.SetActive(false);
        
        _scene.dialogueCharacter.gameObject.SetActive(false);
    }
}