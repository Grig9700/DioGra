using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> dialogueNodes = new List<DialogueNodeData>();
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
}
