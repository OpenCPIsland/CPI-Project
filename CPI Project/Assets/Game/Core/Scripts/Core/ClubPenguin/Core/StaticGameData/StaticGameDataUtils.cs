using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.Core.StaticGameData
{
	public static class StaticGameDataUtils
	{
		private static string CONFIG_PATH = "Configuration/StaticGameData";

		private static string GetConfigPath(Type definitionType)
		{
			return UriUtil.Combine(CONFIG_PATH, string.Format("{0}Config", definitionType.Name));
		}

		private static StaticGameDataDefinitionConfig GetConfig(Type definitionType)
		{
			string configPath = GetConfigPath(definitionType);
			StaticGameDataDefinitionConfig staticGameDataDefinitionConfig = Resources.Load<StaticGameDataDefinitionConfig>(configPath);
			if (staticGameDataDefinitionConfig == null)
			{
				Log.LogErrorFormatted(typeof(StaticGameDataUtils), "No StaticGameDataDefinitionConfig file found in path {0} for static game data type {1}", configPath, definitionType);
			}
			return staticGameDataDefinitionConfig;
		}

		public static string GetDefinitionPath(Type definitionType)
		{
			StaticGameDataDefinitionConfig config = GetConfig(definitionType);
			if (config != null)
			{
				return config.DefinitionPath;
			}
			return "";
		}

		public static string GetExportPath(Type definitionType)
		{
			StaticGameDataDefinitionConfig config = GetConfig(definitionType);
			if (config != null)
			{
				return config.ExportPath;
			}
			return "";
		}

		public static string GetManifestPath(Type definitionType)
		{
			StaticGameDataDefinitionConfig config = GetConfig(definitionType);
			if (config != null)
			{
				return config.ManifestPath;
			}
			return "";
		}

		public static ManifestContentKey GetManifestContentKey(Type definitionType)
		{
			StaticGameDataDefinitionConfig config = GetConfig(definitionType);
			if (config != null)
			{
				return new ManifestContentKey(config.ManifestPath.Split(new string[1]
				{
					"Resources/"
				}, StringSplitOptions.None)[1]);
			}
			return new ManifestContentKey();
		}

		public static bool IsExportable(Type definitionType)
		{
			StaticGameDataDefinitionConfig config = GetConfig(definitionType);
			return config == null || config.IsExportable;
		}

		public static string GetPathFromResources(string manifestPath)
		{
			int startIndex = manifestPath.IndexOf("/", manifestPath.ToLower().IndexOf("resources")) + 1;
			return manifestPath.Substring(startIndex);
		}
	}
}
