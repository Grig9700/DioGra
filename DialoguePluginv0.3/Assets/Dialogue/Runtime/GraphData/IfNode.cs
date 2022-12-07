using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IfNode : GraphNode
{
    //public ExposedVariableType VariableType;
    //public ExposedProperties Property;
    //public bool numComparison;
    //public NumComparison numComparisonType;
    //public BinaryComparison binaryComparisonType;
    public VariableObject comparisonTarget;
    public string comparisonValue;

    public int binaryTracker;
    public string[] binaryComp = new []{"=", "!="};
    public int numTracker;
    public string[] numComp = new []{">", ">=", "=", "<=", "<", "!="};

    public bool RunComparison()
    {
        return true;
    }
}

public enum NumComparison
{
    GreaterThan,
    GreaterThanOrEqualTo,
    EqualTo,
    LessThanOrEqualTo,
    LessThan,
    NotEqualTo
}

public enum BinaryComparison
{
    EqualTo,
    NotEqualTo
}