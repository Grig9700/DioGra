using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueValueContainer : ScriptableObject, IPersistentDialogueData
{
    [SerializeField] private SerializableDictionary<string, VariableObject> dialogueValues;

    public VariableObject GetByName(string valueName)
    {
        foreach (var value in dialogueValues.Where(value => value.Value.name == valueName))
        {
            return value.Value;
        }

        Debug.LogError($"Could not find value by name : {valueName}");
        
        return null;
    }

    public VariableObject GetByID(string ID)
    {
        var temp = dialogueValues[ID];

        if (temp != null) 
            return temp;
        
        Debug.LogError($"Could not find value by ID : {ID}");
        
        return null;
    }

    #region Saving
    
    public void LoadData(DialogueGameData data)
    {
        dialogueValues = data.dialogueValues;
    }

    public void SaveData(ref DialogueGameData data)
    {
        data.dialogueValues = dialogueValues;

    }

    #endregion
}
