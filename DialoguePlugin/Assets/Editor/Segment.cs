using UnityEngine;
using UnityEditor;

public class Segment : EditorWindow
{
    public Rect SegmentSize = new Rect(0, 0, 250, 250);
    public int id;
    public SegmentType SegmentType;
}
