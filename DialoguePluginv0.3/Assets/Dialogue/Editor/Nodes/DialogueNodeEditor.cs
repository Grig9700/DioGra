using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(DialogueNode))]
public class DialogueNodeEditor : Editor
{
    
#if UNITY_EDITOR
    
    private DialogueNode _dialogueNode;
    private SerializedProperty _speaker;
    private SerializedProperty _dialogueText;
    
    private void Initialize()
    {
        _dialogueNode = (DialogueNode)target;
        _speaker = serializedObject.FindProperty("speaker");
        _dialogueText = serializedObject.FindProperty("dialogueText");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        Initialize();

        //Change in the future to popupfield with created actors
        EditorGUILayout.LabelField("Speaker");
        _dialogueNode.speaker = EditorGUI.TextField(new Rect(60, 0, 145, 20), _dialogueNode.speaker);
        //EditorGUILayout.PropertyField(_speaker);
        
        EditorGUILayout.LabelField("Dialogue Text");
        EditorGUILayout.PropertyField(_dialogueText, GUIContent.none);
        //_dialogueNode.dialogueText = EditorGUI.TextArea(new Rect(0, 0, 120, 120), _dialogueNode.dialogueText);
        
        serializedObject.ApplyModifiedProperties();
    }
    
#endif

}
