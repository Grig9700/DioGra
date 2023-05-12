using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIButtonController : MonoBehaviour
{
    [SerializeField] public List<ButtonConnection> buttonConnections;

    private VisualElement _root;
    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        foreach (var connection in buttonConnections)
        {
            InitializeButton(_root, connection.buttonName, connection.buttonAction);
        }
    }

    private static void InitializeButton(VisualElement root, string buttonName, UnityEvent buttonAction)
    {
        var button = root.Q<Button>(buttonName);
        
        if (button == null)
            return;
        
        button.clicked += buttonAction.Invoke;
    }

    private void OnDisable()
    {
        foreach (var connection in buttonConnections)
        {
            UnInitializeButton(_root, connection.buttonName, connection.buttonAction);
        }
    }
    
    private static void UnInitializeButton(VisualElement root, string buttonName, UnityEvent buttonAction)
    {
        var button = root.Q<Button>(buttonName);
        
        if (button == null)
            return;
        
        button.clicked -= buttonAction.Invoke;
    }
}

[Serializable]
public struct ButtonConnection
{
    public string buttonName;
    public UnityEvent buttonAction;
}