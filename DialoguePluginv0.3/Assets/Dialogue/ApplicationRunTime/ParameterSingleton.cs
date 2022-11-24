using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterSingleton : MonoBehaviour
{
    public static ParameterSingleton Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    
}
