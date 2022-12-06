using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IfNode))]
public class IfNodeEditor : Editor
{
    private IfNode _ifNode;
    
    private void OnEnable()
    {
        _ifNode = (IfNode)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        //Write Code
        
        serializedObject.ApplyModifiedProperties();
    }
}
