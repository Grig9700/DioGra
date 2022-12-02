using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    private ExposedPropertyContainer _propertyCache;
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

        //Creates folders if not present
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");

        var exposedPropertiesContainer =
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<ExposedPropertyContainer>("ExposedPropertyContainer*");
        if (exposedPropertiesContainer == null)
        {
            exposedPropertiesContainer = ScriptableObject.CreateInstance<ExposedPropertyContainer>();
            AssetDatabase.CreateAsset(exposedPropertiesContainer, $"Assets/Resources/ExposedPropertyContainer.asset");
        }
        SaveExposedProperties(exposedPropertiesContainer);
        
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Dialogues/{filename}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(ExposedPropertyContainer container)
    {
        foreach (var property in _targetGraphView.ExposedPropertiesList)
        {
            if (container.ExposedPropertyDatas.Exists(x => x.Name == property.name))
            {
                var prop = container.ExposedPropertyDatas.First(x => x.Name == property.name);
                prop.ValueContainer = GetPropertyValueAsString(property);
                prop.ValueType = property.GetType().ToString();
                continue;
            }
            
            container.ExposedPropertyDatas.Add(new ExposedPropertyData()
            {
                Name = property.name,
                ValueContainer = GetPropertyValueAsString(property),
                ValueType = property.GetType().ToString()
            });
        }
    }

    private string GetPropertyValueAsString(ExposedProperties property)
    {
        switch (property)
        {
            case ExposedProperty<bool> prop:
                string boolProp = prop.value.ToString();
                return boolProp;
            case ExposedProperty<float> prop:
                string floatProp = prop.value.ToString(CultureInfo.CurrentCulture);
                return floatProp;
            case ExposedProperty<int> prop:
                string intProp = prop.value.ToString();
                return intProp;
            case ExposedProperty<string> prop:
                return prop.value ?? (prop.value = "");
            default:
                Debug.LogError($"Property of type {property.GetType()} could not be saved");
                return null;
        }
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
                    dialogueContainer.GraphNodes.Add(new GraphNodeData()
                    {
                        nodeName = dialogueNode.title,
                        GUID = dialogueNode.GUID,
                        speaker = dialogueNode.speaker,
                        dialogueText = dialogueNode.dialogueText,
                        position = dialogueNode.GetPosition().position,
                        dialogueGraphNodeType = DialogueGraphNodeType.DialogueNode
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
                    dialogueContainer.GraphNodes.Add(new GraphNodeData()
                    {
                        nodeName = choiceNode.title,
                        GUID = choiceNode.GUID,
                        position = choiceNode.GetPosition().position,
                        dialogueGraphNodeType = DialogueGraphNodeType.ChoiceNode
                    });
                    //Debug.Log("choice node");
                    break;
                case IfNode ifNode:
                    dialogueContainer.GraphNodes.Add(new IfNodeData
                    {
                        nodeName = ifNode.title,
                        GUID = ifNode.GUID,
                        position = ifNode.GetPosition().position,
                        dialogueGraphNodeType = DialogueGraphNodeType.IfNode
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
        _containerCache = FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>($"{filename}.asset", "/Dialogues");
        _propertyCache = FindAndLoadResource.FindAndLoadFirstInResourceFolder<ExposedPropertyContainer>("ExposedPropertyContainer*");
        
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
        
        if (_propertyCache == null)
            return;
        
        foreach (var property in _propertyCache.ExposedPropertyDatas)
        {
             switch (property.ValueType)
             {
                 case "ExposedProperty`1[System.Boolean]":
                     _targetGraphView.AddPropertyToBlackboard(new ExposedProperty<bool>() {name = property.Name, value = bool.Parse(property.ValueContainer)});
                     //_targetGraphView.AddPropertyToBlackboard(RestoreProperty<bool>(property.Name, property.ValueContainer));
                     break;
                 case "ExposedProperty`1[System.Single]":
                     _targetGraphView.AddPropertyToBlackboard(new ExposedProperty<float>(){name = property.Name, value = float.Parse(property.ValueContainer)});
                     //_targetGraphView.AddPropertyToBlackboard(RestoreProperty<float>(property.Name, property.ValueContainer));
                     break;
                 case "ExposedProperty`1[System.Int32]":
                     _targetGraphView.AddPropertyToBlackboard(new ExposedProperty<int>(){name = property.Name, value = int.Parse(property.ValueContainer)});
                     //_targetGraphView.AddPropertyToBlackboard(RestoreProperty<int>(property.Name, property.ValueContainer));
                     break;
                 case "ExposedProperty`1[System.String]":
                     _targetGraphView.AddPropertyToBlackboard(new ExposedProperty<string>(){name = property.Name, value = property.ValueContainer});
                     //_targetGraphView.AddPropertyToBlackboard(new ExposedProperty<string>(){name = property.Name, value = property.ValueContainer});
                     break;
             }
        }
    }

    // private ExposedProperty<T> RestoreProperty<T>(string name, string value)// where T : Type
    // {
    //     var instance = Activator.CreateInstance(typeof(T));
    //     var tryParseMethod = typeof(T).GetMethod("TryParse");
    //     var parseMethod = typeof(T).GetMethod("Parse");
    //
    //     var tryResult = tryParseMethod.Invoke(instance, new[] { $"{name}" });
    //
    //     if (tryResult == null)
    //     {
    //         Debug.LogError($"Parameter by name of {name} has disallowed value : {value}");
    //     }
    //     
    //     return new ExposedProperty<T>(){ name = name, value = (T)tryResult };
    // }
    
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
            switch (cachedNode.dialogueGraphNodeType)
            {
                case DialogueGraphNodeType.DialogueNode:
                    _targetGraphView.RestoreNode(cachedNode);
                    //Debug.Log("dialogue node");
                    break;
                case DialogueGraphNodeType.ChoiceNode:
                    _targetGraphView.RestoreNode(cachedNode, _containerCache.NodeLinks.Where(node => node.baseNodeGUID == cachedNode.GUID).ToList());
                    //Debug.Log("choice node");
                    break;
                case DialogueGraphNodeType.IfNode:
                    _targetGraphView.RestoreNode(cachedNode);
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
