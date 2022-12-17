using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScriptableValue<T> : ScriptableObject
{
    public T value;
}
