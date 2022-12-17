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


[CustomEditor(typeof(ScriptNode))]
public class ScriptNodeEditor : Editor
{
    
#if UNITY_EDITOR
    
    //private SerializedProperty _calls;
    //private ReorderableList _reorderableList;
    //private ScriptNode _scriptNode;
    private bool _firstCall = true;
    private SerializedProperty _functionCalls;

    //private float _lineHeight;
    //private float _lineHeightSpace;

    private void Initialize()
    {
        /*_calls = serializedObject.FindProperty("calls");
        _scriptNode = (ScriptNode)target;
        _reorderableList = new ReorderableList(serializedObject, _calls, //serializedObject.FindProperty("calls"), 
            true, true, true, true);*/
        
        _functionCalls = serializedObject.FindProperty("functionCalls");

        _firstCall = false;
        //_lineHeight = EditorGUIUtility.singleLineHeight;
        //_lineHeightSpace = _lineHeight + 10;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if(_firstCall)
            Initialize();
        
        //DrawDefaultInspector();
        
        //ScriptNodeCalls scriptNode = (ScriptNodeCalls)target;
        // if (scriptNode.calls is not { Count: > 0 })
        // {
        //     return;
        // }
        
        
        /*EditorGUILayout.BeginHorizontal();
        GUILayout.Space(200);
        //EditorGUILayout.PropertyField(_calls.FindPropertyRelative("Array.size"));
        EditorGUILayout.EndHorizontal();
        
        _scriptNode.expandCalls = EditorGUI.BeginFoldoutHeaderGroup(new Rect(0, 0, 200, 20), _scriptNode.expandCalls, "Calls");
        //_scriptNode.expandCalls = EditorGUILayout.Foldout(_scriptNode.expandCalls, "Calls", true);
        //EditorGUILayout.PropertyField(_calls.FindPropertyRelative("Array.size"));
        //EditorGUI.indentLevel++;
        
        GUILayout.Space(20);
        if (_scriptNode.expandCalls)
        {
            
            //Show(_calls, ScriptNodeOptions.NoElementLabels | ScriptNodeOptions.Buttons);
            
            ReorderShow(_calls,ScriptNodeOptions.NoElementLabels | ScriptNodeOptions.Buttons);
        }
        //EditorGUI.indentLevel--;
        EditorGUI.EndFoldoutHeaderGroup();*/
        
        
        EditorGUILayout.PropertyField(_functionCalls);
        
        
        serializedObject.ApplyModifiedProperties();
    }

    /*private void ReorderShow(SerializedProperty list, ScriptNodeOptions options = ScriptNodeOptions.Default)
    {
        _reorderableList.DoLayoutList();

        _reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 160, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
            

            if (_scriptNode.selectedMethod.Count <= index + 1)
            {
                _scriptNode.selectedMethod.Add(0);
                _scriptNode.parameters.Add(new List<ScriptableObject>());
            }

            if (_scriptNode.methodNames.Count <= index + 1)
            {
                _scriptNode.methodNames.Add(null);
            }

            MonoScript script = _scriptNode.calls[index];
            
            if (script == null)
                return;

            Type type = script.GetClass();

            if (type == null)
            {
                Debug.LogError($"Invalid script selected {script.name}");
                return;
            }

            MethodInfo[] scriptMethods =
                type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);

            if (scriptMethods is not { Length: > 0 })
            {
                Debug.LogWarning($"Script does not contain any methods {script.name}");
                return;
            }
            
            List<string> methodNames = scriptMethods.Select(method => method.Name).ToList();
            methodNames.Insert(0, "");
            
            _scriptNode.selectedMethod[index] = EditorGUI.Popup(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, 
                160, EditorGUIUtility.singleLineHeight), _scriptNode.selectedMethod[index],methodNames.ToArray());

            if (_scriptNode.selectedMethod[index] == 0)
                _scriptNode.methodNames[index] = null;
            else
                //_scriptNode.methodNames[index] = scriptMethods[_scriptNode.selectedMethod[index] - 1];
                _scriptNode.methodNames[index] = methodNames[_scriptNode.selectedMethod[index]];
            
            Debug.Log($"{_scriptNode.methodNames[index]}");
            
            
            if (_scriptNode.selectedMethod[index] == 0)
                return;
            //_scriptNode.CreateAction(script, scriptMethods[_scriptNode.selectedMethod[index] - 1]);
            
            
            
            
            /*ParameterInfo[] pars = type.GetMethod(_scriptNode.methodName)?.GetParameters();
            
            if (pars is not { Length: > 0 })
            {
                //Debug.LogWarning($"Method does not contain any parameters {script.name}");
                return;
            }

            if (pars.Any(parameter => !parameter.ParameterType.IsValueType))
            {
                Debug.LogError($"{_scriptNode.methodName} contains parameters that are currently not supported. " +
                               $"\n    Supported Types include; Bool, Int, String, Float. ScriptableObjects will be added at a later date." +
                               $"\n    You can also use the dollar sign (\"$\") before the name of the Blackboard property to indicate " +
                               $"that you wish to send the named property instead of another value into the script call");
                //return;
            }
            
            EditorGUI.indentLevel++;

            List<ScriptableObject> values = new List<ScriptableObject>();
            foreach (var parameter in pars)
            {
                string paramType = parameter.ParameterType.ToString().Split('.').Last();

                switch (paramType)
                {
                    case "Boolean":
                        values.Add(GraphUtility.CreateValue<bool>(parameter.Name, _scriptNode));
                        break;
                    case "Single":
                        values.Add(GraphUtility.CreateValue<float>(parameter.Name, _scriptNode));
                        break;
                    case "Int*":
                        values.Add(GraphUtility.CreateValue<int>(parameter.Name, _scriptNode));
                        break;
                    case "String":
                        values.Add(GraphUtility.CreateValue<string>(parameter.Name, _scriptNode));
                        break;
                }
            }
            
            
            for (int i = 0; i < values.Count; i++)
            {
                //SerializedProperty methodProperty = values[i];

                string methodProperty = EditorGUILayout.TextArea(values[i].name);

                values[i].name = methodProperty;

                // EditorGUI.PropertyField(
                //     new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * (i + 2), 160, EditorGUIUtility.singleLineHeight),
                //     methodProperty, GUIContent.none);
            }
            
            
            EditorGUI.indentLevel--;
            
            _scriptNode.parameters[index] = values;#1#
        };

        _reorderableList.elementHeightCallback = index =>
        {
            float height = EditorGUIUtility.singleLineHeight * 2;

            if (_scriptNode.parameters is not { Count: > 0 })
                return height;

            // var temp = _scriptNode.parameters[index];
            //
            // if (temp is { Count: > 0 }) 
            //     height += EditorGUIUtility.singleLineHeight * _scriptNode.parameters[index].Count;
            return height;
        };
    }*/
    
