using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;

public class DialogueGraphEditor : EditorWindow
{
    private DialogueGraphView _graphView;
    private InspectorView _inspectorView;
    
    [MenuItem("Dialogue Graph Editor/Editor Window")]
    public static void CreateEditorWindow()
    {
        var window = GetWindow<DialogueGraphEditor>();
        window.titleContent = new GUIContent("DialogueGraphEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject is not DialogueContainer) 
            return false;
        
        CreateEditorWindow();
        return true;
    }
    
    [MenuItem("Dialogue Graph Editor/New Dialogue")]
    public static void MakeNewDialogueMenuItem()
    {
        CreateAssets.CreateScriptableObjectAsset<DialogueContainer>("New Dialogue", "Dialogues");
    }
    
    private static DialogueContainer GetFirstOrNewDialogue()
    {
        CreateAssets.CreateFolders();

        var dialogueContainers = FindAssets.GetAllInstances<DialogueContainer>();
        if (dialogueContainers.Any())
            return dialogueContainers.First();

        AssetDatabase.CreateAsset(CreateInstance<DialogueContainer>(), $"Assets/Resources/Dialogues/New Dialogue.asset");
        AssetDatabase.SaveAssets();

        dialogueContainers = FindAssets.GetAllInstances<DialogueContainer>();
        
        return dialogueContainers.First();
    }
    
    public void CreateGUI()
    {
        var root = rootVisualElement;
        
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/DialogueGraphEditor.uxml");
        visualTree.CloneTree(root);
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialoguePlugin/Editor/DialogueGraphEditor.uss");
        root.styleSheets.Add(styleSheet);

        _graphView = root.Q<DialogueGraphView>();
        _graphView.Initialize(this);
        _inspectorView = root.Q<InspectorView>();

        var runTestButton = root.Q<Button>("TestVariables");
        if (runTestButton is not null)
        {
            runTestButton.clickable.clicked += GetListeners.TestVariableObjectConnections;
        }
        
        var container = Selection.activeObject as DialogueContainer;
        _graphView.Container = container ? container : GetFirstOrNewDialogue();
        
        UpdateContainer(_graphView.Container);
        
        AssetDatabase.SaveAssets();
    }
    
    private void OnSelectionChange()
    {
        var container = Selection.activeObject as DialogueContainer;
        if (!container) 
            return;
        
        UpdateContainer(container);
    }

    private void UpdateContainer(DialogueContainer container)
    {
        _graphView.PopulateView(container);
        _inspectorView.UpdateSelection(container);
    }
}