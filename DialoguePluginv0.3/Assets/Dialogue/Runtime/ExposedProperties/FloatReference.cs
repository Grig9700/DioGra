using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public FloatVariable Variable;

    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }
}
