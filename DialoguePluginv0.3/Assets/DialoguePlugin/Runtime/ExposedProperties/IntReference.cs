using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntReference
{
    public bool UseConstant = true;
    public int ConstantValue;
    public IntVariable Variable;

    public int Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }
}
