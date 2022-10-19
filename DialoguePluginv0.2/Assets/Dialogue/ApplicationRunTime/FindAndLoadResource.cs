using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FindAndLoadResource
{
    public static T FindAndLoadFirstInResourceFolder<T>(string folder, string fileName) where T : Object
    {
        string toResource = Application.dataPath + "/Resources/";
        
        var files = Directory.GetFiles(toResource + folder, fileName, SearchOption.AllDirectories);
        
        if (files.Length <= 0)
        {
            Debug.LogError($"No {typeof(T).Name} were found");
            return null;
        }

        string fullFilePath = files.First().Replace('\\', '/');
        string filePathoid = fullFilePath.Remove(0, toResource.Length);
        string[] filePath = filePathoid.Split('.');

        T tempScene = Resources.Load<T>($"{filePath.First()}");
        if (tempScene != null) return tempScene;
        
        Debug.LogError($"No {typeof(T).Name} were found");
        return null;

    }
}
