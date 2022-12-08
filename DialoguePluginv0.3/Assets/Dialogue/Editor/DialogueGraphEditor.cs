using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DialogueGraphEditor : EditorWindow
{
    private DialogueGraphView _graphView;
    private InspectorView _inspectorView;
    private bool _foldersExist;
    
    [MenuItem("Dialogue Graph Editor/Editor...")]
    public static void ShowExample()
    {
        DialogueGraphEditor window = GetWindow<DialogueGraphEditor>();
        window.titleContent = new GUIContent("DialogueGraphEditor");
    }

    [MenuItem("Dialogue Graph Editor/New Dialogue")]
    public static void MakeNewDialogueMenuItem()
    {
        CreateFolders();
        
        var dialogueContainer =
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*", "/Dialogues", true);

        int i = 0;
        if (dialogueContainer != null)
        {
            while (dialogueContainer != null)
            {
                i++;
                dialogueContainer =
                    FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>($"New Dialogue {i}*","/Dialogues", true);
            }
        }
        
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DialogueContainer>(),
            $"Assets/Resources/Dialogues/New Dialogue {i}.asset");
    }
    
    private DialogueContainer MakeNewDialogue()
    {
        if (!_foldersExist)
            _foldersExist = CreateFolders();
        
        var dialogueContainer =
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*", null, true);
        
        if (dialogueContainer != null) return dialogueContainer;
        
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DialogueContainer>(),
            $"Assets/Resources/Dialogues/New Dialogue.asset");
            
        dialogueContainer =
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*", null, true);

        return dialogueContainer;
    }

    private static bool CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues"))
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
        
        return true;
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

        _graphView.Container = MakeNewDialogue();
        
        _graphView.PopulateView(_graphView.Container);
        
        AssetDatabase.SaveAssets();
    }
    
    private void OnSelectionChange()
    {
        DialogueContainer container = Selection.activeObject as DialogueContainer;
        if (container)
        {
            _graphView.PopulateView(container);
            _inspectorView.UpdateSelection(container);
        }
    }
}