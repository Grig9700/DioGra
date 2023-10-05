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
        return dialogueValues.ContainsKey(variableName) ? dialogueValues[variableName] : CreateVariable(FindAssets.GetResourceByName<VariableObject>(variableName));
    }
    
    public VariableObject GetVariable(VariableObject variable)
    {
        return dialogueValues.ContainsKey(variable.name) ? dialogueValues[variable.name] : CreateVariable(variable);
    }

    private VariableObject CreateVariable(VariableObject variable)
    {
        var obj = Instantiate(variable);
        obj.name = variable.name;
        obj.ResetToDefault();
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
