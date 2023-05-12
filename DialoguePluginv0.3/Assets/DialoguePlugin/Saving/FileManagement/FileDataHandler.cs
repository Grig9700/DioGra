using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileDataHandler
{
    private readonly string _dataDirectoryPath;
    private readonly bool _useEncryption;

    public FileDataHandler(bool useEncryption = false)
    {
        _dataDirectoryPath = Application.persistentDataPath;
        _useEncryption = useEncryption;
    }
    
    public DialogueGameData Load(string fileName) => LoadFromFile.Load(fileName, _dataDirectoryPath, _useEncryption);
    
    public void Save(DialogueGameData dialogueGameData, string fileName) => SaveToFile.Save(dialogueGameData, fileName, _dataDirectoryPath, _useEncryption);

    public void Delete(string fileName) => DeleteSaveFile.Delete(fileName, _dataDirectoryPath);
}
