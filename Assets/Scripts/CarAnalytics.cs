using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarAnalytics : MonoBehaviour
{
    [SerializeField] public List<EpisodeData> epsData = new List<EpisodeData>();
    public static string SAVE_FOLDER;
    [SerializeField] string fullPath;
    [SerializeField] string fileName;
    [SerializeField] public int epCount = -1;

    private void Awake()
    {
        SAVE_FOLDER = Application.dataPath + "/Analytics_Saved/";
        fileName = "DataFrom " + DateTime.Now.ToString("dd--h_mmtt")  + ".json";
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public void AddEpDataTolist(EpisodeData epData)
    {
        epsData.Add(epData);

        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(epsData, Formatting.Indented);
        fullPath = SAVE_FOLDER + fileName;
        File.WriteAllText(fullPath, jsonData);

    }

}
