using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class VariableObject : ScriptableObject
{
    public UnityEvent valueChanged;

#if UNITY_EDITOR
    
    protected static bool CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Variables"))
            AssetDatabase.CreateFolder("Assets/Resources", "Variables");
        
        return true;
    }

    protected static void CreateVariable<T>(string variableType) where T : ScriptableObject
    {
        var variable = //FindAssets.GetInstanceByName<T>($"New {variableType} Variable*");//.First();
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<T>($"New {variableType} Variable*", "/Variables", true);

        int i = 0;
        if (variable == null)
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(),
                $"Assets/Resources/Variables/New {variableType} Variable.asset");
            return;
        }
        
        while (variable != null && i != 100)
        {
            i++;
            
            if (i == 100)
                Debug.LogError($"New variable creation eject point reached. \n Change existing variable names.");

            variable = //FindAssets.GetInstanceByName<T>($"New {variableType} Variable {i}*"); //.First();
            FindAndLoadResource.FindAndLoadFirstInResourceFolder<T>($"New {variableType} Variable {i}*","/Variables", true);
        }
        
        AssetDatabase.CreateAsset(CreateInstance<T>(), $"Assets/Resources/Variables/New {variableType} Variable {i}.asset");
    }
    
#endif
}
