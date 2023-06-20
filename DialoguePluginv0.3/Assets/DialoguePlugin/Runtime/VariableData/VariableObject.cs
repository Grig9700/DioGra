using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class VariableObject : ScriptableObject
{
    public UnityEvent valueChanged;
}
