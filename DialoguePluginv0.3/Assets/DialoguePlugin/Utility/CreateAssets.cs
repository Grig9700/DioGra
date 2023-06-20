using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CreateAssets
{
    public static void CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Variables"))
            AssetDatabase.CreateFolder("Assets/Resources", "Variables");
        
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues")) 
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
        
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Characters")) 
            AssetDatabase.CreateFolder("Assets/Resources", "Characters");
        
        AssetDatabase.SaveAssets();
    }
    
    public static void CreateScriptableObjectAsset<T>(string name, string directory) where T : ScriptableObject
    {
        CreateFolders();
        
        var variable = FindAssets.GetResourceByName<T>(name, true);
        
        if (variable == null)
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), $"Assets/Resources/{directory}/{name}.asset");
            return;
        }

        var i = 0;
        while (variable != null && i != 100)
        {
            i++;
            
            if (i == 100)
                Debug.LogError($"New variable creation eject point reached. \n Change existing variable names.");

            variable = FindAssets.GetResourceByName<T>($"{name} {i}", true);
        }
        
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), $"Assets/Resources/{directory}/{name} {i}.asset");
        AssetDatabase.SaveAssets();
    }
}
