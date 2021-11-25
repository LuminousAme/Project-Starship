using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

public static class DifficultyMod 
{
    ///DLL INTERFACE///

    //Import functions from the plugin
    [DllImport("SimpleDifficultyEditor")]
    private static extern void LoadData(string fileName);

    [DllImport("SimpleDifficultyEditor")]
    private static extern float GetMultiplierByName(string settingName);

    ///THIS SCRIPT TO THE REST OF PROJECT INTERFACE///
    public static readonly string file = Application.persistentDataPath + "/DifficultySettings.txt";

    private static bool isDirty = true;

    //function to lookup what the multiplier for a given difficulty setting should be by name, returns 1 if it finds nothing
    public static float GetMultiplier(string settingName)
    {
        //first try if the plugin has these functions and it works
        try
        {
            //only bother reading the file once it's acutally being used, and only read it once
            if (isDirty)
            {
                LoadData(file);
                Debug.Log(file);
                isDirty = false;
            }

            //once the data is loading do the lookup in C++ and return the value it generates
            float setting = GetMultiplierByName(settingName);
            Debug.Log(settingName + ": " + setting);
            return setting;
        }
        //if it doesn't then just return 1, the standard difficulty
        catch
        {
            return 1.0f;
        }
    }
}
