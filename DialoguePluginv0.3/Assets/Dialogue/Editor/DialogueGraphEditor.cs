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
        
        OnSelectionChange();
        
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
        //SaveExposedProperties(exposedPropertiesContainer);
        
        var dialogueContainer =
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*");
        if (dialogueContainer == null)
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DialogueContainer>(),
                $"Assets/Resources/Dialogues/New Dialogue.asset");
            
            dialogueContainer =
                FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*");
        }
        _graphView.PopulateView(dialogueContainer);
        
        AssetDatabase.SaveAssets();
    }
    
    private void OnSelectionChange()
    {
        DialogueContainer container = Selection.activeObject as DialogueContainer;
        if (container)
        {
            //var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            //saveUtility.LoadData(container.name, this);
            _graphView.PopulateView(container);
            //_graphView.Container = container;
        }
    }
}