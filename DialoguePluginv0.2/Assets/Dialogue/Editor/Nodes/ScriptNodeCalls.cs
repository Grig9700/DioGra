using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScriptNodeCalls : ScriptableObject
{
    public List<MonoScript> calls;
    
    [HideInInspector] 
    public List<int> selectedMethod = new List<int>();


    // public UnityAction actionScripts;
    //
    // public UnityEvent eventScripts;
    //
    // public void InvokeScripts()
    // {
    //     eventScripts.Invoke();
    // }
}

[CustomEditor(typeof(ScriptNodeCalls))]
public class ScriptNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ScriptNodeCalls scriptNode = (ScriptNodeCalls)target;
        if (scriptNode.calls is not { Count: > 0 })
        {
            return;
        }
        
        
        
        //Show(scriptNode.calls);

        
        
        if (scriptNode.selectedMethod.Count < scriptNode.calls.Count)
        {
            for (int j = 0; j < scriptNode.calls.Count - scriptNode.selectedMethod.Count; j++)
            {
                scriptNode.selectedMethod.Add(0);
            }
        }

        int i = 0;
        foreach (var script in scriptNode.calls)
        {
            if (script == null)
                continue;

            Type type = script.GetClass();

            if (type == null)
            {
                Debug.LogError($"Invalid script selected {script.name}");
                continue;
            }

            MethodInfo[] scriptMethods =
                type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (scriptNode.calls is not { Count: > 0 })
            {
                Debug.LogWarning($"Script does not contain any methods {script.name}");
                continue;
            }
            
            List<string> methodNames = scriptMethods.Select(method => method.Name).ToList();
            
            scriptNode.selectedMethod[i] = EditorGUILayout.Popup(scriptNode.selectedMethod[i],methodNames.ToArray());
            
            
            
            // foreach (var method in scriptMethods)
            // {
            //     Debug.Log($"{method.Name}");
            // }

            i++;
        }
        
        
        serializedObject.Update();
        GUI.enabled = false;
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();
        
        //MonoScript script = null; 
        //script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;
    }

    private static void Show(SerializedProperty list, ScriptNodeOptions options = ScriptNodeOptions.Default)
    {
        bool
            showListLabel = (options & ScriptNodeOptions.ListLabel) != 0,
            showListSize = (options & ScriptNodeOptions.ListSize) != 0;
        
        EditorGUILayout.PropertyField(list);
        if (showListLabel)
            EditorGUI.indentLevel++;
        if (list.isExpanded)
        {
            if (showListSize)
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            
            ShowElements(list, options);
        }
        if (showListLabel)
            EditorGUI.indentLevel--;
    }

    private static readonly GUIContent
        MoveButtonContent = new GUIContent("\u21b4", "move down"),
        DuplicateButtonContent = new GUIContent("+", "duplicate"),
        DeleteButtonContent = new GUIContent("-", "delete");

    private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20f);

    private static void ShowElements(SerializedProperty list, ScriptNodeOptions options)
    {
        bool 
            showElementLabels = (options & ScriptNodeOptions.ElementLabels) != 0,
            showButtons = (options & ScriptNodeOptions.Buttons) != 0;
        
        for (int i = 0; i < list.arraySize; i++)
        {
            if (showButtons)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Button(MoveButtonContent, EditorStyles.miniButtonLeft, MiniButtonWidth);
            }
            
            if (showElementLabels)
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            else
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
            
            if (showButtons)
                EditorGUILayout.EndHorizontal();
        }
        
        if (showButtons)
            ShowButtons();
    }

    private static void ShowButtons()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Button(DuplicateButtonContent, EditorStyles.miniButtonLeft, MiniButtonWidth);
        GUILayout.Button(DeleteButtonContent, EditorStyles.miniButtonRight, MiniButtonWidth);
        EditorGUILayout.EndHorizontal();
    }
}

[Flags]
public enum ScriptNodeOptions
{
    None = 0,
    ListSize = 1,
    ListLabel = 2,
    ElementLabels = 4,
    Buttons = 8,
    Default = ListSize | ListLabel | ElementLabels,
    NoElementLabels = ListSize | ListLabel,
    All = Default | Buttons
}
