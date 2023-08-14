using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class GraphNodeView : Node
{
    public GraphNode Node;
    
    public string GUID;
    public bool EntryPoint = false;
    public Port InputPort;
    public readonly List<Port> OutputPorts = new List<Port>();

    public readonly Vector2 DefaultNodeSize = new Vector2(400, 400);

    protected Editor editor;
    protected VisualElement inspector;

    protected GraphNodeView() : base("Assets/DialoguePlugin/Editor/Nodes/GraphNodeView.uxml")
    {
        inspector = mainContainer.Q<VisualElement>("inspector");
    }
    
    protected void GenerateMultiOutputButton(DialogueGraphView graphView, GraphNode node)
    {
        var button = new Button(() => { AddChoicePort(graphView, node); })
        {
            text = "New Output"
        };
        inputContainer.Add(button);
    }
    
    protected void AddChoicePort(DialogueGraphView graphView, GraphNode node, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(Direction.Output);

        //removes duplicate label
        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        var children = generatedPort.contentContainer.Children();
        foreach (var visualElement in children.Where(c => c.ClassListContains("connectorBox")))
        {
            visualElement.pickingMode = PickingMode.Position;
        }
        
        var outputPortCount = outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Output {outputPortCount}" : overriddenPortName;

        if (string.IsNullOrEmpty(overriddenPortName))
        {
            node.childPortName.Add(choicePortName);
        }
        
        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt =>
        {
            node.childPortName[node.childPortName.FindIndex(n => n == evt.previousValue)] = evt.newValue;
        });
        generatedPort.contentContainer.Add(textField);

        //permits removal of port
        var deleteButton = new Button(() => RemovePort(graphView, node, generatedPort)) { text = "X" };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        outputContainer.Add(generatedPort);
        OutputPorts.Add(generatedPort);
        
        
        
        //prevents visual glitch
        RefreshExpandedState();
        RefreshPorts();
    }

    private void RemovePort(GraphView graphView, GraphNode node, Port generatedPort)
    {
        node.childPortName.Remove(generatedPort.portName);
        
        var edges = generatedPort.connections.ToList();

        if (edges.Any())
        {
            generatedPort.DisconnectAll();
            foreach (var edge in edges)
            {
                graphView.RemoveElement(edge);
            }
        }
        
        outputContainer.Remove(generatedPort);
        RefreshExpandedState();
        RefreshPorts();
    }
    
    protected void GenerateInputPort()
    {
        var inputPort = GeneratePort(Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);
        InputPort = inputPort;
    }

    protected void GenerateOutputPort(string overrideName = "")
    {
        var outputPort = GeneratePort(Direction.Output);
        outputPort.portName = string.IsNullOrEmpty(overrideName) ? "Output" : overrideName;
        outputContainer.Add(outputPort);
        OutputPorts.Add(outputPort);
    }

    private Port GeneratePort(Direction portOrientation, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, portOrientation, capacity, typeof(float));
    }
}