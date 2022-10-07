using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> DialogueNodes = new List<DialogueNodeData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<ExposedProperty<int>> ExposedPropertiesInt = new List<ExposedProperty<int>>();
    public List<ExposedProperty<float>> ExposedPropertiesFloat = new List<ExposedProperty<float>>();
    public List<ExposedProperty<string>> ExposedPropertiesString = new List<ExposedProperty<string>>();
    public List<ExposedProperty<bool>> ExposedPropertiesBool = new List<ExposedProperty<bool>>();
    //public List<ExposedProperty<>> ExposedProperties = new List<ExposedProperty<>>();
}
