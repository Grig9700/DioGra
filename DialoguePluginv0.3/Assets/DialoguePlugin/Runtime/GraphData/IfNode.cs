using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IfNode : GraphNode
{
    public VariableObject comparisonTarget;
    public string comparisonValue;

    public int binaryTracker;
    public string[] binaryComp = {"=", "!="};
    public int numTracker;
    public string[] numComp = {">", ">=", "=", "<=", "<", "!="};
    
    public override NodeReturn Run(DialogueManager manager)
    {
        if (IsNullOrEmpty())
            return NodeReturn.End;
        
        manager.SetTargetNode(RunComparison() ? 
            childPortName[0] == "True" ? children[0] : children[1] : 
            childPortName[0] == "False" ? children[0] : children[1]);

        return NodeReturn.Next;
    }

    public override void Clear(){}

    public bool RunComparison()
    {
        if (comparisonTarget == null)
        {
            Debug.LogError($"IfNode did not contain a comparison target");
            return false;
        }

        switch (comparisonTarget)
        {
            case BoolVariable boolVariable:
            case StringVariable stringVariable:
                return ComparisonOperation(binaryComp[binaryTracker]);
            
            case FloatVariable floatVariable:
            case IntVariable intVariable:
                return ComparisonOperation(numComp[numTracker]);
            
            default:
                Debug.LogError($"Invalid comparison target");
                return false;
        }
    }

    private bool ComparisonOperation(string operation)
    {
        float floatResult;
        int intResult;
        bool boolResult;
        
        switch (operation)
        {
            case ">":
                switch (comparisonTarget)
                {
                    case FloatVariable floatVariable:
                        if (float.TryParse(comparisonValue, out floatResult)) 
                            return floatVariable.Value > floatResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;

                    case IntVariable intVariable:
                        if (int.TryParse(comparisonValue, out intResult)) 
                            return intVariable.Value > intResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;

                    default:
                        Debug.LogError($"{comparisonTarget} is not part of this update and could not be read");
                        return false;
                }
                
            case ">=":
                switch (comparisonTarget)
                {
                    case FloatVariable floatVariable:
                        if (float.TryParse(comparisonValue, out floatResult)) 
                            return floatVariable.Value >= floatResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case IntVariable intVariable:
                        if (int.TryParse(comparisonValue, out intResult)) 
                            return intVariable.Value >= intResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    default:
                        Debug.LogError($"{comparisonTarget} is not part of this update and could not be read");
                        return false;
                }
            
            case "=":
                switch (comparisonTarget)
                {
                    case BoolVariable boolVariable:
                        if (bool.TryParse(comparisonValue, out boolResult)) 
                            return boolVariable.Value == boolResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case FloatVariable floatVariable:
                        if (float.TryParse(comparisonValue, out floatResult)) 
                            return Math.Abs(floatVariable.Value - floatResult) < 0.001f;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case IntVariable intVariable:
                        if (int.TryParse(comparisonValue, out intResult)) 
                            return intVariable.Value == intResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case StringVariable stringVariable:
                        return stringVariable.Value == comparisonValue;
                    
                    default:
                        Debug.LogError($"{comparisonTarget} is not part of this update and could not be read");
                        return false;
                }
            
            case "<=":
                switch (comparisonTarget)
                {
                    case FloatVariable floatVariable:
                        if (float.TryParse(comparisonValue, out floatResult)) 
                            return floatVariable.Value <= floatResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case IntVariable intVariable:
                        if (int.TryParse(comparisonValue, out intResult)) 
                            return intVariable.Value <= intResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    default:
                        Debug.LogError($"{comparisonTarget} is not part of this update and could not be read");
                        return false;
                }
            
            case "<":
                switch (comparisonTarget)
                {
                    case FloatVariable floatVariable:
                        if (float.TryParse(comparisonValue, out floatResult)) 
                            return floatVariable.Value < floatResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case IntVariable intVariable:
                        if (int.TryParse(comparisonValue, out intResult)) 
                            return intVariable.Value < intResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    default:
                        Debug.LogError($"{comparisonTarget} is not part of this update and could not be read");
                        return false;
                }
            
            case "!=":
                switch (comparisonTarget)
                {
                    case BoolVariable boolVariable:
                        if (bool.TryParse(comparisonValue, out boolResult)) 
                            return boolVariable.Value != boolResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case FloatVariable floatVariable:
                        if (float.TryParse(comparisonValue, out floatResult)) 
                            return Math.Abs(floatVariable.Value - floatResult) > 0.001f;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case IntVariable intVariable:
                        if (int.TryParse(comparisonValue, out intResult)) 
                            return intVariable.Value != intResult;
                        
                        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
                        return false;
                    
                    case StringVariable stringVariable:
                        return stringVariable.Value != comparisonValue;
                    
                    default:
                        Debug.LogError($"{comparisonTarget} is not part of this update and could not be read");
                        return false;
                }
                
            default:
                Debug.LogError($"Invalid Comparison", this);
                return false;
        }
    }
}