using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphUtility
{
    public static ScriptableObject CreateValue<T>(string valueName, ScriptableObject assetTarget)
    {
        var scriptableValue = ScriptableObject.CreateInstance<ScriptableValue<T>>();
        scriptableValue.name = valueName;

        AssetDatabase.AddObjectToAsset(scriptableValue, assetTarget);
        AssetDatabase.SaveAssets();
        
        EditorUtility.SetDirty(scriptableValue);
        EditorUtility.SetDirty(assetTarget);
           
        return scriptableValue;
    }
    
    public static void DestroyValue<T>(ScriptableValue<T> value)
    {
        AssetDatabase.RemoveObjectFromAsset(value);
        AssetDatabase.SaveAssets();
    }
    
    public void MakeInProgressGraph(string filename)
    {
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        //Creates folders if not present
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Dialogues/{filename}.asset");
        AssetDatabase.SaveAssets();
    }
    
    public static GraphNode CreateGraphNode(Type node, ScriptableObject assetTarget, string guid = null)
    {
        GraphNode graphNode = ScriptableObject.CreateInstance(node) as GraphNode;
        if (graphNode == null) return graphNode;
        
        graphNode.name = node.Name;
        graphNode.GUID = guid ?? GUID.Generate().ToString();

        AssetDatabase.AddObjectToAsset(graphNode, assetTarget);
        AssetDatabase.SaveAssets();

        EditorUtility.SetDirty(graphNode);
        EditorUtility.SetDirty(assetTarget);
        
        return graphNode;
    }
    
    public static void DestroyGraphNode(GraphNode node)
    {
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
}
