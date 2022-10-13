using System;
using System.Collections;
using System.Collections.Generic;
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
        if (!SaveNode(dialogueContainer)) return;
        SaveExposedProperties(dialogueContainer);

        //Creates a resources folder if not present
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{filename}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        //dialogueContainer.ExposedPropertiesBool.AddRange(_targetGraphView.ExposedPropertiesBool);
        //dialogueContainer.ExposedPropertiesFloat.AddRange(_targetGraphView.ExposedPropertiesFloat);
        //dialogueContainer.ExposedPropertiesInt.AddRange(_targetGraphView.ExposedPropertiesInt);
        //dialogueContainer.ExposedPropertiesString.AddRange(_targetGraphView.ExposedPropertiesString);
        dialogueContainer.ExposedPropertiesList.AddRange(_targetGraphView.ExposedPropertiesList);
    }

    private bool SaveNode(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any()) return false; //No edges means no graph

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var inputNode = connectedPorts[i].input.node as GraphNode;
            var outputNode = connectedPorts[i].output.node as GraphNode;
            
            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                baseNodeGUID = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
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
                    break;
                case ChoiceNode choiceNode:
                    dialogueContainer.GraphNodes.Add(new ChoiceNodeData
                    {
                        nodeName = choiceNode.title,
                        GUID = choiceNode.GUID,
                        position = choiceNode.GetPosition().position
                    });
                    break;
                case IfNode ifNode:
                    dialogueContainer.GraphNodes.Add(new IfNodeData
                    {
                        nodeName = ifNode.title,
                        GUID = ifNode.GUID,
                        position = ifNode.GetPosition().position
                    });
                    break;
            }
            
        }
        return true;
    }
    
    public void LoadData(string filename)
    {
        _containerCache = Resources.Load<DialogueContainer>(filename);
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
        CreateExposedVariables();
    }

    public void Clear()
    {
        CreateGraphBackup();
        ClearGraph();
    }
    
    private void CreateExposedVariables()
    {
        _targetGraphView.ClearBlackboardAndExposedProperties();
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
        // foreach (var property in _containerCache.ExposedPropertiesBool)
        // {
        //     _targetGraphView.AddPropertyToBlackboard(property);
        // }
        // foreach (var property in _containerCache.ExposedPropertiesString)
        // {
        //     _targetGraphView.AddPropertyToBlackboard(property);
        // }
        // foreach (var property in _containerCache.ExposedPropertiesFloat)
        // {
        //     _targetGraphView.AddPropertyToBlackboard(property);
        // }
        // foreach (var property in _containerCache.ExposedPropertiesInt)
        // {
        //     _targetGraphView.AddPropertyToBlackboard(property);
        // }
    }
    private void ConnectNodes()
    {
        for (int i = 0; i < GraphNodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == GraphNodes[i].GUID).ToList();
            
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].targetNodeGUID;
                var targetNode = GraphNodes.First(node => node.GUID == targetNodeGUID);
            
                LinkNodes(GraphNodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
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
                    break;
                case ChoiceNodeData nodeData:
                    var cNode = _targetGraphView.CreateChoiceNode(nodeData.nodeName, nodeData.position);
                    cNode.GUID = nodeData.GUID;
                    _targetGraphView.AddElement(cNode);

                    var cNodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
                    cNodePorts.ForEach(port => _targetGraphView.AddChoicePort(cNode, port.portName));
                    break;
                case IfNodeData nodeData:
                    var iNode = _targetGraphView.CreateIfNode(nodeData.nodeName, nodeData.position);
                    iNode.GUID = nodeData.GUID;
                    _targetGraphView.AddElement(iNode);

                    var iNodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
                    iNodePorts.ForEach(port => _targetGraphView.AddChoicePort(iNode, port.portName));
                    break;
            }
        }
        // foreach (var nodeData in _containerCache.DialogueNodes)
        // {
        //     var tempNode = _targetGraphView.CreateDialogueNode(nodeData.nodeName, nodeData.position, nodeData.speaker , nodeData.dialogueText);
        //     tempNode.GUID = nodeData.GUID;
        //     _targetGraphView.AddElement(tempNode);
        //
        //     //var nodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
        //     //nodePorts.ForEach(port => _targetGraphView.AddChoicePort(tempNode, port.portName));
        // }
        // foreach (var nodeData in _containerCache.ChoiceNodes)
        // {
        //     var tempNode = _targetGraphView.CreateChoiceNode(nodeData.nodeName, nodeData.position);
        //     tempNode.GUID = nodeData.GUID;
        //     _targetGraphView.AddElement(tempNode);
        //
        //     var nodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
        //     nodePorts.ForEach(port => _targetGraphView.AddChoicePort(tempNode, port.portName));
        // }
        // foreach (var nodeData in _containerCache.IfNodes)
        // {
        //     var tempNode = _targetGraphView.CreateIfNode(nodeData.nodeName, nodeData.position);
        //     tempNode.GUID = nodeData.GUID;
        //     _targetGraphView.AddElement(tempNode);
        //
        //     var nodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
        //     nodePorts.ForEach(port => _targetGraphView.AddChoicePort(tempNode, port.portName));
        // }
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