    /*private void Show(SerializedProperty list, ScriptNodeOptions options = ScriptNodeOptions.Default)
    {
        bool
            showListLabel = (options & ScriptNodeOptions.ListLabel) != 0,
            showListSize = (options & ScriptNodeOptions.ListSize) != 0;
        
        //EditorGUILayout.PropertyField(list);
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
    }*/

    /*private readonly GUIContent
        _moveButtonContentUp = new GUIContent("\u2191", "move up"),
        _moveButtonContentDown = new GUIContent("\u2193", "move down"),
        _duplicateButtonContent = new GUIContent("+", "duplicate"),
        _deleteButtonContent = new GUIContent("-", "delete");

    private readonly GUILayoutOption _miniButtonWidth = GUILayout.Width(20f);*/

    /*private void ShowElements(SerializedProperty list, ScriptNodeOptions options)
    {
        bool 
            showElementLabels = (options & ScriptNodeOptions.ElementLabels) != 0,
            showButtons = (options & ScriptNodeOptions.Buttons) != 0;
        int i;
        for (i = 0; i < list.arraySize; i++)
        {
            if (showButtons)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(_moveButtonContentUp, EditorStyles.miniButtonLeft, _miniButtonWidth))
                {
                    list.MoveArrayElement(i, i - 1);
                    Repaint();
                }
                if (GUILayout.Button(_moveButtonContentDown, EditorStyles.miniButtonRight, _miniButtonWidth))
                {
                    list.MoveArrayElement(i, i + 1);
                    Repaint();
                }
            }
            
            if (showElementLabels)
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            else
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
            
            if (showButtons)
                EditorGUILayout.EndHorizontal();
            
            if (_scriptNode.selectedMethod.Count <= i + 1)
                _scriptNode.selectedMethod.Add(0);

            MonoScript script = _scriptNode.calls[i];
            
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

            if (_scriptNode.calls is not { Count: > 0 })
            {
                Debug.LogWarning($"Script does not contain any methods {script.name}");
                continue;
            }
            
            List<string> methodNames = scriptMethods.Select(method => method.Name).ToList();
            
            _scriptNode.selectedMethod[i] = EditorGUILayout.Popup(_scriptNode.selectedMethod[i],methodNames.ToArray());
        }
        
        if (showButtons)
            ShowButtons(list, i);
    }*/

    /*private void ShowButtons(SerializedProperty list, int index)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(200);
        if (GUILayout.Button(_duplicateButtonContent, EditorStyles.miniButtonLeft, _miniButtonWidth))
        {
            list.InsertArrayElementAtIndex(index);
        }
        if (GUILayout.Button(_deleteButtonContent, EditorStyles.miniButtonRight, _miniButtonWidth))
        {
            list.DeleteArrayElementAtIndex(index);
        }
        EditorGUILayout.EndHorizontal();
    }*/
    
    #endif
    
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