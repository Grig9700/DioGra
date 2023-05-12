using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDialogueDataManager : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] private bool newDataIfNull;
    
    public static PersistentDialogueDataManager Instance { get; private set; }

    private FileDataHandler _dataHandler;
    private DialogueGameData _dialogueGameData;
    private List<IPersistentDialogueData> _persistentDataObjects;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _dataHandler = new FileDataHandler();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private static List<IPersistentDialogueData> FindAllPersistentDataObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IPersistentDialogueData>().ToList(); //Has to be MonoBehavior to exist as an object in the scene
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _persistentDataObjects = FindAllPersistentDataObjects();
        
        if (_dialogueGameData == null && newDataIfNull)
            NewGame();
        
        if (_dialogueGameData == null)
            return;
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.LoadData(_dialogueGameData);
        }
    }
    
    private void OnSceneUnLoaded(Scene scene)
    {
        if (newDataIfNull && _dialogueGameData == null)
            NewGame();
        
        if (_dialogueGameData == null)
        {
            Debug.Log("No dialogue data to save to");
            return;
        }
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.SaveData(ref _dialogueGameData);
        }
    }
    
    public void NewGame()
    {
        _dialogueGameData = new DialogueGameData();

        var variables = FindAssets.GetAllInstances<VariableObject>();

        foreach (var variable in variables)
        {
            _dialogueGameData.dialogueValues.Add(variable.name, variable);
        }
        
        SaveGame("DialogueData");
    }

    public void LoadGame(string fileName)
    {
        if (newDataIfNull && _dialogueGameData == null)
            NewGame();
        
        _dialogueGameData = _dataHandler.Load(fileName);

        if (_dialogueGameData == null)
        {
            Debug.LogWarning($"No dialogue save data was found. Could not load data");
            return;
        }

        //SceneManager.LoadSceneAsync(_dialogueGameData.sceneName);
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.LoadData(_dialogueGameData);
        }
    }

    public void SaveGame(string fileName)
    {
        if (newDataIfNull && _dialogueGameData == null)
            NewGame();

        if (_dialogueGameData == null)
        {
            Debug.LogWarning($"No dialogue data container was found. Could not save data");
            return;
        }
        
        foreach (var persistentDataObject in _persistentDataObjects)
        {
            persistentDataObject.SaveData(ref _dialogueGameData);
        }

        _dataHandler.Save(_dialogueGameData, fileName);
    }

    public void DeleteGame(string fileName)
    {
        _dataHandler.Delete(fileName);
    }
    
    // private void OnApplicationQuit()
    // {
    //     //Auto-saving on quit?
    // }
}

[CustomEditor(typeof(PersistentDialogueDataManager))]
public class PersistentDialogueDataManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PersistentDialogueDataManager script = (PersistentDialogueDataManager)target;
        if (GUILayout.Button("Delete Save")) {
            script.DeleteGame("DialogueData");
        }
    }
}