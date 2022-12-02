using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExposedProperty<T> : ExposedProperties
{
    public T value;
}
