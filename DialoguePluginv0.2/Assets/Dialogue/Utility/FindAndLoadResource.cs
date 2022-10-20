using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class FindAndLoadResource
{
    public static T FindAndLoadFirstInResourceFolder<T>(string fileName, string folder = null) where T : Object
    {
        string toResource = Application.dataPath + "/Resources";

        var files= folder == null ? Directory.GetFiles(toResource + folder, fileName, SearchOption.AllDirectories) : 
            Directory.GetFiles(toResource, fileName, SearchOption.AllDirectories);
        
        //Debug.Log($"{toResource + folder} & contains : {fileName}");
        
        if (files.Length <= 0)
        {
            Debug.LogError($"No {typeof(T).Name} were found");
            return null;
        }

        List<string> notMetaFiles = new List<string>();
        foreach (var path in files)
        {
            string[] tempFilePath = path.Split('.');
            
            if (tempFilePath.Last() != "meta")
                notMetaFiles.Add(path);
        }
        
        string fullFilePath = notMetaFiles.First().Replace('\\', '/');
        string filePathoid = fullFilePath.Remove(0, toResource.Length + 1);
        string[] filePath = filePathoid.Split('.');

        T tempScene = Resources.Load<T>($"{filePath.First()}");
        if (tempScene != null) return tempScene;
        
        Debug.LogError($"No {typeof(T).Name} were found");
        return null;

    }
}
