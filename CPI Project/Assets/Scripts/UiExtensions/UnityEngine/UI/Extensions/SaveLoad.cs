using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnityEngine.UI.Extensions
{
	public static class SaveLoad
	{
		public static string saveGamePath = Application.persistentDataPath + "/Saved Games/";

		public static void Save(SaveGame saveGame)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			SurrogateSelector ss = new SurrogateSelector();
			AddSurrogates(ref ss);
			binaryFormatter.SurrogateSelector = ss;
			CheckPath(saveGamePath);
			FileStream fileStream = File.Create(saveGamePath + saveGame.savegameName + ".sav");
			binaryFormatter.Serialize(fileStream, saveGame);
			fileStream.Close();
			Debug.Log("Saved Game: " + saveGame.savegameName);
		}

		public static SaveGame Load(string gameToLoad)
		{
			if (File.Exists(saveGamePath + gameToLoad + ".sav"))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				SurrogateSelector ss = new SurrogateSelector();
				AddSurrogates(ref ss);
				binaryFormatter.SurrogateSelector = ss;
				FileStream fileStream = File.Open(saveGamePath + gameToLoad + ".sav", FileMode.Open);
				SaveGame saveGame = (SaveGame)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				Debug.Log("Loaded Game: " + saveGame.savegameName);
				return saveGame;
			}
			Debug.Log(gameToLoad + " does not exist!");
			return null;
		}

		private static void AddSurrogates(ref SurrogateSelector ss)
		{
			Vector2Surrogate surrogate = new Vector2Surrogate();
			ss.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), surrogate);
			Vector3Surrogate surrogate2 = new Vector3Surrogate();
			ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), surrogate2);
			Vector4Surrogate surrogate3 = new Vector4Surrogate();
			ss.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All), surrogate3);
			ColorSurrogate surrogate4 = new ColorSurrogate();
			ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), surrogate4);
			QuaternionSurrogate surrogate5 = new QuaternionSurrogate();
			ss.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), surrogate5);
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
