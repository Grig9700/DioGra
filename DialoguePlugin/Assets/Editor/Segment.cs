using System;
using UnityEngine;
using UnityEditor;

public class Segment : EditorWindow
{
    public Rect SegmentSize = new Rect(0, 0, 200, 150);
    public int id;

    public Texture buttonTexture;
    //public SegmentType SegmentType;

    public virtual void OnDraw()
    {
        
    }

    // protected void MouseIsInside(Event currentEvent, EditorWindow window)
    // {
    //     
    // }
}
