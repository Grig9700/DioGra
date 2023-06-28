using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntryNode : GraphNode
{
    public override NodeReturn Run(DialogueManager manager)
    {
        manager.SetTargetNode(children.First());
        return NodeReturn.Next;
    }
    public override void Clear(){}
}
