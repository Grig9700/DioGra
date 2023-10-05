using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class VariableObject : ScriptableObject
{
    public UnityEvent valueChanged;
    public virtual void ResetToDefault(){}
    
    public virtual void SetValue(bool value, bool ignoreInvoke = false) {}
    public virtual void SetValue(float value, bool ignoreInvoke = false) {}
    public virtual void SetValue(int value, bool ignoreInvoke = false) {}
    public virtual void SetValue(string value, bool ignoreInvoke = false) {}
}
