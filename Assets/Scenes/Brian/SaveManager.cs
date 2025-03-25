using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveManager : MonoBehaviour
{
    RaceLineData Data;                                                                  // Local saved data

    public void SetCheckPoints(RaceLineData line)
    {
        line.locations.Add(transform.position);                                         // Save Location in line data 
    }

    public void SaveRaceLinesAsync(List<Vector3> raceLineLocations)               // Save the data in a new gameobject
    {                                              
        try
        {
            #if UNITY_EDITOR
            // Editor-specific saving using ScriptableObject
            string saveFolder = Path.Combine(Application.dataPath, "SavedData");        // Save Location folder
            string assetPath = Path.Combine("Assets", "SavedData", Data.name+".asset"); // Set asset save path and name

            // if folder does not exist create the folder
            if (!Directory.Exists(saveFolder))                                          // Look it directory exist 
            {
                Directory.CreateDirectory(saveFolder);                                  // Create directory if not exist
            }

            // Create or load the ScriptableObject
            RaceLineData data;
            if (File.Exists(assetPath))
            {
                data = AssetDatabase.LoadAssetAtPath<RaceLineData>(assetPath);          // Gather the scriptible object
            }
            else
            {
                data = ScriptableObject.CreateInstance<RaceLineData>();                 // Create new scriptible object
                AssetDatabase.CreateAsset(data, assetPath);                             // Create the data in assetpath                                        
            }

            // Update the data
            data.locations = new List<Vector3>(raceLineLocations);                      // Translate all data to new vector3 list

            // Mark the asset as dirty and save
            EditorUtility.SetDirty(data);                                               // Make scriptible object editible
            AssetDatabase.SaveAssets();                                                 // Save data in scriptible object
            AssetDatabase.Refresh();                                                    // Refresh scriptible object (cant be addited)       

            Debug.Log("Saved ScriptableObject to " + assetPath);
            #endif
        }
        catch (Exception ex)
        {                                                                               // Error debug
            Debug.LogError("Save failed: " + ex.Message);
        }
    }

    public void OnSaveButtonClicked(RaceLineData line)                            // preset the data for async
    {
        Debug.Log("Attempting to save race line");
        Data = line;                                                                    // Save line data in manager script
        Data.name = "RaceLineData_" + Data.locations.Count;                             // Change name of raceline to correct line data
        SaveRaceLinesAsync(line.locations);                                       // Pass the locations list
    }

    public RaceLineData LoadFirstRaceLine()                                             // Get Data in file path
    {
        #if UNITY_EDITOR
        string directoryPath = "Assets/SavedData";                                      // File load location

        // Ensure the directory path exist
        if (!AssetDatabase.IsValidFolder(directoryPath))                                // Look it directory exist
        {
            Debug.LogWarning("Invalid directory path: " + directoryPath);
            return null;                                                                // Return NULL (directory not found)
        }

        // Try to load the specific asset saved by SaveManager
        string assetPath = Path.Combine(directoryPath, "RaceLineData_.asset");              // Get asset path
        RaceLineData raceLine = AssetDatabase.LoadAssetAtPath<RaceLineData>(assetPath);     // Get asset data and save local in script 

        if (raceLine != null)
        {
            Debug.Log("Loaded RaceLineData from: " + assetPath);
            return raceLine;                                                                // Return local data to request
        }
        else
        {
            Debug.LogWarning("No RaceLineData found at: " + assetPath);
            return null;                                                                     // Return NULL to request (not data found)
        }
        #endif
    }
}