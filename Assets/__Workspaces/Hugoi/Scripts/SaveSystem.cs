using System;
using System.IO;
using UnityEngine;

namespace __Workspaces.Hugoi.Scripts
{
    public static class SaveSystem
    {
        private static SaveGameData _saveGameData = new SaveGameData();
        
        public static void SaveGameData(float bestScore)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "SaveGameData.json");
            
            if (File.Exists(filePath))
            {
                _saveGameData = JsonUtility.FromJson<SaveGameData>(File.ReadAllText(filePath));
            }
            
            _saveGameData.BestScore = bestScore;
            
            string json = JsonUtility.ToJson(_saveGameData);
            File.WriteAllText(filePath, json);
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
