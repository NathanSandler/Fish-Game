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

    public static string fileLocation(string worldName) => rootDirectory + "/" + worldName + extension;
    public static async Task<bool> LoadSave(string worldName)
    {
        if (!Directory.Exists(rootDirectory)) return false;
        if (!File.Exists(fileLocation(worldName))) return false;
        Debug.Log(fileLocation(worldName));
        FileStream fs = File.OpenRead(fileLocation(worldName));
        byte[] bytes = new byte[1024];
        await fs.ReadAsync(bytes, 0, bytes.Length);
        fs.Close();
        Debug.Log(Encoding.UTF8.GetString(bytes));
        return true;
    }

    public static async Task SaveGame(string worldName)
    {
        if (!Directory.Exists(rootDirectory)) Directory.CreateDirectory(rootDirectory);
        if (File.Exists(fileLocation(worldName))) File.Delete(fileLocation(worldName));
        FileStream fs = File.OpenWrite(fileLocation(worldName)); // Open the file
        byte[] buffer = Encoding.UTF8.GetBytes("Bye!");
        await fs.WriteAsync(buffer, 0, buffer.Length);
        Debug.Log(buffer[0]);
        // Closes the file
        await fs.DisposeAsync();
        fs.Close();
    }
}
