using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IfNode : GraphNode
{
    public ExposedVariableType VariableType;
    public ExposedProperties Property;
}
