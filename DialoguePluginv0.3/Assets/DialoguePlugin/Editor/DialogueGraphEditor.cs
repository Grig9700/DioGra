using System.Linq;
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
        
        var dialogueContainer = //FindAssets.GetInstanceByName<DialogueContainer>("New Dialogue*").First();
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*", "/Dialogues", true);

        var i = 0;
        if (dialogueContainer != null)
        {
            while (dialogueContainer != null && i != 100)
            {
                i++;
                
                if (i == 100)
                    Debug.LogError($"New dialog creation eject point reached. \n Change existing dialogue names.");
                
                dialogueContainer = //FindAssets.GetInstanceByName<DialogueContainer>($"New Dialogue {i}*").First();
                    FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>($"New Dialogue {i}*","/Dialogues", true);
            }
        }
        
        AssetDatabase.CreateAsset(CreateInstance<DialogueContainer>(), $"Assets/Resources/Dialogues/New Dialogue {i}.asset");
    }
    
    private DialogueContainer MakeNewDialogue()
    {
        if (!_foldersExist)
            _foldersExist = CreateFolders();
        
        var dialogueContainer = //FindAssets.GetInstanceByName<DialogueContainer>("New Dialogue*").First();
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<DialogueContainer>("New Dialogue*", null, true);
        
        if (dialogueContainer != null) return dialogueContainer;
        
        AssetDatabase.CreateAsset(CreateInstance<DialogueContainer>(), $"Assets/Resources/Dialogues/New Dialogue.asset");
            
        dialogueContainer = //FindAssets.GetInstanceByName<DialogueContainer>("New Dialogue*").First();
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

        _graphView.Container = MakeNewDialogue();
        
        _graphView.PopulateView(_graphView.Container);
        
        AssetDatabase.SaveAssets();
    }
    
    private void OnSelectionChange()
    {
        var container = Selection.activeObject as DialogueContainer;
        if (!container) 
            return;
        
        _graphView.PopulateView(container);
        _inspectorView.UpdateSelection(container);
    }
}