using System;using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public RunMode runMode;
    public DialogueContainer container;

    private List<GameObject> buttons;
    private SceneLayout _scene;
    private string _GUID;

    private void OnEnable()
    {
        buttons = new List<GameObject>();
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadNewDialogue("bot");
        }
    }
    
    public void Back()
    {
        
    }
    
    public void Next()
    {
        
    }

    public void Skip()
    {
        
    }
    
    private void ClearButtons()
    {
        
    }
    
    private void EndDialogue()
    {
        
    }

    public void LoadNewDialogue(string filename)
    {
        foreach (var obj in buttons)
        {
            DestroyImmediate(obj);
        }
        buttons.Clear();

        string toResource = Application.dataPath + "/Resources/";
        
        var files = Directory.GetFiles(toResource + "Dialogues", $"{filename}.asset", SearchOption.AllDirectories);
        
        if (files == null || files.Length <= 0)
        {
            Debug.LogError($"No dialogue containers were found");
            return;
        }

        string fullFilePath = files.First().Replace('\\', '/');
        string filePathoid = fullFilePath.Remove(0, toResource.Length);
        string[] filePath = filePathoid.Split('.');

        DialogueContainer tempContainer = Resources.Load<DialogueContainer>($"{filePath.First()}");

        if (tempContainer == null)
        {
            Debug.Log($"{filePath.First()}");
            Debug.LogError($"The dialogue you tried to load does not exist");
            return;
        }

        Debug.Log($"{tempContainer.name}");
        container = tempContainer;
    }
}

public enum RunMode
{
    FedName,
    ContainerDialogue
}