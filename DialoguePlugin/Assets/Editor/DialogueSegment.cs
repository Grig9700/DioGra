using System;
using UnityEngine;
using UnityEditor;

public class DialogueSegment : Segment
{
    private void OnGUI()
    {
        BeginWindows();
        GUI.Window(id, SegmentSize, DiaSegment, "Dialogue");
        EndWindows();
    }
    
    public static void DiaSegment(int meID)
    {
        GUILayout.Button("Hi");
        GUI.DragWindow();
    }
}
