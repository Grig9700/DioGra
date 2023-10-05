using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using UnityEditor;
using UnityEngine.Events;

[Serializable]
public class EventNode : GraphNode
{
    public List<DialogueEvent> invokedEvents;

    public override NodeReturn Run(DialogueManager manager)
    {
        if (invokedEvents.Count > 0)
            foreach (var dialogueEvent in invokedEvents)
                dialogueEvent.Raise();
        else
            Debug.LogError($"No events present in {this}", this);
        
        manager.SetTargetNode(children.First());
        
        return NodeReturn.Next;
    }

    public override void Clear()
    {
        
    }
}
