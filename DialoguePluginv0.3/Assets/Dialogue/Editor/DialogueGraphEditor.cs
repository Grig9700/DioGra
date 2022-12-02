using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DialogueGraphEditor : EditorWindow
{
    private DialogueGraphView _graphView;
    private InspectorView _inspectorView;
    
    
    [MenuItem("Dialogue Graph Editor/Editor...")]
    public static void ShowExample()
    {
        DialogueGraphEditor window = GetWindow<DialogueGraphEditor>();
        window.titleContent = new GUIContent("DialogueGraphEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Dialogue/Editor/DialogueGraphEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Dialogue/Editor/DialogueGraphEditor.uss");
        root.styleSheets.Add(styleSheet);

        _graphView = root.Q<DialogueGraphView>();
        _graphView.Initialize(this);
        _inspectorView = root.Q<InspectorView>();
    }
    
    private void OnSelectionChange()
    {
        DialogueContainer container = Selection.activeObject as DialogueContainer;
        if (container)
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            saveUtility.LoadData(container.name, this);
            _graphView.Container = container;
            //_filename = container.name;
            //_toolbar.MarkDirtyRepaint();
        }
    }
}