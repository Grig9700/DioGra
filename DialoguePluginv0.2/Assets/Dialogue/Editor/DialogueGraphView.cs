using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
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

    private Port GeneratePort(DialogueNode node, Direction portOrientation, Port.Capacity capacity = Port.Capacity.Single)
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
        AddElement(CreateDialogueNode(nodeName, position));
    }
    
    public DialogueNode CreateDialogueNode(string nodeName, Vector2 position)
    {
        var dialogueNode = new DialogueNode()
        {
            title = nodeName,
            speaker = "",
            dialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Single);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(dialogueNode); });
        button.text = "New Output";
        dialogueNode.titleContainer.Add(button);
        
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

    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "")
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

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
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

    // public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    // {
    //     var localPropertyName = exposedProperty.name;
    //     var localPropertyValue = exposedProperty.value;
    //     while (ExposedProperties.Any(x => x.name == localPropertyName))
    //         localPropertyName = $"{localPropertyName}(1)";
    //     
    //     var property = new ExposedProperty();
    //     property.name = localPropertyName;
    //     property.value = localPropertyValue;
    //     ExposedProperties.Add(property);
    //
    //     var container = new VisualElement();
    //     var blackboardField = new BlackboardField { text = property.name, typeText = "string" };
    //     container.Add(blackboardField);
    //
    //     var propertyValueTextField = new TextField
    //     {
    //         value = localPropertyValue
    //     };
    //     propertyValueTextField.RegisterValueChangedCallback(evt =>
    //     {
    //         var changingPropertyIndex = ExposedProperties.FindIndex(x => x.propertyName == property.name);
    //         ExposedProperties[changingPropertyIndex].propertyValue = evt.newValue;
    //     });
    //     var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
    //     container.Add(blackboardValueRow);
    //     
    //     Blackboard.Add(container);
    // }
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
    
    private void MakeBool(ExposedProperty<bool> exposedProperty, List<ExposedProperty<bool>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (exposedProperties.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<bool>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "bool" };
        container.Add(blackboardField);

        var propertyValueTextField = new TextField
        {
            value = localPropertyValue.ToString()
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = bool.Parse(evt.newValue);
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeFloat(ExposedProperty<float> exposedProperty, List<ExposedProperty<float>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (exposedProperties.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<float>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "float" };
        container.Add(blackboardField);

        var propertyValueTextField = new TextField
        {
            value = localPropertyValue.ToString(CultureInfo.CurrentCulture)
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = float.Parse(evt.newValue);
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeInt(ExposedProperty<int> exposedProperty, List<ExposedProperty<int>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (exposedProperties.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<int>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        exposedProperties.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "int" };
        container.Add(blackboardField);

        var propertyValueTextField = new TextField
        {
            value = localPropertyValue.ToString()
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = exposedProperties.FindIndex(x => x.name == property.name);
            exposedProperties[changingPropertyIndex].value = int.Parse(evt.newValue);
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeString(ExposedProperty<string> exposedProperty, List<ExposedProperty<string>> exposedProperties)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (exposedProperties.Any(x => x.name == localPropertyName))
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
}
