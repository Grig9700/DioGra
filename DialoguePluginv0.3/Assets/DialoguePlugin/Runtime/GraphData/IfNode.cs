using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IfNode : GraphNode
{
    public VariableObject comparisonTarget;
    public string comparisonValue;

    public string[] binaryComp = {"=", "!="};
    public int numTracker;
    public string[] numComp = {"=", "!=", ">", ">=", "<=", "<"};
    
    public override NodeReturn Run(DialogueManager manager)
    {
        if (IsNullOrEmpty())
            return NodeReturn.End;
        
        manager.SetTargetNode(RunComparison(manager) ? 
            childPortName[0] == "True" ? children[0] : children[1] : 
            childPortName[0] == "False" ? children[0] : children[1]);

        return NodeReturn.Next;
    }

    public override void Clear(){}

    public bool RunComparison(DialogueManager manager)
    {
        if (comparisonTarget != null) 
            return ComparisonOperation(numComp[numTracker], manager);
        
        Debug.LogError($"IfNode did not contain a comparison target");
        return false;

    }

    private bool ComparisonOperation(string operation, DialogueManager manager)
    {
        var val = manager.variableContainer.GetVariable(comparisonTarget);

        switch (operation)
        {
            case ">":
                return Greater(val);
                
            case ">=":
                return GreaterOrEqual(val);
                
            case "=":
                return Equal(val);
            
            case "<=":
                return LessOrEqual(val);
            
            case "<":
                return Less(val);
            
            case "!=":
                return NotEqual(val);
                
            default:
                Debug.LogError($"Invalid Comparison", this);
                return false;
        }
    }

    private bool Greater(VariableObject val)
    {
        switch (val)
        {
            case FloatVariable floatVariable:
                if (float.TryParse(comparisonValue, out var floatResult)) 
                    return floatVariable.Value > floatResult;
                        
                InvalidComparison();
                return false;

            case IntVariable intVariable:
                if (int.TryParse(comparisonValue, out var intResult)) 
                    return intVariable.Value > intResult;
                        
                InvalidComparison();
                return false;

            default:
                NotImplementedComparison();
                return false;
        }
    }
    
    private bool GreaterOrEqual(VariableObject val)
    {
        switch (val)
        {
            case FloatVariable floatVariable:
                if (float.TryParse(comparisonValue, out var floatResult)) 
                    return floatVariable.Value >= floatResult;
                        
                InvalidComparison();
                return false;
                    
            case IntVariable intVariable:
                if (int.TryParse(comparisonValue, out var intResult)) 
                    return intVariable.Value >= intResult;
                        
                InvalidComparison();
                return false;
                    
            default:
                NotImplementedComparison();
                return false;
        }
    }
    
    private bool Equal(VariableObject val)
    {
        switch (val)
        {
            case BoolVariable boolVariable:
                if (bool.TryParse(comparisonValue, out var boolResult)) 
                    return boolVariable.Value == boolResult;
                        
                InvalidComparison();
                return false;
                    
            case FloatVariable floatVariable:
                if (float.TryParse(comparisonValue, out var floatResult)) 
                    return Math.Abs(floatVariable.Value - floatResult) < 0.001f;
                        
                InvalidComparison();
                return false;
                    
            case IntVariable intVariable:
                if (int.TryParse(comparisonValue, out var intResult)) 
                    return intVariable.Value == intResult;
                        
                InvalidComparison();
                return false;
                    
            case StringVariable stringVariable:
                return stringVariable.Value == comparisonValue;
                    
            default:
                NotImplementedComparison();
                return false;
        }
    }
    
    private bool LessOrEqual(VariableObject val)
    {
        switch (val)
        {
            case FloatVariable floatVariable:
                if (float.TryParse(comparisonValue, out var floatResult)) 
                    return floatVariable.Value <= floatResult;
                        
                InvalidComparison();
                return false;
                    
            case IntVariable intVariable:
                if (int.TryParse(comparisonValue, out var intResult)) 
                    return intVariable.Value <= intResult;
                        
                InvalidComparison();
                return false;
                    
            default:
                NotImplementedComparison();
                return false;
        }
    }
    
    private bool Less(VariableObject val)
    {
        switch (val)
        {
            case FloatVariable floatVariable:
                if (float.TryParse(comparisonValue, out var floatResult)) 
                    return floatVariable.Value < floatResult;
                        
                InvalidComparison();
                return false;
                    
            case IntVariable intVariable:
                if (int.TryParse(comparisonValue, out var intResult)) 
                    return intVariable.Value < intResult;
                        
                InvalidComparison();
                return false;
                    
            default:
                NotImplementedComparison();
                return false;
        }
    }
    
    private bool NotEqual(VariableObject val)
    {
        switch (val)
        {
            case BoolVariable boolVariable:
                if (bool.TryParse(comparisonValue, out var boolResult)) 
                    return boolVariable.Value != boolResult;
                        
                InvalidComparison();
                return false;
                    
            case FloatVariable floatVariable:
                if (float.TryParse(comparisonValue, out var floatResult)) 
                    return Math.Abs(floatVariable.Value - floatResult) > 0.001f;
                        
                InvalidComparison();
                return false;
                    
            case IntVariable intVariable:
                if (int.TryParse(comparisonValue, out var intResult)) 
                    return intVariable.Value != intResult;
                        
                InvalidComparison();
                return false;
                    
            case StringVariable stringVariable:
                return stringVariable.Value != comparisonValue;
                    
            default:
                NotImplementedComparison();
                return false;
        }
    }

    private void InvalidComparison()
    {
        Debug.LogError($"{comparisonValue} is not a valid input for this comparison");
    }

    private void NotImplementedComparison()
    {
        Debug.LogError($"{comparisonTarget} might be an invalid type of comparison or simply one not implemented yet.");
    }
}