using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;
    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

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
        dialogueContainer.ExposedPropertiesBool.AddRange(_targetGraphView.ExposedPropertiesBool);
        dialogueContainer.ExposedPropertiesFloat.AddRange(_targetGraphView.ExposedPropertiesFloat);
        dialogueContainer.ExposedPropertiesInt.AddRange(_targetGraphView.ExposedPropertiesInt);
        dialogueContainer.ExposedPropertiesString.AddRange(_targetGraphView.ExposedPropertiesString);
    }

    private bool SaveNode(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any()) return false; //No edges means no graph

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var inputNode = connectedPorts[i].input.node as DialogueNode;
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            
            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                baseNodeGUID = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGUID = inputNode.GUID
            });
        }

        foreach (var dialogueNode in Nodes.Where(node => !node.entryPoint))
        {
            dialogueContainer.DialogueNodes.Add(new DialogueNodeData
            {
                GUID = dialogueNode.GUID,
                speaker = dialogueNode.speaker,
                dialogueText = dialogueNode.dialogueText,
                position = dialogueNode.GetPosition().position
            });
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
        foreach (var property in _containerCache.ExposedPropertiesBool)
        {
            _targetGraphView.AddPropertyToBlackboard(property);
        }
        foreach (var property in _containerCache.ExposedPropertiesString)
        {
            _targetGraphView.AddPropertyToBlackboard(property);
        }
        foreach (var property in _containerCache.ExposedPropertiesFloat)
        {
            _targetGraphView.AddPropertyToBlackboard(property);
        }
        foreach (var property in _containerCache.ExposedPropertiesInt)
        {
            _targetGraphView.AddPropertyToBlackboard(property);
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == Nodes[i].GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].targetNodeGUID;
                var targetNode = Nodes.First(node => node.GUID == targetNodeGUID);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
                
                targetNode.SetPosition(new Rect(
                    _containerCache.DialogueNodes.First(node => node.GUID == targetNodeGUID).position, 
                    _targetGraphView.defaultNodeSize));
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
        foreach (var nodeData in _containerCache.DialogueNodes)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.dialogueText, Vector2.zero);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(node => node.baseNodeGUID == nodeData.GUID).ToList();
            nodePorts.ForEach(port => _targetGraphView.AddChoicePort(tempNode, port.portName));
        }
    }

    private void ClearGraph()
    {
        _targetGraphView.ClearBlackboardAndExposedProperties();
        foreach (var node in Nodes)
        {
            if (node.entryPoint) continue;
            
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
