using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;

public class DialogueEditorWindow : EditorWindow
{
    private Dictionary<int, Segment> segments = new Dictionary<int, Segment>();
    private int _lastID = 0;
    private Event current;

    //Creates a menu item for the window
    [MenuItem("Window/Dialogue")]
    public static void ShowWindow()
    {
        GetWindow<DialogueEditorWindow>("Dialogue");
    }
    
    private void OnGUI()
    {
        current = Event.current;
        RightClickDropdownMenu();
        
        BeginWindows();
        foreach (KeyValuePair<int, Segment> segment in segments)
        {
            switch (segment.Value.SegmentType)
            {
                case SegmentType.Dialogue:
                    segment.Value.SegmentSize = GUI.Window(segment.Key, segment.Value.SegmentSize, DialogueSegment.DiaSegment, "Dialogue");
                    break;
                case SegmentType.CallFunction:
                    break;
                case SegmentType.MultipleChoice:
                    break;
                case SegmentType.RandomizeInt:
                    break;
                case SegmentType.Null:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        EndWindows();
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
        dSegment.SegmentType = SegmentType.Dialogue;
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