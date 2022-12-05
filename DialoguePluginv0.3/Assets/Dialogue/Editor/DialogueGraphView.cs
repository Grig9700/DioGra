using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> {}

    public DialogueGraphView()//EditorWindow editorWindow)
    {
        Insert(0, new GridBackground());
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dialogue/Editor/DialogueGraphEditor.uss"); 
        styleSheets.Add(styleSheet);
        
        //AddElement(GenerateEntryPointNode());
    }
    
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public Blackboard Blackboard;

    private Editor _editor;
    
    public List<ExposedProperties> ExposedPropertiesList = new List<ExposedProperties>();
    private Dictionary<string, TempContainer> tempContainer = new Dictionary<string, TempContainer>();

    private NodeSearchWindow _nodeSearchWindow;
    
    private DialogueGraphEditor _dialogueEditor;
    
    public DialogueContainer Container;
    
    /*public DialogueGraphView(EditorWindow editorWindow)
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
    }*/

    public void Initialize(DialogueGraphEditor window)
    {
        _dialogueEditor = window;
        //AddSearchWindow();
    }
    
    /*private void AddSearchWindow()
    {
        _nodeSearchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _nodeSearchWindow.Initialize(_dialogueEditor, this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _nodeSearchWindow);
    }*/

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

    private Port GeneratePort(GraphNodeView nodeView, Direction portOrientation, Port.Capacity capacity = Port.Capacity.Single)
    {
        return nodeView.InstantiatePort(Orientation.Horizontal, portOrientation, capacity, typeof(float));
    }
    
    /*private DialogueNode GenerateEntryPointNode()
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
    }*/

    /*public void CreateNode(string nodeName, Vector2 position)
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
            case "Script Node":
                AddElement(CreateScriptNode(nodeName, position));
                return;
            default:
                Debug.LogWarning($"{nodeName} is not a valid node type");
                return;
        }
    }*/

    /*public void RestoreNode(GraphNodeData nodeData, List<NodeLinkData> linkedPorts = null)
    {
        switch (nodeData.nodeName)
        {
            case "Dialogue Node":
                AddElement(CreateDialogueNode(nodeData.nodeName, nodeData.position, nodeData));
                return;
            case "Choice Node":
                AddElement(CreateChoiceNode(nodeData.nodeName, nodeData.position, nodeData, linkedPorts));
                return;
            case "If Node":
                AddElement(CreateIfNode(nodeData.nodeName, nodeData.position, nodeData));
                return;
            default:
                Debug.LogWarning($"{nodeData.nodeName} is not a valid node type");
                return;
        }
    }*/

    public void PopulateView(DialogueContainer container)
    {
        Container = container;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        container.GraphNodes.ForEach(CreateNodeViewElement);
        CreateEntryPoint();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        graphViewChange.elementsToRemove?.ForEach(elem =>
        {
            if (elem is GraphNodeView graphNode)
                Container.DeleteGraphNode(graphNode.Node);
        });
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        var types = TypeCache.GetTypesDerivedFrom<GraphNode>();
        foreach (Type type in types.Where(type => type.Name != "EntryNodeData" && type.Name != "EntryNode"))
        {
            evt.menu.AppendAction($"{type.Name}", (n) => CreateGraphNode(type));
        }
    }

    private void CreateEntryPoint()
    {
        if (Container.GraphNodes.Any(node => node.entryNode))
            return;
        AddElement(new EntryNodeView(Container.CreateEntryGraphNode()));
    }
    
    private void CreateGraphNode(Type type)
    {
        GraphNode node = Container.CreateGraphNode(type);
        
        CreateNodeViewElement(node);
    }
    
    private void CreateNodeViewElement(GraphNode node)
    {
        GraphNodeView graphNodeView;
        switch (node)
        {
            case EntryNode:
                graphNodeView = new EntryNodeView(node);
                break;
            case DialogueNode:
                graphNodeView = new DialogueNodeView(node);
                break;
            case ChoiceNode: 
                graphNodeView = new ChoiceNodeView(node);
                break;
            case IfNode:
                graphNodeView = new IfNodeView(node);
                break;
            case ScriptNode:
                graphNodeView = new ScriptNodeView(node);
                break;
            default:
                Debug.LogWarning($"{node.name} is not a valid node type");
                return;
        }
        AddElement(graphNodeView);
    }
    
    /*private IfNode CreateIfNode(string nodeName, Vector2 position, GraphNodeData nodeData = null)
    {
        var ifNode = new IfNode()
        {
            title = nodeName,
            GUID = nodeData == null ? Guid.NewGuid().ToString() : nodeData.GUID,
        };

        ifNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(ifNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        ifNode.inputContainer.Add(inputPort);
        
        
        /*
        switch (ifNode.Property)
        {
            case ExposedProperty<bool> boolProperty:
                boolProperty.value = false;
                break;
        }
        
        SerializeField ifNode.Property;
        typeSelector.
        typeSelector.RegisterValueChangedCallback(evt =>
        {
            ifNode.VariableType = evt.newValue
        });
        ifNode.titleButtonContainer.Add(typeSelector);
        PopupField<ExposedProperty<bool>> elementSelector = new PopupField<ExposedProperty<bool>>();
        ifNode.outputContainer.Add(elementSelector);#1#

        TempContainer temp = ScriptableObject.CreateInstance<TempContainer>();
        tempContainer.Add(ifNode.GUID, temp);
        temp.Property = new ExposedProperty<bool>();
        temp.VariableType = TempContainer.ExposedVariableType.Bool;

        //ifNode.Property = ScriptableObject.CreateInstance<ExposedProperties>();
        SerializedObject serializedObject = new UnityEditor.SerializedObject(temp);
        SerializedProperty serializedProperty = serializedObject.FindProperty("Property");
        
        
        //I weep, so much, I cry, endless tears, why world, will you not write out this variable
        
        
        //ExtendedEditorWindow.DrawField("Property", false, serializedObject, serializedProperty);
        EditorGUILayout.PropertyField(serializedProperty, new GUIContent("Exposed Property"), GUILayout.Height(20));
        //ifNode.Insert();
        
        PropertyField property = new PropertyField(serializedProperty, "Exposed Property");
        property.RegisterValueChangeCallback(evt =>
        {
            serializedObject.ApplyModifiedProperties();
        });
        ifNode.titleButtonContainer.Add(property);
        ifNode.mainContainer.Add(property);
        ifNode.mainContainer.Add(new PopupField<ExposedProperties>());
        
        var outputPortTrue = GeneratePort(ifNode, Direction.Output, Port.Capacity.Single);
        outputPortTrue.portName = "True";
        ifNode.outputContainer.Add(outputPortTrue);
        var outputPortFalse = GeneratePort(ifNode, Direction.Output, Port.Capacity.Single);
        outputPortFalse.portName = "False";
        ifNode.outputContainer.Add(outputPortFalse);
        
        ifNode.RefreshExpandedState();
        ifNode.RefreshPorts();
        
        ifNode.SetPosition(new Rect(position, defaultNodeSize));

        return ifNode;
    }*/

    /*private ChoiceNode CreateChoiceNode(string nodeName, Vector2 position, GraphNodeData nodeData = null, List<NodeLinkData> linkedPorts = null)
    {
        var choiceNode = new ChoiceNode()
        {
            title = nodeName,
            GUID = nodeData == null ? Guid.NewGuid().ToString() : nodeData.GUID
        };

        choiceNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(choiceNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        choiceNode.inputContainer.Add(inputPort);
        
        var button = new Button(() => { AddChoicePort(choiceNode); });
        button.text = "New Output";
        choiceNode.titleContainer.Add(button);

        if (linkedPorts != null)
        {
            foreach (var link in linkedPorts)
            {
                AddChoicePort(choiceNode, link.portName);
            }
        }
        
        choiceNode.RefreshExpandedState();
        choiceNode.RefreshPorts();
        
        choiceNode.SetPosition(new Rect(position, defaultNodeSize));

        return choiceNode;
    }*/

    /*private ScriptNode CreateScriptNode(string nodeName, Vector2 position, GraphNodeData nodeData = null, List<NodeLinkData> linkedPorts = null)
    {
        var scriptNode = new ScriptNode()
        {
            title = nodeName,
            GUID = nodeData == null ? Guid.NewGuid().ToString() : nodeData.GUID,
            call = ScriptableObject.CreateInstance<ScriptNodeCalls>()
        };

        scriptNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        
        //Generates input port
        var inputPort = GeneratePort(scriptNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        scriptNode.inputContainer.Add(inputPort);

        var generatedPort = GeneratePort(scriptNode, Direction.Output);
        generatedPort.portName = "Output";
        scriptNode.outputContainer.Add(generatedPort);

        
        
        UnityEngine.Object.DestroyImmediate(scriptNode.Editor);
        scriptNode.Editor = Editor.CreateEditor(scriptNode.call);
        IMGUIContainer container = new IMGUIContainer(() => { scriptNode.Editor.OnInspectorGUI(); });
        //scriptNode.Add(container);
        //scriptNode.contentContainer.Add(container);
        scriptNode.outputContainer.Add(container);
        
        // var button = new Button(() => { AddFunctionCall(scriptNode); });
        // button.text = "New Script Call";
        // scriptNode.titleContainer.Add(button);


        if (linkedPorts != null)
        {
            foreach (var link in linkedPorts)
            {
                AddChoicePort(scriptNode, link.portName);
            }
        }
        
        scriptNode.RefreshExpandedState();
        scriptNode.RefreshPorts();
        
        scriptNode.SetPosition(new Rect(position, defaultNodeSize));

        return scriptNode;
    }*/
    
    /*private DialogueNode CreateDialogueNode(string nodeName, Vector2 position, GraphNodeData nodeData = null)
    {
        var dialogueNode = new DialogueNode()
        {
            title = nodeName,
            speaker = nodeData == null ? "" : nodeData.speaker,
            dialogueText = nodeData == null ? "" : nodeData.dialogueText,
            GUID = nodeData == null ? Guid.NewGuid().ToString() : nodeData.GUID
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

        var speakerField = new TextField("Speaker") {value = dialogueNode.speaker};
        speakerField.RegisterValueChangedCallback(evt => dialogueNode.speaker = evt.newValue);
        dialogueNode.mainContainer.Add(speakerField);
        
        var textField = new TextField(string.Empty) {value = dialogueNode.dialogueText};
        textField.RegisterValueChangedCallback(evt => dialogueNode.dialogueText = evt.newValue);
        dialogueNode.mainContainer.Add(textField);
            
        //prevents visual glitch
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        
        dialogueNode.SetPosition(new Rect(position, defaultNodeSize));

        return dialogueNode;
    }*/

    private void AddChoicePort(GraphNodeView dialogueNodeView, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNodeView, Direction.Output);

        //removes duplicate label
        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);
        
        var outputPortCount = dialogueNodeView.outputContainer.Query("connector").ToList().Count;
        
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
        var deleteButton = new Button(() => RemovePort(dialogueNodeView, generatedPort)) { text = "X" };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        dialogueNodeView.outputContainer.Add(generatedPort);
        
        //prevents visual glitch
        dialogueNodeView.RefreshExpandedState();
        dialogueNodeView.RefreshPorts();
    }

    private void RemovePort(GraphNodeView graphNodeView, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(
            edge => edge.output.portName == generatedPort.portName && edge.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        graphNodeView.outputContainer.Remove(generatedPort);
        graphNodeView.RefreshExpandedState();
        graphNodeView.RefreshPorts();
    }

    private void AddFunctionCall(GraphNodeView graphNodeView)
    {
        UnityEngine.Object.DestroyImmediate(graphNodeView.Editor);
        graphNodeView.Editor = Editor.CreateEditor(graphNodeView.call);
        IMGUIContainer container = new IMGUIContainer(() => { graphNodeView.Editor.OnInspectorGUI(); });
        graphNodeView.Add(container);
    }

    public void ClearBlackboardAndExposedProperties()
    {
        ExposedPropertiesList.Clear();
        Blackboard.Clear();
    }
    
    public void AddPropertyToBlackboard(ExposedProperty<int> propertyInt)
    {
        MakeInt(propertyInt);
    }
    public void AddPropertyToBlackboard(ExposedProperty<float> propertyFloat)
    {
        MakeFloat(propertyFloat);
    }
    public void AddPropertyToBlackboard(ExposedProperty<string> propertyString)
    {
        MakeString(propertyString);
    }
    public void AddPropertyToBlackboard(ExposedProperty<bool> propertyBool)
    {
        MakeBool(propertyBool);
    }

    public void RemovePropertyFromBlackboard()
    {
        
    }
    
    private void MakeBool(ExposedProperty<bool> exposedProperty)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (ExposedPropertiesList.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<bool>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        ExposedPropertiesList.Add(property);
                
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
            var changingPropertyIndex = ExposedPropertiesList.FindIndex(x => x.name == property.name);
            switch (ExposedPropertiesList[changingPropertyIndex])
            {
                case ExposedProperty<bool> boolProperty:
                    boolProperty.value = evt.newValue;
                    break;
            }
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeFloat(ExposedProperty<float> exposedProperty)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (ExposedPropertiesList.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<float>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        ExposedPropertiesList.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "float" };
        container.Add(blackboardField);

        var propertyValueTextField = new FloatField()
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedPropertiesList.FindIndex(x => x.name == property.name);
            switch (ExposedPropertiesList[changingPropertyIndex])
            {
                case ExposedProperty<float> boolProperty:
                    boolProperty.value = evt.newValue;
                    break;
            }
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeInt(ExposedProperty<int> exposedProperty)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (ExposedPropertiesList.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<int>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        ExposedPropertiesList.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "int" };
        container.Add(blackboardField);

        var propertyValueTextField = new IntegerField()
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedPropertiesList.FindIndex(x => x.name == property.name);
            switch (ExposedPropertiesList[changingPropertyIndex])
            {
                case ExposedProperty<int> boolProperty:
                    boolProperty.value = evt.newValue;
                    break;
            }
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
    private void MakeString(ExposedProperty<string> exposedProperty)
    {
        var localPropertyName = exposedProperty.name;
        var localPropertyValue = exposedProperty.value;
        while (ExposedPropertiesList.Any(x => x.name == localPropertyName))
            localPropertyName = $"{localPropertyName}(1)";
                
        var property = new ExposedProperty<string>();
        property.name = localPropertyName;
        property.value = localPropertyValue;
        ExposedPropertiesList.Add(property);
                
        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.name, typeText = "string" };
        container.Add(blackboardField);

        var propertyValueTextField = new TextField
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedPropertiesList.FindIndex(x => x.name == property.name);
            switch (ExposedPropertiesList[changingPropertyIndex])
            {
                case ExposedProperty<string> boolProperty:
                    boolProperty.value = evt.newValue;
                    break;
            }
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);
        
        Blackboard.Add(container);
    }
}
