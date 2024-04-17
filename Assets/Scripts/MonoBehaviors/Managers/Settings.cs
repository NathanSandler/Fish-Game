using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    static Settings()
    {
        ResetSettings();
    }

    // Retrieval/Interface. Allows for changes to the data
    public static float mouseSens { get => _mouseSens; set {
            _mouseSens = value;
            PlayerPrefs.SetFloat(mouseSensID, _mouseSens);
        } }
    // Cached value. The saved data
    private static float _mouseSens;
    // ID to tell what to save to
    private const string mouseSensID = "mSense";
    
    public static void LoadSettings()
    {
        _mouseSens = PlayerPrefs.GetFloat(mouseSensID);
    }

    public static void ResetSettings()
    {
        _mouseSens = 1f;
    }

    public static void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}
