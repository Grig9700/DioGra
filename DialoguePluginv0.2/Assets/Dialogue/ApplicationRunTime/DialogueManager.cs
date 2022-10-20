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
    private string _currentNode;
    private Stack<string> _previousNodes;

    private GameObject _canvas;
    private void OnEnable()
    {
        _buttons = new List<GameObject>();
        _previousNodes = new Stack<string>();
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
        _currentNode = _previousNodes.Pop();
        Next(true);
    }
    
    public void Next(bool alreadyPulled = false)
    {
        if (_buttons.Count > 0)
            return;
        
        if (!alreadyPulled)
            GetNext();

        var node = container.GraphNodes.First(x => x.GUID == _currentNode);
        
        switch (node.NodeType)
        {
            case NodeType.DialogueNode:
                _scene.nameField.text = node.speaker;
                _scene.textField.text = node.dialogueText;
                break;
            case NodeType.ChoiceNode:
                var buttonsToMake = container.NodeLinks.Where(x => x.baseNodeGUID == _currentNode).ToList();
                var height = _scene.buttonPrefab.GetComponent<RectTransform>().rect.height;
                for (int i = 0; i < buttonsToMake.Count(); i++)
                {
                    int con = i;
                    var obj = Instantiate(_scene.buttonPrefab, _scene.viewPort.transform);
                    obj.GetComponent<RectTransform>().transform.localPosition = new Vector2(91.5f, -100f + i * -height);
                    obj.GetComponent<Button>().onClick.AddListener(() => { Button(buttonsToMake[con].targetNodeGUID);});
                    obj.GetComponentInChildren<Text>().text = buttonsToMake[i].portName;
                    _buttons.Add(obj);
                }
                break;
            case NodeType.IfNode:
                break;
            default:
                Debug.LogError("Entered Default on Next", this);
                break;
        }
    }

    private void GetNext()
    {
        var temp = container.NodeLinks.Where(x => x.baseNodeGUID == _currentNode).ToList();
        if (temp.Count <= 0)
            EndDialogue();
        _previousNodes.Push(_currentNode);
        _currentNode = temp.First().targetNodeGUID;
    }
    
    public void Button(string targetGUID)
    {
        _previousNodes.Push(_currentNode);
        _currentNode = targetGUID;
        ClearButtons();
        Next(true);
    }
    
    public void Skip()
    {
        
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
        
        if (!loadCall)
            Debug.Log($"Dialogue Ended");
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

        Debug.Log($"{tempContainer.name}");
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
        _currentNode = "StartPoint";
        
        Next();
    }
}

public enum RunMode
{
    FedName,
    ContainerDialogue
}