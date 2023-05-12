using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueValueContainer : MonoBehaviour, IPersistentDialogueData
{
    public SerializableDictionary<string, VariableObject> dialogueValues;

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
