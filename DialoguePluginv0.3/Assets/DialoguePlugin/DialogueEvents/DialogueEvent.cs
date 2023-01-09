using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueEvent : ScriptableObject
{
    private readonly List<DialogueEventListener> _listeners = new List<DialogueEventListener>();

    public void Raise()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(DialogueEventListener listener)
    {
        _listeners.Add(listener);
    }
    
    public void UnregisterListener(DialogueEventListener listener)
    {
        _listeners.Remove(listener);
    }
}