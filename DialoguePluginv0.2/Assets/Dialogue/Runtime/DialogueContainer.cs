using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<GraphNodeData> GraphNodes = new List<GraphNodeData>();
    //public List<DialogueNodeData> DialogueNodes = new List<DialogueNodeData>();
    //public List<ChoiceNodeData> ChoiceNodes = new List<ChoiceNodeData>();
    //public List<IfNodeData> IfNodes = new List<IfNodeData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    //public List<ExposedProperty<int>> ExposedPropertiesInt = new List<ExposedProperty<int>>();
    //public List<ExposedProperty<float>> ExposedPropertiesFloat = new List<ExposedProperty<float>>();
    //public List<ExposedProperty<string>> ExposedPropertiesString = new List<ExposedProperty<string>>();
    //public List<ExposedProperty<bool>> ExposedPropertiesBool = new List<ExposedProperty<bool>>();

    public List<ExposedProperties> ExposedPropertiesList = new List<ExposedProperties>();
    //public List<ExposedProperty<>> ExposedProperties = new List<ExposedProperty<>>();
}
