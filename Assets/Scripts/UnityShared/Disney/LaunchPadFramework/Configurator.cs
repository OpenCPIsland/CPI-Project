using Disney.LaunchPadFramework.Utility;
using Disney.MobileNetwork;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class Configurator
	{
		public class ConfigurationFileInfo
		{
			public string FileName;

			public bool IsResourceFile;

			public bool IsEncrypted;

			public string EncryptionKey;
		}

		public class ConfigurableSystem
		{
			public string ConfigurationFile;

			public string SchemaFile;

			public string SystemName;

			public IDictionary<string, object> Dictionary;
		}

		public delegate List<ConfigurableSystem> ConfigurableSystemDiscoverer(Configurator configurator);

		private const string kConfigurationDir = "Configuration";

		private const string kResourcesDir = "Resources";

		private const string kDefaultSystemConfigurationFileSuffix = "defaultConfig.txt";

		protected IDictionary<string, object> mDictionary = null;

		protected List<ConfigurableSystemDiscoverer> mDiscoverers = null;

		protected string mConfigurationPath;

		protected static List<ConfigurationFileInfo> mConfigurationFilesByPriority;

		protected static string[] mUnitTestPaths;

		static Configurator()
		{
			mConfigurationFilesByPriority = new List<ConfigurationFileInfo>
			{
				new ConfigurationFileInfo
				{
					FileName = "DeviceConfig.txt",
					IsResourceFile = false,
					IsEncrypted = true
				},
				new ConfigurationFileInfo
				{
					FileName = "LocalConfig.txt",
					IsResourceFile = true,
					IsEncrypted = false
				},
				new ConfigurationFileInfo
				{
					FileName = "ApplicationConfig.txt",
					IsResourceFile = true,
					IsEncrypted = true
				}
			};
			mUnitTestPaths = new string[2]
			{
				"/UnitTest/",
				"/_UnitTest/"
			};
			string defaultConfigurationSubdirectory = GetDefaultConfigurationSubdirectory();
			for (int i = 0; i < mConfigurationFilesByPriority.Count; i++)
			{
				mConfigurationFilesByPriority[i].EncryptionKey = EncryptionHelper.GenerateHash(defaultConfigurationSubdirectory + mConfigurationFilesByPriority[i].FileName);
			}
		}

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
			if (text == null)
			{
				text = File.ReadAllText(filePath);
			}
			string text2 = null;
			if (configFileInfo.IsEncrypted)
			{
				try
				{
					text2 = AesCipher.Decrypt(text, configFileInfo.EncryptionKey);
				}
				catch (Exception)
				{
				}
			}
			if (text2 == null)
			{
				text2 = text;
			}
			if (text2 != null)
			{
				result = (LPFJsonMapper.ToObjectSimple(text2) as IDictionary<string, object>);
			}
			return result;
		}

		public IDictionary<string, object> GetDictionary()
		{
			return mDictionary;
		}

		public IDictionary<string, object> GetDictionaryForSystem(Type systemType)
		{
			return GetDictionaryForSystem(systemType.FullName);
		}

		public IDictionary<string, object> GetDictionaryForSystem(string systemName)
		{
			if (!mDictionary.ContainsKey("Systems"))
			{
				return null;
			}
			if (!mDictionary["Systems"].AsDic().ContainsKey(systemName))
			{
				return null;
			}
			return mDictionary["Systems"].AsDic()[systemName].AsDic();
		}

		public void SetConfigurationPath(string configurationPath)
		{
			mConfigurationPath = configurationPath;
		}

		public static string GetDefaultConfigurationPath()
		{
			return (Application.dataPath ?? Directory.GetCurrentDirectory()) + "/Generated" + GetDefaultConfigurationSubdirectory();
		}

		protected static string GetDefaultConfigurationSubdirectory()
		{
			return "/Resources/Configuration/";
		}

		public string GetConfigurationPath()
		{
			return mConfigurationPath;
		}

		public static List<ConfigurationFileInfo> GetConfigurationFilesByPriority()
		{
			return mConfigurationFilesByPriority;
		}

		public bool IsSystemEnabled(string systemName)
		{
			IDictionary<string, object> dictionaryForSystem = GetDictionaryForSystem(systemName);
			object value = null;
			if (dictionaryForSystem != null && dictionaryForSystem.SafeTryGetValue("enabled", out value))
			{
				return (bool)value;
			}
			return true;
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

		private void SetSystemEnabled(string systemName, bool enabled)
		{
			IDictionary<string, object> dictionaryForSystem = GetDictionaryForSystem(systemName);
			object value = null;
			if (dictionaryForSystem.SafeTryGetValue("enabled", out value))
			{
				value = enabled;
			}
			else
			{
				dictionaryForSystem.AsDic().Add("enabled", enabled);
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

		public List<ConfigurationFileInfo> GetValidConfigurationFiles()
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

		protected void CreateConfigurationFile(ConfigurationFileInfo info)
		{
			string fileName = info.FileName;
			List<ConfigurableSystem> list = DiscoverAllConfigurableSystems();
			foreach (ConfigurableSystem item in list)
			{
				InsertNewSystemIntoDictionary(item.SystemName, item.Dictionary);
			}
			if (!Directory.Exists(GetConfigurationPath()))
			{
			}
			SerializeToFile(GetConfigurationPath() + fileName, mDictionary, info);
		}

		protected void SerializeToFile(string filePath, object obj, ConfigurationFileInfo info)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			StringWriter stringWriter = new StringWriter();
			JsonWriter jsonWriter = new JsonWriter(stringWriter);
			jsonWriter.Validate = false;
			jsonWriter.PrettyPrint = true;
			JsonMapper.ToJson(obj, jsonWriter);
			string text = stringWriter.ToString();
			string contents = info.IsEncrypted ? AesCipher.Encrypt(text, info.EncryptionKey) : text;
			File.WriteAllText(filePath, contents);
		}

		protected void SetDictionaryForSystem(string systemName, IDictionary<string, object> dictionary)
		{
			IDictionary<string, object> orAddDic = mDictionary.GetOrAddDic("Systems").GetOrAddDic(systemName);
			MergeInDictionary(orAddDic, dictionary);
		}

		protected void InsertNewSystemIntoDictionary(string systemName, IDictionary<string, object> systemDictionary)
		{
			systemDictionary.Remove("system");
			SetDictionaryForSystem(systemName, systemDictionary);
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
