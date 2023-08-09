using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueDocumentEditor : EditorWindow
{
    private DialogueDocumentView _documentView;
    private InspectorView _inspectorView;
    
    [MenuItem("Dialogue Editor/Document Window")]
    public static void CreateDocumentWindow()
    {
        var window = GetWindow<DialogueDocumentEditor>();
        window.titleContent = new GUIContent("DialogueDocument");
    }
    
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject is not DialogueContainer) 
            return false;
        
        CreateDocumentWindow();
        return true;
    }
    
    public void CreateGUI()
    {
        var root = rootVisualElement;
        
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/DialoguePlugin/Editor/DialogueDocumentEditor.uxml");
        visualTree.CloneTree(root);
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/DialoguePlugin/Editor/DialogueDocumentEditor.uss");
        root.styleSheets.Add(styleSheet);

        _documentView = root.Q<DialogueDocumentView>();
        _inspectorView = root.Q<InspectorView>();

        var container = Selection.activeObject as DialogueContainer;
        UpdateContainer(container ? container : DialogueGraphEditor.GetFirstOrNewDialogue());
        
        AssetDatabase.SaveAssets();
    }

    private void UpdateContainer(DialogueContainer container)
    {
        _documentView.PopulateView(container);
        _inspectorView.UpdateSelection(container);
    }
}