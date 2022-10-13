using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public Blackboard Blackboard;

    public List<ExposedProperty<int>> ExposedPropertiesInt = new List<ExposedProperty<int>>();
    public List<ExposedProperty<float>> ExposedPropertiesFloat = new List<ExposedProperty<float>>();
    public List<ExposedProperty<string>> ExposedPropertiesString = new List<ExposedProperty<string>>();
    public List<ExposedProperty<bool>> ExposedPropertiesBool = new List<ExposedProperty<bool>>();
    //public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();

    private NodeSearchWindow _nodeSearchWindow;
    
    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //sets up grid backgrounds
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _nodeSearchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _nodeSearchWindow.Init(editorWindow, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _nodeSearchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        //Done to allow ports of different types to connect as we don't need dataflow between them atm
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    private Port GeneratePort(GraphNode node, Direction portOrientation, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portOrientation, capacity, typeof(float));
    }
    
    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode()
        {
            title = "START",
            GUID = "StartPoint",
            dialogueText = "ENTRY POINT",
            entryPoint = true
        };

        //Generates start output
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        //Prevents moving or deleting start node
        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;
        
        //prevents visual glitch
        node.RefreshExpandedState();
        node.RefreshPorts();
        
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName, Vector2 position)
    {
        switch (nodeName)
        {
            case "Dialogue Node":
                AddElement(CreateDialogueNode(nodeName, position));
                return;
            case "Choice Node":
                AddElement(CreateChoiceNode(nodeName, position));
                return;
            case "If Node":
                AddElement(CreateIfNode(nodeName, position));
                return;
            default:
                return;
        }
    }

    public IfNode CreateIfNode(string nodeName, Vector2 position)
    {
        var ifNode = new IfNode()
        {
            title = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        ifNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(ifNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        ifNode.inputContainer.Add(inputPort);
        
        var button = new Button(() => { AddChoicePort(ifNode); });
        button.text = "New Output";
        ifNode.titleContainer.Add(button);
        
        ifNode.RefreshExpandedState();
        ifNode.RefreshPorts();
        
        ifNode.SetPosition(new Rect(position, defaultNodeSize));

        return ifNode;
    }
    
    public ChoiceNode CreateChoiceNode(string nodeName, Vector2 position)
    {
        var choiceNode = new ChoiceNode()
        {
            title = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        choiceNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(choiceNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        choiceNode.inputContainer.Add(inputPort);
        
        var button = new Button(() => { AddChoicePort(choiceNode); });
        button.text = "New Output";
        choiceNode.titleContainer.Add(button);
        
        choiceNode.RefreshExpandedState();
        choiceNode.RefreshPorts();
        
        choiceNode.SetPosition(new Rect(position, defaultNodeSize));

        return choiceNode;
    }
    
    public DialogueNode CreateDialogueNode(string nodeName, Vector2 position, string speaker = "", string dialogueText = "")
    {
        var dialogueNode = new DialogueNode()
        {
            title = nodeName,
            speaker = speaker,
            dialogueText = dialogueText,
            GUID = Guid.NewGuid().ToString()
        };

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        //Generates output port
        var outputPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Single);
        outputPort.portName = "Output";
        dialogueNode.outputContainer.Add(outputPort);

        var speakerField = new TextField("Speaker");
        speakerField.RegisterValueChangedCallback(evt => dialogueNode.speaker = evt.newValue);
        dialogueNode.mainContainer.Add(speakerField);
        
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => dialogueNode.dialogueText = evt.newValue);
        dialogueNode.mainContainer.Add(textField);
            
        //prevents visual glitch
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        
        dialogueNode.SetPosition(new Rect(position, defaultNodeSize));

        return dialogueNode;
    }

    public void AddChoicePort(GraphNode dialogueNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        //removes duplicate label
        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        
        //creates name of port
        var choicePortName = string.IsNullOrEmpty(overriddenPortName)? $"Output {outputPortCount}" : overriddenPortName;
        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("    "));
        generatedPort.contentContainer.Add(textField);

        //permits removal of port
        var deleteButton = new Button((() => RemovePort(dialogueNode, generatedPort))) { text = "X" };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        
        //prevents visual glitch
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(GraphNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(
            edge => edge.output.portName == generatedPort.portName && edge.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    public void ClearBlackboardAndExposedProperties()
    {
        //ExposedProperties.Clear();
        ExposedPropertiesBool.Clear();
        ExposedPropertiesFloat.Clear();
        ExposedPropertiesInt.Clear();
        ExposedPropertiesString.Clear();
        Blackboard.Clear();
    }
    
    public void AddPropertyToBlackboard(ExposedProperty<int> propertyInt)
    {
        MakeInt(propertyInt, ExposedPropertiesInt);
    }
    public void AddPropertyToBlackboard(ExposedProperty<float> propertyFloat)
    {
        MakeFloat(propertyFloat, ExposedPropertiesFloat);
    }
    public void AddPropertyToBlackboard(ExposedProperty<string> propertyString)
    {
        MakeString(propertyString, ExposedPropertiesString);
    }
    public void AddPropertyToBlackboard(ExposedProperty<bool> propertyBool)
    {
        MakeBool(propertyBool, ExposedPropertiesBool);
    }

    public void RemovePropertyFromBlackboard()
    {
        
    }
    
    private void MakeBool(ExposedProperty<bool> exposedProperty, List<ExposedProperty<bool>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (CheckIfNameExists(localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<bool>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "bool" };
        //blackboardField.IsDroppable();
        //blackboardField.RegisterCallback<DeleteSelectionDelegate>();
        //blackboardField.
        //container.Add(blackboardField);
        
        var propertyValueTextField = new Toggle()
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = evt.newValue;
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeFloat(ExposedProperty<float> exposedProperty, List<ExposedProperty<float>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (CheckIfNameExists(localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<float>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "float" };
        container.Add(blackboardField);

        var propertyValueTextField = new FloatField()
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = evt.newValue;
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeInt(ExposedProperty<int> exposedProperty, List<ExposedProperty<int>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (CheckIfNameExists(localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<int>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "int" };
        container.Add(blackboardField);

        var propertyValueTextField = new IntegerField()
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = evt.newValue;
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeString(ExposedProperty<string> exposedProperty, List<ExposedProperty<string>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (CheckIfNameExists(localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<string>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "string" };
        container.Add(blackboardField);

        var propertyValueTextField = new TextField
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = evt.newValue;
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }

    public bool CheckIfNameExists(string newValue)
    {
        return ExposedPropertiesBool.Any(x => x.name == newValue) ||
               ExposedPropertiesFloat.Any(x => x.name == newValue) ||
               ExposedPropertiesInt.Any(x => x.name == newValue) ||
               ExposedPropertiesString.Any(x => x.name == newValue);
    }
}
