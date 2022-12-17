using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceNode : GraphNode
{
    public List<string> outputOptions = new List<string>();
    //public List<string> buttonsLeadToGUID = new List<string>();
}
