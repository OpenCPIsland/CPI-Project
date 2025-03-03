using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.Manifest
{
	[Serializable]
	public class ContentManifestDirectory
	{
		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public static readonly string EMBEDDED_CONTENT = "embedded";

		public ContentManifestDirectoryEntry[] directory;

		public ContentManifestDirectoryEntry FindEmbeddedEntry(DateTimeOffset targetDate, string clientVersion, string clientPlatform)
		{
			for (int i = 0; i < directory.Length; i++)
			{
				ContentManifestDirectoryEntry contentManifestDirectoryEntry = directory[i];
				if (contentManifestDirectoryEntry.content == EMBEDDED_CONTENT)
				{
					return contentManifestDirectoryEntry;
				}
			}
			Debug.LogError("FindEmbeddedEntry(): NO Embedded ContentManifestDirectoryEntry Found!");
			return null;
		}

		public ContentManifestDirectoryEntry FindEntry(DateTimeOffset targetDate, Version clientVersion, string clientPlatform, string environment)
		{
			return FindEntry(targetDate, clientVersion.ToString(), clientPlatform, environment);
		}

		public ContentManifestDirectoryEntry FindEntry(DateTimeOffset targetDate, string clientVersion, string clientPlatform, string environment)
		{
			Debug.LogFormat("FindEntry(): targetDate={0}, clientVersion={1}, clientPlatform={2}, environment={3}", targetDate, clientVersion, clientPlatform, environment);
			if (directory == null)
			{
				Log.LogError(this, "directory is null. There are no client entries to find.");
				return null;
			}
			if (directory.Length <= 0)
			{
				Log.LogError(this, "directory length is zero. There are no client entries to find.");
				return null;
			}
			return FindEntry_Prod(clientVersion, clientPlatform);
		}

		public ContentManifestDirectoryEntry FindEntry_Prod(string client_version, string client_platform)
		{
			string environment = Disney.Kelowna.Common.Environment.Environment.PRODUCTION.ToString().ToLower();
			for (int i = 0; i < directory.Length; i++)
			{
				ContentManifestDirectoryEntry contentManifestDirectoryEntry = directory[i];
				if (contentManifestDirectoryEntry.IsEqualTo(client_version, client_platform, environment))
				{
					return contentManifestDirectoryEntry;
				}
			}
			return null;
		}

		public bool DoesNewerVersionExist(DateTimeOffset targetDate, Version currentVersion, string clientPlatform, string environment)
		{
			for (int i = 0; i < directory.Length; i++)
			{
				ContentManifestDirectoryEntry contentManifestDirectoryEntry = directory[i];
				if (!contentManifestDirectoryEntry.IsEqualTo(clientPlatform, environment))
				{
					continue;
				}
				Version v = ClientInfo.ParseClientVersion(contentManifestDirectoryEntry.client);
				if (v > currentVersion)
				{
					DateTimeOffset releaseDateFromEntry = getReleaseDateFromEntry(contentManifestDirectoryEntry);
					if (releaseDateFromEntry <= targetDate)
					{
						return true;
					}
				}
			}
			return false;
		}

		public List<string> GetReleaseNamesUpToAndIncludingDate(DateTimeOffset date, string client_version, string client_platform, string environment)
		{
			List<string> list = new List<string>();
			if (directory == null || directory.Length <= 0)
			{
				return list;
			}
			for (int i = 0; i < directory.Length; i++)
			{
				ContentManifestDirectoryEntry contentManifestDirectoryEntry = directory[i];
				if (contentManifestDirectoryEntry.IsEqualTo(client_version, client_platform, environment) && getReleaseDateFromEntry(contentManifestDirectoryEntry) <= date)
				{
					list.Add(GetReleaseNameFromEntry(contentManifestDirectoryEntry));
				}
			}
			return list;
		}

		public string GetReleaseNameFromEntry(ContentManifestDirectoryEntry entry)
		{
			if (entry != null && entry.content != EMBEDDED_CONTENT && entry.content != null)
			{
				if (entry.content.IndexOf('_') > 0)
				{
					return entry.content.Split('_')[0];
				}
				return entry.content;
			}
			return string.Empty;
		}

		private DateTimeOffset getReleaseDateFromEntry(ContentManifestDirectoryEntry entry)
		{
			if (entry.content == EMBEDDED_CONTENT)
			{
				return DateTimeOffset.MinValue;
			}
			if (!string.IsNullOrEmpty(entry.releaseDate))
			{
				return CommonDateTime.Deserialize(entry.releaseDate);
			}
			string[] array = entry.content.Split('_');
			if (array.Length == 2)
			{
				string dateString = array[1];
				return CommonDateTime.CreateDate(dateString);
			}
			return DateTimeOffset.MinValue;
		}
	}
}
