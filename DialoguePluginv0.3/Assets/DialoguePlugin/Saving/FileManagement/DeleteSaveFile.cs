

using System.IO;

public static class DeleteSaveFile
{
    public static void Delete(string fileName, string fileDirectory)
    {
        string fullPath = Path.Combine(fileDirectory, fileName);
        
        File.Delete(fullPath);
    }
}