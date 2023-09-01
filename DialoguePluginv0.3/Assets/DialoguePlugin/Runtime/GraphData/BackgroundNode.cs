using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundNode : GraphNode
{
    public Sprite background;
    
    public override NodeReturn Run(DialogueManager manager)
    {
        if (background != null)
            manager.Background.style.backgroundImage = new StyleBackground(background);
        
        manager.SetTargetNode(IsNullOrEmpty() ? null : children?.First());
        
        return NodeReturn.Next;
    }

    public override void Clear(){}
}
