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
    private SerializedProperty _variableTarget;
    private SerializedProperty _comparisonValue;
    private bool _firstCall = true;
    
    private void Initialize()
    {
        _ifNode = (IfNode)target;
        _variableTarget = serializedObject.FindProperty("comparisonTarget");
        _comparisonValue = serializedObject.FindProperty("comparisonValue");
        
        _firstCall = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if(_firstCall)
            Initialize();

        EditorGUI.PropertyField(new Rect(0, 0, 60, 20), _variableTarget, GUIContent.none);

        if (_ifNode.comparisonTarget != null)
        {
            switch (_ifNode.comparisonTarget)
            {
                case BoolVariable boolVariable:
                case StringVariable stringVariable:
                    _ifNode.binaryTracker = EditorGUI.Popup(new Rect(60, 0, 40, 20), _ifNode.binaryTracker, _ifNode.binaryComp);
                    break;
                case FloatVariable floatVariable:
                case IntVariable intVariable:
                    _ifNode.numTracker = EditorGUI.Popup(new Rect(60, 0, 40, 20), _ifNode.numTracker, _ifNode.numComp);
                    break;
                default:
                    Debug.LogError($"Invalid comparison target");
                    break;
            }
        }
        
        GUILayout.Space(40);
        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        GUILayout.EndHorizontal();
        
        if (_ifNode.comparisonTarget != null)
            EditorGUI.PropertyField(new Rect(0, 20, 100, 20), _comparisonValue, GUIContent.none);
        
        serializedObject.ApplyModifiedProperties();
    }
    
#endif

}
