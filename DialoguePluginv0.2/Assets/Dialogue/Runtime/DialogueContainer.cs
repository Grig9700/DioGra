using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<GraphNodeData> GraphNodes = new List<GraphNodeData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<ExposedProperties> ExposedPropertiesList = new List<ExposedProperties>();
}
