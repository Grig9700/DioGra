using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> {}

    public DialogueGraphView()
    {
        Insert(0, new GridBackground());
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dialogue/Editor/DialogueGraphEditor.uss"); 
        styleSheets.Add(styleSheet);
    }
    
    public Blackboard Blackboard;

    private Editor _editor;
    
    public List<ExposedProperties> ExposedPropertiesList = new List<ExposedProperties>();
    private Dictionary<string, TempContainer> tempContainer = new Dictionary<string, TempContainer>();

    private NodeSearchWindow _nodeSearchWindow;
    
    private DialogueGraphEditor _dialogueEditor;
    
    public DialogueContainer Container;

    private float _timer;
    private Vector2 _localMousePosition;

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
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    private Port GeneratePort(GraphNodeView nodeView, Direction portOrientation, Port.Capacity capacity = Port.Capacity.Single)
    {
        return nodeView.InstantiatePort(Orientation.Horizontal, portOrientation, capacity, typeof(float));
    }

    private GraphNodeView FindNodeView(GraphNode node)
    {
        return GetNodeByGuid(node.GUID) as GraphNodeView;
    }
    
    public void PopulateView(DialogueContainer container)
    {
        Container = container;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        //create nodes
        container.GraphNodes.ForEach(CreateNodeViewElement);
        CreateEntryPoint();
        
        //create edges
        container.GraphNodes.ForEach(CreateEdgeViewElement);
    }

    private void CreateEdgeViewElement(GraphNode graphNode)
    {
        GraphNodeView parentView = FindNodeView(graphNode);
        for (int i = 0; i < graphNode.children.Count; i++)
        {
            GraphNodeView childView = FindNodeView(graphNode.children[i]);
            Edge edge = parentView.OutputPorts.First(port => port.portName == graphNode.childPortName[i]).ConnectTo(childView.InputPort);
            AddElement(edge);
        }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        graphViewChange.elementsToRemove?.ForEach(elem =>
        {
            switch (elem)
            {
                case GraphNodeView graphNode:
                    Container.DeleteGraphNode(graphNode.Node);
                    break;
                /*case Port port:
                    if (port.direction != Direction.Output)
                        break;
                    var edges = port.connections;
                    port.DisconnectAll();
                    foreach (var edge in edges)
                    {
                        RemoveElement(edge);
                    }
                    break;*/
                case Edge edge:
                    GraphNodeView parentView = edge.output.node as GraphNodeView;
                    GraphNodeView childView = edge.input.node as GraphNodeView;
            
                    Container.RemoveChild(parentView!.Node, childView!.Node);
                    break;
                    
            }
        });
        
        graphViewChange.edgesToCreate?.ForEach(edge =>
        {
            GraphNodeView parentView = edge.output.node as GraphNodeView;
            GraphNodeView childView = edge.input.node as GraphNodeView;
            
            Container.AddChild(parentView!.Node, childView!.Node, edge.output.portName);
        });
        
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        _localMousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);
        //base.BuildContextualMenu(evt);
        var types = TypeCache.GetTypesDerivedFrom<GraphNode>();
        foreach (Type type in types.Where(type => type.Name != "EntryNodeData" && type.Name != "EntryNode"))
        {
            evt.menu.AppendAction($"{type.Name}", (n) => CreateGraphNode(type, _localMousePosition));
        }
    }

    private void CreateEntryPoint()
    {
        if (Container.GraphNodes.Any(node => node.entryNode))
            return;

        AddElement(new EntryNodeView(Container.CreateEntryGraphNode()));
        //DelayEntryNode();
    }

    private async void DelayEntryNode()
    {
        await Task.Delay(50);
        Selection.activeGameObject = null;
        AddElement(new EntryNodeView(Container.CreateEntryGraphNode()));
    }
    
    private void CreateGraphNode(Type type, Vector2 pos)
    {
        GraphNode node = Container.CreateGraphNode(type, pos);
        
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
                graphNodeView = new ChoiceNodeView(node, this);
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
