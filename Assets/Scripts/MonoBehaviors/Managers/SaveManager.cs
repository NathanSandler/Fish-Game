using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public static class SaveManager
{
    [Serializable]
    public struct saveStats 
    {
        public int fishKilled;
        public int highestDmg;
    }

    private static readonly string rootDirectory = Application.persistentDataPath + "/Saves";
    private const string extension = ".dat";

    private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(saveStats));

    static string FileLocation(string worldName) => rootDirectory + "/" + worldName + extension;
    public static async Task<bool> LoadSave(string worldName)
    {
        if (!Directory.Exists(rootDirectory)) return false;
        if (!File.Exists(FileLocation(worldName))) return false;
        FileStream fs = File.OpenRead(FileLocation(worldName));
        byte[] bytes = new byte[1024];
        var readAsync = await fs.ReadAsync(bytes, 0, bytes.Length);
        fs.Close();
        return true;
    }

    public static async Task SaveGame(string worldName)
    {
        if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);
        if (File.Exists(FileLocation(worldName))) File.Delete(FileLocation(worldName));
        FileStream fs = File.OpenWrite(FileLocation(worldName)); // Open the file
        byte[] buffer = Encoding.UTF8.GetBytes("Bye!");
        await fs.WriteAsync(buffer, 0, buffer.Length); ;
        // Closes the file
        await fs.DisposeAsync();
        fs.Close();
    }
}
