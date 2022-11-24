using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ScriptNodeData : GraphNodeData
{
    public UnityEvent scripts;

    private void InvokeScripts()
    {
        scripts.Invoke();
    }
}
