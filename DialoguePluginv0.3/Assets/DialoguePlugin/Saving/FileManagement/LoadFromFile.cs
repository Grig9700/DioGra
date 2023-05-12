using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LoadFromFile
{
    public static DialogueGameData Load(string fileName, string fileDirectory, bool useEncryption)
    {
        string fullPath = Path.Combine(fileDirectory, fileName);
        
        DialogueGameData dialogueGameData = null;

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"No save by this name was found: {fullPath}");
            return dialogueGameData;
        }

        try
        {
            string dataToLoad = "";
            
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            
            if (useEncryption)
            {
                //Implement Decryption Here
            }

            dialogueGameData = JsonUtility.FromJson<DialogueGameData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occured when trying to load from file: {fullPath} \n {e}");
            throw;
        }
        
        return dialogueGameData;
    }
}
