using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BackgroundNode))]
public class BackgroundNodeEditor : Editor
{
    private SerializedProperty _backgroundImage;
    private bool _firstCall = true;
    
    private void Initialize()
    {
        _backgroundImage = serializedObject.FindProperty("background");
        _firstCall = false;
    }
    
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if(_firstCall)
            Initialize();
        
        EditorGUILayout.PropertyField(_backgroundImage, GUIContent.none);
        
        serializedObject.ApplyModifiedProperties();
    }
}
