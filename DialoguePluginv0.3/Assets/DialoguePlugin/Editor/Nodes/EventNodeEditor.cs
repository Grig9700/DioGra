using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;
#endif


[CustomEditor(typeof(EventNode))]
public class EventNodeEditor : Editor
{
    
#if UNITY_EDITOR
    
    private bool _firstCall = true;
    private SerializedProperty _invokedEvents;
    
    private void Initialize()
    {
        _invokedEvents = serializedObject.FindProperty("invokedEvents");

        _firstCall = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if(_firstCall)
            Initialize();
        
        EditorGUILayout.PropertyField(_invokedEvents);
        
        serializedObject.ApplyModifiedProperties();
    }
    #endif
    
}