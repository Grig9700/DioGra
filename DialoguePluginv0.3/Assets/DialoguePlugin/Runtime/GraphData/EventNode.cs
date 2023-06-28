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
    //public UnityEvent functionCalls;

    public List<DialogueEvent> invokedEvents;

    public override NodeReturn Run(DialogueManager manager)
    {
        //functionCalls.Invoke();
        foreach (var dialogueEvent in invokedEvents)
            dialogueEvent.Raise();
        
        manager.SetTargetNode(children.First());
        
        return NodeReturn.Wait;
    }

    public override void Clear()
    {
        
    }

    //public List<Action> Actions = new List<Action>();

    //private Action _scriptAction = () => { };

    //[HideInInspector]
    
    /*[HideInInspector]
    public List<MonoScript> calls = new List<MonoScript>();
    
    [HideInInspector] 
    public List<int> selectedMethod = new List<int>();
    
    [HideInInspector] 
    public List<string> methodNames = new List<string>();
    
    [HideInInspector]
    public bool expandCalls = true;
    [HideInInspector] 
    public List<List<ScriptableObject>> parameters = new List<List<ScriptableObject>>();*/

    /*public void CreateActions()
    {
        Actions.Clear();

        for (int i = 0; i < calls.Count; i++)
        {
            CreateAction(calls[i], selectedMethodInfos[i]);
        }
    }*/
    
    /*public void CreateActions(UnityEngine.Object obj) //, int i)
    {
        for (int i = 0; i < methodNames.Count; i++)
        {
            if (methodNames[i] == null || methodNames[i] == "")
                continue;
            
            MethodInfo methodInfo = calls[i].GetClass().GetMethod(methodNames[i],
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
            
            if (methodInfo == null) continue;
            
            _scriptAction += (Action)Delegate.CreateDelegate(typeof(Action), obj, methodInfo);
        }

        //var newAction = (Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo);
        
        //Actions.Add(newAction);

        //_scriptAction += newAction;
    }*/
    
    
    /*public void CreateAction(MonoScript script, MethodInfo methodInfo) //, int i)
    {
        System.Type type = script.GetClass();

        //MethodInfo methodInfo = selectedMethodInfos[i];
        
        if (methodInfo == null) return;
        _scriptAction += (Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo);
        
        //var newAction = (Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo);
        
        //Actions.Add(newAction);

        //_scriptAction += newAction;
    }*/

    /*public void CallActions()
    {
        _scriptAction();

        /*foreach (var action in Actions)
        {
            action();
        }#1#
    }*/
}
