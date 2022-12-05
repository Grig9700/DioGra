using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<GraphNode> GraphNodes = new List<GraphNode>();
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public GameObject SceneLayoutPrefab;

#if UNITY_EDITOR
    public GraphNode CreateEntryGraphNode()
    {
        GraphNode node = ScriptableObject.CreateInstance<EntryNode>();
        node.name = "Start Point";
        node.GUID = "StartPoint";
        node.entryNode = true;
        GraphNodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }
    
    public GraphNode CreateGraphNode(Type type)
    {
        GraphNode node = ScriptableObject.CreateInstance(type) as GraphNode;
        node.name = type.Name;
        node.GUID = GUID.Generate().ToString();
        GraphNodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteGraphNode(GraphNode node)
    {
        GraphNodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
#endif
}
