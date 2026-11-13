using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class Configurator
	{
		public struct ConfigurationFileInfo
		{
			public string FileName;

			public bool IsResourceFile;
		}

		public class ConfigurableSystem
		{
			public string ConfigurationFile;

			public string SchemaFile;

			public Type SystemType;

			public IDictionary<string, object> Dictionary;
		}

		public delegate List<ConfigurableSystem> ConfigurableSystemDiscoverer(Configurator configurator);

		private const string kConfigurationDir = "Configuration";

		private const string kResourcesDir = "Resources";

		private const string kDefaultSystemConfigurationFileSuffix = "defaultConfig.txt";

		protected IDictionary<string, object> mDictionary = null;

		protected List<ConfigurableSystemDiscoverer> mDiscoverers = null;

		protected string mConfigurationPath;

		protected static List<ConfigurationFileInfo> mConfigurationFilesByPriority = new List<ConfigurationFileInfo>
		{
			new ConfigurationFileInfo
			{
				FileName = "ClassicMiniGamesDeviceConfig.txt",
				IsResourceFile = false
			},
			new ConfigurationFileInfo
			{
				FileName = "ClassicMiniGamesLocalConfig.txt",
				IsResourceFile = true
			},
			new ConfigurationFileInfo
			{
				FileName = "ClassicMiniGamesApplicationConfig.txt",
				IsResourceFile = true
			}
		};

		protected static string[] mUnitTestPaths = new string[2]
		{
			"/UnitTest/",
			"/_UnitTest/"
		};

		public Configurator()
		{
			mConfigurationPath = GetDefaultConfigurationPath();
		}

		public void Init()
		{
			Init(false);
		}

		public void Init(bool isUnitTest)
		{
			mDictionary = new Dictionary<string, object>();
			List<ConfigurationFileInfo> validConfigurationFiles = GetValidConfigurationFiles();
			mDiscoverers = new List<ConfigurableSystemDiscoverer>();
			mDiscoverers.Add(DefaultConfigurableSystemDiscoverer);
			if (validConfigurationFiles.Count > 0)
			{
				validConfigurationFiles.Reverse();
				foreach (ConfigurationFileInfo item in validConfigurationFiles)
				{
					string configurationFilePath = GetConfigurationFilePath(item);
					IDictionary<string, object> higherPriorityDictionary = LoadFromFile(configurationFilePath, item);
					MergeInDictionary(mDictionary, higherPriorityDictionary);
				}
			}
		}

		protected string GetConfigurationFilePath(ConfigurationFileInfo configFileInfo)
		{
			if (configFileInfo.IsResourceFile)
			{
				return "Configuration/" + Path.GetFileNameWithoutExtension(configFileInfo.FileName);
			}
			return Application.persistentDataPath + "/" + configFileInfo.FileName;
		}

		protected IDictionary<string, object> LoadFromFile(string filePath, ConfigurationFileInfo configFileInfo)
		{
			IDictionary<string, object> result = null;
			string text = null;
			if (configFileInfo.IsResourceFile)
			{
				TextAsset textAsset = Resources.Load(filePath, typeof(TextAsset)) as TextAsset;
				if (textAsset != null)
				{
					text = textAsset.text;
				}
			}
			else
			{
				text = File.ReadAllText(filePath);
			}
			if (text != null)
			{
				result = (JsonMapper.ToObjectSimple(text) as IDictionary<string, object>);
			}
			return result;
		}

		public IDictionary<string, object> GetDictionary()
		{
			return mDictionary;
		}

		public IDictionary<string, object> GetDictionaryForSystem(string systemType)
		{
			return GetDictionaryForSystem(Type.GetType(systemType));
		}

		public IDictionary<string, object> GetDictionaryForSystem(Type systemType)
		{
			if (!mDictionary["Systems"].AsDic().ContainsKey(systemType.FullName))
			{
				Logger.LogWarning(this, "Cannot find system " + systemType.FullName + " in Configuration files.");
				return null;
			}
			return mDictionary["Systems"].AsDic()[systemType.FullName].AsDic();
		}

		public void SetConfigurationPath(string configurationPath)
		{
			mConfigurationPath = configurationPath;
		}

		public static string GetDefaultConfigurationPath()
		{
			return Application.dataPath + "/ClassicMiniGames/Resources/Configuration/";
		}

		public string GetConfigurationPath()
		{
			return mConfigurationPath;
		}

		public static List<ConfigurationFileInfo> GetConfigurationFilesByPriority()
		{
			return mConfigurationFilesByPriority;
		}

		public void GetSystemsStatus(out IDictionary<string, bool> systemAndStatus)
		{
			systemAndStatus = new Dictionary<string, bool>();
			IDictionary<string, object> dictionary = mDictionary["SystemsConfiguration"].AsDic();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				bool value = (bool)item.Value.AsDic()["Enabled"];
				systemAndStatus.Add(item.Key, value);
			}
		}

		public bool IsSystemEnabled(Type systemType)
		{
			return (bool)mDictionary["SystemsConfiguration"].AsDic()[systemType.FullName].AsDic()["Enabled"];
		}

		public void RegisterDiscoverer(ConfigurableSystemDiscoverer discoverer)
		{
			mDiscoverers.Add(discoverer);
		}

		protected static List<ConfigurableSystem> DefaultConfigurableSystemDiscoverer(Configurator configurator)
		{
			return configurator.DiscoverConfigurableSystem("defaultConfig.txt");
		}

		public List<ConfigurableSystem> DiscoverConfigurableSystem(string configurationFileSuffix)
		{
			return new List<ConfigurableSystem>();
		}

		public List<ConfigurableSystem> DiscoverAllConfigurableSystems()
		{
			List<ConfigurableSystem> list = new List<ConfigurableSystem>();
			foreach (ConfigurableSystemDiscoverer mDiscoverer in mDiscoverers)
			{
				list.AddRange(mDiscoverer(this));
			}
			return list;
		}

		private void SetSystemEnabled(Type systemType, bool enabled)
		{
			IDictionary<string, object> orAddDic = mDictionary.GetOrAddDic("SystemsConfiguration").GetOrAddDic(systemType.FullName);
			object value;
			orAddDic.TryGetValue("Enabled", out value);
			if (value == null)
			{
				orAddDic.AsDic().Add("Enabled", enabled);
			}
			else
			{
				value = enabled;
			}
		}

		protected bool DefaultConfigFileIsValid(string filePath)
		{
			bool result = true;
			bool flag = false;
			for (int i = 0; i < mUnitTestPaths.Length; i++)
			{
				flag |= (filePath.IndexOf(mUnitTestPaths[i]) != -1);
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				result = false;
			}
			return result;
		}

		protected List<ConfigurationFileInfo> GetValidConfigurationFiles()
		{
			List<ConfigurationFileInfo> list = new List<ConfigurationFileInfo>();
			foreach (ConfigurationFileInfo item in mConfigurationFilesByPriority)
			{
				bool flag = false;
				string configurationFilePath = GetConfigurationFilePath(item);
				if (item.IsResourceFile)
				{
					TextAsset x = Resources.Load(configurationFilePath, typeof(TextAsset)) as TextAsset;
					flag = (x != null);
				}
				else
				{
					flag = File.Exists(configurationFilePath);
				}
				if (flag)
				{
					list.Add(item);
				}
			}
			return list;
		}

		protected void CreateConfigurationFile(string file)
		{
			List<ConfigurableSystem> list = DiscoverAllConfigurableSystems();
			foreach (ConfigurableSystem item in list)
			{
				InsertNewSystemIntoDictionary(item.SystemType, item.Dictionary);
			}
			SerializeToFile(GetConfigurationPath() + file, mDictionary);
		}

		protected void SerializeToFile(string filePath, object obj)
		{
			StringWriter stringWriter = new StringWriter();
			JsonWriter jsonWriter = new JsonWriter(stringWriter);
			jsonWriter.Validate = false;
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson(obj, jsonWriter);
			File.WriteAllText(filePath, stringWriter.ToString());
		}

		protected void SetDictionaryForSystem(Type systemType, IDictionary<string, object> dictionary)
		{
			IDictionary<string, object> orAddDic = mDictionary.GetOrAddDic("Systems").GetOrAddDic(systemType.FullName);
			MergeInDictionary(orAddDic, dictionary);
		}

		protected void InsertNewSystemIntoDictionary(Type systemType, IDictionary<string, object> systemDictionary)
		{
			systemDictionary.Remove("system");
			SetSystemEnabled(systemType, true);
			SetDictionaryForSystem(systemType, systemDictionary);
		}

		protected void MergeInDictionary(IDictionary<string, object> originalDictionary, IDictionary<string, object> higherPriorityDictionary)
		{
			if (originalDictionary != null && higherPriorityDictionary != null)
			{
				foreach (KeyValuePair<string, object> item in higherPriorityDictionary)
				{
					if (originalDictionary.ContainsKey(item.Key))
					{
						if (item.Value is IDictionary<string, object>)
						{
							MergeInDictionary(originalDictionary[item.Key].AsDic(), item.Value.AsDic());
						}
						else
						{
							originalDictionary[item.Key] = item.Value;
						}
					}
					else
					{
						originalDictionary[item.Key] = item.Value;
					}
				}
			}
		}

		protected void MergeInDictionaryPreserveOldValues(IDictionary<string, object> originalDictionary, IDictionary<string, object> lowerPriorityDictionary)
		{
			if (originalDictionary != null && lowerPriorityDictionary != null)
			{
				foreach (KeyValuePair<string, object> item in lowerPriorityDictionary)
				{
					if (originalDictionary.ContainsKey(item.Key))
					{
						if (item.Value is IDictionary<string, object>)
						{
							MergeInDictionaryPreserveOldValues(originalDictionary[item.Key].AsDic(), item.Value.AsDic());
						}
					}
					else
					{
						originalDictionary[item.Key] = item.Value;
					}
				}
			}
		}
	}
}
