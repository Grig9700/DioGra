using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
//[CreateAssetMenu(fileName = "New Dialogue", menuName = "New Dialogue")]
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
    
    public GraphNode CreateGraphNode(Type type, Vector2 pos)
    {
        GraphNode node = ScriptableObject.CreateInstance(type) as GraphNode;
        node.name = type.Name;
        node.GUID = GUID.Generate().ToString();
        node.position = pos;
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

    public void AddChild(GraphNode parent, GraphNode child)
    {
        parent.children.Add(child);
    }

    public void RemoveChild(GraphNode parent, GraphNode child)
    {
        parent.children.Remove(child);
    }

    public List<GraphNode> GetChildren(GraphNode parent)
    {
        return parent.children;
    }
#endif
}
