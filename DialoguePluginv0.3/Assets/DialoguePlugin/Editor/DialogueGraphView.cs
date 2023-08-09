using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialoguePlugin/Editor/DialogueGraphEditor.uss"); 
        styleSheets.Add(styleSheet);
    }

    private DialogueContainer _container;

    private Vector2 _localMousePosition;

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    private GraphNodeView FindNodeView(GraphNode node)
    {
        return GetNodeByGuid(node.GUID) as GraphNodeView;
    }
    
    public void PopulateView(DialogueContainer container)
    {
        _container = container;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        container.graphNodes.ForEach(CreateNodeViewElement);
        CreateEntryPoint();
        
        container.graphNodes.ForEach(CreateEdgeViewElement);
    }

    private void CreateEdgeViewElement(GraphNode graphNode)
    {
        var parentView = FindNodeView(graphNode);
        for (var i = 0; i < graphNode.children.Count; i++)
        {
            var childView = FindNodeView(graphNode.children[i]);
            var edge = parentView.OutputPorts.First(port => port.portName == graphNode.childPortName[i]).ConnectTo(childView.InputPort);
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
                    _container.DeleteGraphNode(graphNode.Node);
                    break;
                case Edge edge:
                    var parentView = edge.output.node as GraphNodeView;
                    var childView = edge.input.node as GraphNodeView;
            
                    _container.RemoveChild(parentView!.Node, childView!.Node);
                    break;
                    
            }
        });
        
        graphViewChange.edgesToCreate?.ForEach(edge =>
        {
            var parentView = edge.output.node as GraphNodeView;
            var childView = edge.input.node as GraphNodeView;
            
            _container.AddChild(parentView!.Node, childView!.Node, edge.output.portName);
        });
        
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        _localMousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);
        var types = TypeCache.GetTypesDerivedFrom<GraphNode>();
        foreach (var type in types.Where(type => type.Name != "EntryNodeData" && type.Name != "EntryNode"))
        {
            evt.menu.AppendAction($"{type.Name}", (n) => CreateGraphNode(type, _localMousePosition));
        }
    }

    private void CreateEntryPoint()
    {
        if (_container.graphNodes.Any(node => node.entryNode))
            return;

        AddElement(new EntryNodeView(_container.CreateEntryGraphNode()));
    }

    private void CreateGraphNode(Type type, Vector2 pos)
    {
        var node = _container.CreateGraphNode(type, pos);
        
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
            case EventNode:
                graphNodeView = new EventNodeView(node);
                break;
            default:
                Debug.LogWarning($"{node.name} is not a valid node type");
                return;
        }
        AddElement(graphNodeView);
    }
}
