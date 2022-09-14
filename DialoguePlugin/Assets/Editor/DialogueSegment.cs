using System;
using UnityEngine;
using UnityEditor;

public class DialogueSegment : Segment
{
    public string speakerName; //{get => speakerName; set => speakerName = value; }
    public string text;
    
    private Vector2 scroll;
    
    public override void OnDraw()
    {
        //BeginWindows();
        SegmentSize = GUI.Window(id, SegmentSize, DiaSegment, "Dialogue");
        //EndWindows();
    }

    private void DiaSegment(int meID)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(buttonTexture, GUILayout.Width(10), GUILayout.Height(10)))
        {
            
        }
        GUILayout.Label("In");
        GUILayout.Space(120);
        GUILayout.Label("Out");
        if (GUILayout.Button(buttonTexture, GUILayout.Width(10), GUILayout.Height(10)))
        {
            
        }
        GUILayout.EndHorizontal();
        
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        GUILayout.Space(-140);
        speakerName = GUILayout.TextField(speakerName);
        GUILayout.EndHorizontal();
        
        scroll = EditorGUILayout.BeginScrollView(scroll);
        text = EditorGUILayout.TextArea(text, GUILayout.Height(position.height - 30));
        EditorGUILayout.EndScrollView();
        
        GUI.DragWindow();
    }
}
