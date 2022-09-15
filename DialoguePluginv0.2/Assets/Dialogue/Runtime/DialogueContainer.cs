using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> DialogueNodes = new List<DialogueNodeData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
}
