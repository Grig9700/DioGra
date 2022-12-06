using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CustomEditor(typeof(IfNode))]
public class IfNodeEditor : Editor
{
    
#if UNITY_EDITOR
    
    private IfNode _ifNode;
    private bool _firstCall = true;
    
    private void Initialize()
    {
        _ifNode = (IfNode)target;
        
        _firstCall = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if(_firstCall)
            Initialize();
        
        //Write Code
        
        serializedObject.ApplyModifiedProperties();
    }
    
#endif

}
