using System;
using System.IO;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public static class SaveSystem
    {
        private static SaveGameData _saveGameData = new SaveGameData();
        
        public static void SaveGameData(float currentScore)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "SaveGameData.json");
    
            SaveGameData data = GetSaveGameData();
    
            if (currentScore > data.BestScore)
            {
                data.BestScore = currentScore;
        
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(filePath, json);
            }
        }

        public static SaveGameData GetSaveGameData()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "SaveGameData.json");
            
            if (File.Exists(filePath))
            {
                _saveGameData = JsonUtility.FromJson<SaveGameData>(File.ReadAllText(filePath));
            }
            
            return _saveGameData;
        }
    }

    [Serializable]
    public class SaveGameData
    {
        public float BestScore;
    }
}
