using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExposedProperty<T>
{
    public string name = "New Property";
    public T value;
}
