using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VariableContainer : MonoBehaviour, IPersistentDialogueData
{
    [SerializeField]
    private SerializableDictionary<string, VariableObject> dialogueValues;

    public VariableObject GetVariable(string variableName)
    {
        if (dialogueValues.ContainsKey(variableName))
            return dialogueValues[variableName];

        var obj = FindAssets.GetResourceByName<VariableObject>(variableName);
        dialogueValues.Add(obj.name, obj);
        return obj;
    }
    
    public VariableObject GetVariable(VariableObject variable)
    {
        if (dialogueValues.ContainsValue(variable))
            return dialogueValues[variable.name];

        var obj = Instantiate(variable);
        dialogueValues.Add(obj.name, obj);
        return obj;
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
