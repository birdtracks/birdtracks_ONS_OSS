using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BirdTracks.Game.Core;
using UnityEngine;

namespace Birdtracks.Game.ONS
{
    /// <summary>
    /// Saves data related to the ONS_TapToAnswer story games
    /// </summary>
    public class SaveTapDataUtil
    {
        private static string _saveFileName = "";
        private static GameDataCollection inMemoryCollection = new GameDataCollection();

        public static void CreateNewFile(string fileName)
        {
            string versionNumber = Application.version;
            _saveFileName = fileName + versionNumber +".json";
        }
        public static void AddGameData(GameData newData)
        {
            inMemoryCollection.entries.Add(newData);
            Debug.Log($"Added new entry to memory: Game: {newData.gameName}, Question: {newData.question}");
        }

        public static void RemoveGameData(int index)
        {
            if (index >= 0 && index < inMemoryCollection.entries.Count)
            {
                inMemoryCollection.entries.RemoveAt(index);
                Debug.Log($"Removed entry at index: {index}");
            }
            else
            {
                Debug.LogError($"Invalid index: {index}");
            }
        }

        public static List<GameData> GetAllGameData()
        {
            return inMemoryCollection.entries;
        }

        public static void UpdateGameData(int index, GameData updatedData)
        {
            if (index >= 0 && index < inMemoryCollection.entries.Count)
            {
                inMemoryCollection.entries[index] = updatedData;
                Debug.Log($"Updated entry at index: {index}");
            }
            else
            {
                Debug.LogError($"Invalid index for update: {index}");
            }
        }

        public static void SaveAllData()
        {
            string savePath = Path.Combine(Application.persistentDataPath, _saveFileName);

            try
            {
                string jsonData = JsonUtility.ToJson(inMemoryCollection);
                File.WriteAllText(savePath, jsonData);
                Debug.Log($"All game data saved successfully to: {savePath}");
                Debug.Log($"StoryResults: {jsonData}");
                
                VADAnalytics.CreateNewGameCompleteEvent(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving game data: {e.Message}");
            }
        }

        public static void LoadSavedData()
        {
            string loadPath = Path.Combine(Application.persistentDataPath, _saveFileName);

            if (!File.Exists(loadPath))
            {
                Debug.Log("No save file found. Starting with empty collection.");
                inMemoryCollection = new GameDataCollection();
                return;
            }

            try
            {
                string jsonData = File.ReadAllText(loadPath);
                inMemoryCollection = JsonUtility.FromJson<GameDataCollection>(jsonData);
                Debug.Log($"Loaded {inMemoryCollection.entries.Count} entries from save file.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game data: {e.Message}");
                inMemoryCollection = new GameDataCollection();
            }
        }

        public static void ClearAllData()
        {
            inMemoryCollection = new GameDataCollection();
            Debug.Log("Cleared all in-memory data");
        }

        public static void DeleteSaveFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, _saveFileName);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Debug.Log("Save file deleted successfully.");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deleting save file: {e.Message}");
                }
            }
        }
    }

    [Serializable]
    public class GameDataCollection
    {
        public List<GameData> entries = new List<GameData>();
    }

    [Serializable]
    public class GameData
    {
        public string gameName;
        public string question;
        public string selectedAnswer;
        public bool answeredCorrectly;
    }
}