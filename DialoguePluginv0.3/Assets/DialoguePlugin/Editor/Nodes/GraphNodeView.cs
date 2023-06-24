using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

    protected void GenerateMultiOutputButton(DialogueGraphView graphView)
    {
        var button = new Button(() => { AddChoicePort(graphView); })
        {
            text = "New Output"
        };
        titleContainer.Add(button);
    }
    
    protected void AddChoicePort(DialogueGraphView graphView, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(Direction.Output);
        OutputPorts.Add(generatedPort);

        //removes duplicate label
        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        var outputPortCount = outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Output {outputPortCount}" : overriddenPortName;

        if (string.IsNullOrEmpty(overriddenPortName))
        {
            switch (Node)
            {
                case ChoiceNode choiceNode:
                    choiceNode.outputOptions.Add(choicePortName);
                    break;
            }
        }
        
        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt =>
        {
            switch (Node)
            {
                case ChoiceNode choiceNode:
                    for (var i = 0; i < choiceNode.outputOptions.Count; i++)
                    {
                        if (choiceNode.outputOptions[i] != generatedPort.portName) continue;
                        choiceNode.outputOptions[i] = evt.newValue;
                        break;
                    }
                    break;
            }
            generatedPort.portName = evt.newValue;
        });
        generatedPort.contentContainer.Add(new Label("    "));
        generatedPort.contentContainer.Add(textField);

        //permits removal of port
        var deleteButton = new Button(() => RemovePort(graphView, generatedPort)) { text = "X" };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        outputContainer.Add(generatedPort);
        
        //prevents visual glitch
        RefreshExpandedState();
        RefreshPorts();
    }

    private void RemovePort(GraphView graphView, Port generatedPort)
    {
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
        outputPort.portName = String.IsNullOrEmpty(overrideName) ? "Output" : overrideName;
        outputContainer.Add(outputPort);
        OutputPorts.Add(outputPort);
    }

    private Port GeneratePort(Direction portOrientation, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, portOrientation, capacity, typeof(float));
    }
}