using System;
using System.Diagnostics;
using System.IO;

public class Notes
{
    private readonly string path;
    public Notes(string fileName = null)
    {
        path = Path.Combine(UnityEngine.Application.temporaryCachePath, $"{fileName ?? Guid.NewGuid().ToString()}.txt");
        File.WriteAllText(path, ""); 
        Process.Start("code", path);
    }

    public void Set(string text)
    {
        File.WriteAllText(path, text);
    }
}