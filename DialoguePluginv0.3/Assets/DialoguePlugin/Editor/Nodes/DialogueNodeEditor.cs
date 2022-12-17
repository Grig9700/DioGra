using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

[CustomEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : Editor
{
    
#if UNITY_EDITOR
    
    private DialogueNode _dialogueNode;
    private SerializedProperty _speaker;
    private SerializedProperty _dialogueText;
    private bool _firstCall = true;
    
    private void Initialize()
    {
        _dialogueNode = (DialogueNode)target;
        _speaker = serializedObject.FindProperty("speaker");
        _dialogueText = serializedObject.FindProperty("dialogueText");
        
        _firstCall = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if(_firstCall)
            Initialize();

        EditorGUILayout.PropertyField(_speaker, GUIContent.none);

        if (_dialogueNode.speaker != null)
        {
            string[] expressions = _dialogueNode.speaker.expressions
                .Where(expression => !String.IsNullOrEmpty(expression.emotion))
                .Select(expression => expression.emotion).ToArray();

            _dialogueNode.expressionSelector = EditorGUILayout.Popup(_dialogueNode.expressionSelector, expressions);
        }
        
        EditorGUILayout.LabelField("Dialogue Text");
        EditorGUILayout.PropertyField(_dialogueText, GUIContent.none);
        //_dialogueNode.dialogueText = EditorGUI.TextArea(new Rect(0, 0, 120, 120), _dialogueNode.dialogueText);
        
        serializedObject.ApplyModifiedProperties();
    }
    
#endif

}
