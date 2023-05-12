using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueGameData
{
    public SerializableDictionary<string, VariableObject> dialogueValues;
    
    public DialogueGameData()
    {
        dialogueValues = new SerializableDictionary<string, VariableObject>();
    }
}
