using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtendedEditorWindow : EditorWindow
{
    //public SerializedObject serializedObject;
    //public SerializedProperty currentProperty;

    //public string selectedPropertyPath;
    //public SerializedProperty selectedProperty;

    public void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (!p.isExpanded) continue;
                
                EditorGUI.indentLevel++;
                DrawProperties(p, drawChildren);
                EditorGUI.indentLevel--;
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) {continue;}

                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }

    public SerializedProperty DrawSidebar(SerializedProperty prop, SerializedObject serializedObject)
    {
        string selectedPropertyPath = null; //added
        foreach (SerializedProperty p in prop)
        {
            if (GUILayout.Button(p.displayName))
            {
                selectedPropertyPath = p.propertyPath;
            }
        }

        if (!string.IsNullOrEmpty(selectedPropertyPath))
        {
            return serializedObject.FindProperty(selectedPropertyPath); //formerly selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }

        return null;
    }
    
    //added sO and sP to func rather than store internally
    public static void DrawField(string propertyName, bool relative, SerializedObject serializedObject = null, SerializedProperty currentProperty = null) 
    {
        if (relative && currentProperty != null)
        {
            EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propertyName), true);
        }
        else if (serializedObject != null)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), true);
        }
    }

    public void Apply(SerializedObject serializedObject)
    {
        serializedObject.ApplyModifiedProperties();
    }
}
