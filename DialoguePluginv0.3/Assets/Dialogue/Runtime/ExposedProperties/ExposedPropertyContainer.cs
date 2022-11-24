using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExposedPropertyContainer : ScriptableObject
{
    public List<ExposedPropertyData> ExposedPropertyDatas = new List<ExposedPropertyData>();
}
