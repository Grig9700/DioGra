using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework.Internal.Execution;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;
    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<GraphNode> GraphNodes => _targetGraphView.nodes.ToList().Cast<GraphNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility()
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveData(string filename)
    {
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        if (!SaveGraphData(dialogueContainer)) return;
        SaveExposedProperties(dialogueContainer);

        //Creates folders if not present
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
        
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Dialogues/{filename}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        dialogueContainer.ExposedPropertiesList.AddRange(_targetGraphView.ExposedPropertiesList);
    }

    private bool SaveGraphData(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any()) return false; //No edges means no graph

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        foreach (Edge port in connectedPorts)
        {
            var inputNode = port.input.node as GraphNode;
            var outputNode = port.output.node as GraphNode;
            
            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                baseNodeGUID = outputNode.GUID,
                portName = port.output.portName,
                targetNodeGUID = inputNode.GUID
            });
        }
        
        foreach (var graphNode in GraphNodes.Where(node => !node.entryPoint))
        {
            switch (graphNode)
            {
                case DialogueNode dialogueNode:
                    dialogueContainer.GraphNodes.Add(new DialogueNodeData
                    {
                        nodeName = dialogueNode.title,
                        GUID = dialogueNode.GUID,
                        speaker = dialogueNode.speaker,
                        dialogueText = dialogueNode.dialogueText,
                        position = dialogueNode.GetPosition().position
                    });
                    // Debug.Log("dialogue node");
                    // var bop = dialogueContainer.GraphNodes[0];
                    // switch (bop)
                    // {
                    //     case DialogueNodeData dat:
                    //         Debug.Log("Confusion");
                    //         Debug.Log($"{dat.speaker}");
                    //         Debug.Log($"{dat.dialogueText}");
                    //         Debug.Log($"{dat.position}");
                    //         Debug.Log($"{dat.nodeName}");
                    //         Debug.Log($"{dat.GUID}");
                    //         Debug.Log($"{dat}");
                    //         break;
                    // }
                    break;
                case ChoiceNode choiceNode:
                    dialogueContainer.GraphNodes.Add(new ChoiceNodeData
                    {
                        nodeName = choiceNode.title,
                        GUID = choiceNode.GUID,
                        position = choiceNode.GetPosition().position
                    });
                    //Debug.Log("choice node");
                    break;
                case IfNode ifNode:
                    dialogueContainer.GraphNodes.Add(new IfNodeData
                    {
                        nodeName = ifNode.title,
                        GUID = ifNode.GUID,
                        position = ifNode.GetPosition().position
                    });
                    //Debug.Log("if node");
                    break;
                default:
                    Debug.LogError("Entered default");
                    break;
            }
            
        }
        return true;
    }
    
    public void LoadData(string filename, DialogueGraph graph)
    {
        string toResource = Application.dataPath + "/Resources/";
        
        var files = Directory.GetFiles(toResource + "Dialogues", $"{filename}.asset", SearchOption.AllDirectories);
        
        if (files.Length <= 0)
        {
            Debug.LogError($"No dialogue containers were found");
            return;
        }

        string fullFilePath = files.First().Replace('\\', '/');
        string filePathoid = fullFilePath.Remove(0, toResource.Length);
        string[] filePath = filePathoid.Split('.');
        
        _containerCache = Resources.Load<DialogueContainer>($"{filePath.First()}");
        if (_containerCache == null)
        {
            Debug.LogError($"{filename} is not present. Please check that the filename is correct");
            return;
        }

        //Creates a backup in case Load was pushed without having saved.
        //This file will be overridden each time load or clear is clicked
        CreateGraphBackup();
        
        ClearGraph();
        CreateNodes();
        
        ConnectNodes();
        CreateExposedVariables(graph);
    }

    public void Clear()
    {
        CreateGraphBackup();
        ClearGraph();
    }
    
    private void CreateExposedVariables(DialogueGraph graph)
    {
        _targetGraphView.ClearBlackboardAndExposedProperties();
        graph.CreateBlackBoardElements();
        foreach (var property in _containerCache.ExposedPropertiesList)
        {
            switch (property)
            {
                case ExposedProperty<bool> boolProperty:
                    _targetGraphView.AddPropertyToBlackboard(boolProperty);
                    break;
                case ExposedProperty<float> floatProperty:
                    _targetGraphView.AddPropertyToBlackboard(floatProperty);
                    break;
                case ExposedProperty<int> intProperty:
                    _targetGraphView.AddPropertyToBlackboard(intProperty);
                    break;
                case ExposedProperty<string> stringProperty:
                    _targetGraphView.AddPropertyToBlackboard(stringProperty);
                    break;
            }
        }
    }
    private void ConnectNodes()
    {
        foreach (GraphNode graphNode in GraphNodes)
        {
            var connections = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == graphNode.GUID).ToList();
            
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].targetNodeGUID;
                var targetNode = GraphNodes.First(node => node.GUID == targetNodeGUID);
            
                LinkNodes(graphNode.outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);

        _targetGraphView.AddElement(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var cachedNode in _containerCache.GraphNodes)
        {
            switch (cachedNode)
            {
                case DialogueNodeData nodeData:
                    var dNode = _targetGraphView.CreateDialogueNode(nodeData.nodeName, nodeData.position, nodeData.speaker , nodeData.dialogueText);
                    dNode.GUID = nodeData.GUID;
                    _targetGraphView.AddElement(dNode);
                    //Debug.Log("dialogue node");
                    break;
                case ChoiceNodeData nodeData:
                    var cNode = _targetGraphView.CreateChoiceNode(nodeData.nodeName, nodeData.position);
                    cNode.GUID = nodeData.GUID;
                    _targetGraphView.AddElement(cNode);

                    var cNodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
                    cNodePorts.ForEach(port => _targetGraphView.AddChoicePort(cNode, port.portName));
                    //Debug.Log("choice node");
                    break;
                case IfNodeData nodeData:
                    var iNode = _targetGraphView.CreateIfNode(nodeData.nodeName, nodeData.position);
                    iNode.GUID = nodeData.GUID;
                    _targetGraphView.AddElement(iNode);

                    var iNodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
                    iNodePorts.ForEach(port => _targetGraphView.AddChoicePort(iNode, port.portName));
                    //Debug.Log("if node");
                    break;
            }
        }
    }

    private void ClearGraph()
    {
        _targetGraphView.ClearBlackboardAndExposedProperties();
        foreach (var node in _targetGraphView.nodes.ToList())
        {
            switch (node)
            {
                case DialogueNode dialogueNode:
                    if (dialogueNode.entryPoint) continue;
                    break;
            }
            
            //Removes the edges between nodes
            Edges.Where(edge => edge.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));
            
            //Removes nodes
            _targetGraphView.RemoveElement(node);
        }
    }
    
    private void CreateGraphBackup()
    {
        DialogueContainer backupBeforeLoad = Resources.Load<DialogueContainer>("BackupBeforeLoad");
        if (backupBeforeLoad != null)
        {
            AssetDatabase.DeleteAsset("Assets/Resources/BackupBeforeLoad.asset");
        }
        SaveData("BackupBeforeLoad");
    }
}
