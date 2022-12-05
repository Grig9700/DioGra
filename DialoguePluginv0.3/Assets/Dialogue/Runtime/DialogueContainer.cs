using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<GraphNodeData> GraphNodes = new List<GraphNodeData>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public GameObject SceneLayoutPrefab;

#if UNITY_EDITOR
    public GraphNodeData CreateEntryGraphNode()
    {
        GraphNodeData node = ScriptableObject.CreateInstance<EntryNodeData>();
        node.name = "Start Point";
        node.GUID = "StartPoint";
        node.entryNode = true;
        GraphNodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }
    
    public GraphNodeData CreateGraphNode(Type type)
    {
        GraphNodeData node = ScriptableObject.CreateInstance(type) as GraphNodeData;
        node.name = type.Name;
        node.GUID = GUID.Generate().ToString();
        GraphNodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteGraphNode(GraphNodeData node)
    {
        GraphNodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
#endif
}
