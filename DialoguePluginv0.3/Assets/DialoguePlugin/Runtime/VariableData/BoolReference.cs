using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolReference
{
    public bool UseConstant = true;
    public bool ConstantValue;
    public BoolVariable Variable;

    public bool Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }
}
