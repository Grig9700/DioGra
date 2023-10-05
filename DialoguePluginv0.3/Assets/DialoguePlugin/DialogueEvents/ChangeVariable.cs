using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVariable : MonoBehaviour
{
    public VariableObject target;
    
    public void ChangeValueBy(string value)
    {
        var container = FindObjectOfType<VariableContainer>();

        if (!container)
        {
            Debug.LogError("No VariableContainer found in scene. \n att the VariableContainer MonoBehaviour to an object in the scene so that variables can be tracked.");
        }
        
        switch (target)
        {
            case FloatVariable variable:
                if (float.TryParse(value, out var fVal))
                {
                    var varObj = container.GetVariable(variable) as FloatVariable;
                    
                    varObj.SetValue(varObj.Value + fVal);
                    return;
                }
                
                InvalidValueForThisVariable();
                break;
            
            case IntVariable variable:
                if (int.TryParse(value, out var iVal))
                {
                    var varObj = container.GetVariable(variable) as IntVariable;
                    
                    varObj.SetValue(varObj.Value + iVal);
                    return;
                }
                
                InvalidValueForThisVariable();
                break;
            
            default:
                Debug.LogError($"Wrong function for this type of variable. \n A {target} cannot be incremented");
                break;
        }
    }
    
    public void SetValueTo(string value)
    {
        var container = FindObjectOfType<VariableContainer>();

        if (!container)
        {
            Debug.LogError("No VariableContainer found in scene. \n att the VariableContainer MonoBehaviour to an object in the scene so that variables can be tracked.");
        }

        switch (target)
        {
            case BoolVariable variable:
                if (bool.TryParse(value, out var bVal))
                {
                    var varObj = container.GetVariable(variable);
                    
                    varObj.SetValue(bVal);
                    return;
                }
                
                InvalidValueForThisVariable();
                break;
            
            case FloatVariable variable:
                if (float.TryParse(value, out var fVal))
                {
                    var varObj = container.GetVariable(variable);
                    
                    varObj.SetValue(fVal);
                    return;
                }
                
                InvalidValueForThisVariable();
                break;
            
            case IntVariable variable:
                if (int.TryParse(value, out var iVal))
                {
                    var varObj = container.GetVariable(variable);
                    
                    varObj.SetValue(iVal);
                    return;
                }
                
                InvalidValueForThisVariable();
                break;
            
            case StringVariable variable:
                var stringObj = container.GetVariable(variable);
                stringObj.SetValue(value);
                break;
            
            default:
                Debug.LogError($"Wrong function for this type of variable. \n A {target} cannot be incremented");
                break;
        }
    }
    
    private void InvalidValueForThisVariable()
    {
        Debug.LogError($"{target} is not a valid variable for this change");
    }
}
