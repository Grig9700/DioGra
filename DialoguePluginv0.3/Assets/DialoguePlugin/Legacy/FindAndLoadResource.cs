using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class FindAndLoadResource
{
    public static T FindAndLoadFirstInResourceFolder<T>(string fileName, string folder = null, bool suppressErrorMessages = false) where T : Object
    {
        var toResource = Application.dataPath + "/Resources";

        var files= folder == null ? Directory.GetFiles(toResource + folder, fileName, SearchOption.AllDirectories) : 
            Directory.GetFiles(toResource, fileName, SearchOption.AllDirectories);
        
        //Debug.Log($"{toResource + folder} & contains : {fileName}");
        
        if (files.Length <= 0)
        {
            if (suppressErrorMessages)
                return null;
            Debug.LogError($"No {typeof(T).Name} were found");
            return null;
        }

        var notMetaFiles = (from path in files let tempFilePath = path.Split('.') where tempFilePath.Last() != "meta" select path).ToList();
        
        var fullFilePath = notMetaFiles.First().Replace('\\', '/');
        var filePathoid = fullFilePath.Remove(0, toResource.Length + 1);
        var filePath = filePathoid.Split('.');

        var tempScene = Resources.Load<T>($"{filePath.First()}");
        if (tempScene != null) return tempScene;
        
        if (suppressErrorMessages)
            return null;
        Debug.LogError($"No {typeof(T).Name} were found");
        return null;
    }
}
