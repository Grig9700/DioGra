using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.PackageManager.UI;

public class DialogueEditorWindow : EditorWindow
{
    

    private Dictionary<int, Segment> segments = new Dictionary<int, Segment>();
    private int _lastID = 0;
    private Event current;
    private Texture buttonTexture;
    private Vector2 scroll;

    //Creates a menu item for the window
    [MenuItem("Window/Flowchart")]
    public static void ShowWindow()
    {
        GetWindow<DialogueEditorWindow>("Flowchart");
    }
    
    private void OnGUI()
    {
        GetButtonTexture();
        
        current = Event.current;
        RightClickDropdownMenu();
        
        BeginWindows();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (KeyValuePair<int, Segment> segment in segments)
        {
            segment.Value.OnDraw();
        }
        EditorGUILayout.EndScrollView();
        EndWindows();
    }

    private void GetButtonTexture()
    {
        if (buttonTexture != null) return; 
        buttonTexture = Resources.Load("button") as Texture2D;
    }

    private void RightClickDropdownMenu()
    {
        if (!mouseOverWindow || this != mouseOverWindow || current.type != EventType.ContextClick) return;
        
        GenericMenu menu = new GenericMenu();
        //menu.AddDisabledItem(new GUIContent("I clicked on a thing"));
        menu.AddItem(new GUIContent("Dialogue Segment"), false, CreateDialogueSegment);
        menu.ShowAsContext();
        current.Use();
    }
    
    private void CreateDialogueSegment()
    {
        _lastID++;
        DialogueSegment dSegment = ScriptableObject.CreateInstance<DialogueSegment>();
        dSegment.id = _lastID;
        //dSegment.SegmentType = SegmentType.Dialogue;
        dSegment.buttonTexture = buttonTexture;
        dSegment.SegmentSize = new Rect(current.mousePosition, dSegment.SegmentSize.size);
        segments.Add(_lastID, dSegment);
    }
}

public enum SegmentType
{
    Dialogue,
    RandomizeInt,
    CallFunction,
    MultipleChoice,
    Null
}