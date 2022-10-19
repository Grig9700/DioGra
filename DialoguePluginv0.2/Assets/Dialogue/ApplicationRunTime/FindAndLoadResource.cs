using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FindAndLoadResource
{
    public static T FindAndLoadFirstInResourceFolder<T>(string fileName, string folder = null) where T : Object
    {
        string toResource = Application.dataPath + "/Resources";
        
        //var files = Directory.GetFiles(toResource + folder, fileName, SearchOption.AllDirectories);

        if (!fileName.Contains(".asset"))
            fileName += ".asset";

        var files= folder == null ? Directory.GetFiles(toResource + folder, fileName, SearchOption.AllDirectories) : 
            Directory.GetFiles(toResource, fileName, SearchOption.AllDirectories);
        
        //Debug.Log($"{toResource + folder} & contains : {fileName}");
        
        if (files.Length <= 0)
        {
            Debug.LogError($"No {typeof(T).Name} were found");
            return null;
        }

        string fullFilePath = files.First().Replace('\\', '/');
        string filePathoid = fullFilePath.Remove(0, toResource.Length + 1);
        string[] filePath = filePathoid.Split('.');

        T tempScene = Resources.Load<T>($"{filePath.First()}");
        if (tempScene != null) return tempScene;
        
        Debug.LogError($"No {typeof(T).Name} were found");
        return null;

    }
}
