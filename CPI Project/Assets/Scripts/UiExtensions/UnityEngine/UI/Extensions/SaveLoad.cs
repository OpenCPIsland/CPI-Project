using System;
using System.IO;

namespace UnityEngine.UI.Extensions
{
    public static class SaveLoad
    {
        public static string saveGamePath = Application.persistentDataPath + "/Saved Games/";

        public static void Save(SaveGame saveGame)
        {
            CheckPath(saveGamePath);
            string json = JsonUtility.ToJson(saveGame, true);
            File.WriteAllText(saveGamePath + saveGame.savegameName + ".sav", json);
            Debug.Log("Saved Game: " + saveGame.savegameName);
        }

        public static SaveGame Load(string gameToLoad)
        {
            string filePath = saveGamePath + gameToLoad + ".sav";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SaveGame saveGame = JsonUtility.FromJson<SaveGame>(json);
                Debug.Log("Loaded Game: " + saveGame.savegameName);
                return saveGame;
            }

            Debug.Log(gameToLoad + " does not exist!");
            return null;
        }

        private static void CheckPath(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Debug.Log("The directory was created successfully at " + path);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("The process failed: " + ex.ToString());
            }
            finally
            {
            }
        }
    }
}