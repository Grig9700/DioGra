using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntryNode : GraphNode
{
    public override NodeReturn Run(SceneLayout scene, DialogueManagerLegacy managerLegacy)
    {
        managerLegacy.SetTargetNode(children.First());
        return NodeReturn.Next;
    }

    public override void Clear(){}
}
