#define UNITY_ASSERTIONS
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Disney.Kelowna.Common
{
	public class ContentManifest
	{
		[Serializable]
		public struct AssetEntry : IEquatable<AssetEntry>
		{
			public const string URI_DEVICE_LIST_KEY = "dl";

			public const string URI_EXTENSION_KEY = "x";

			public const string URI_BUNDLE_KEY = "b";

			public const string URI_PATH_PREFIX_KEY = "p";

			public const string URI_IS_LOCALIZED_KEY = "l";

			public const string URI_DEFAULT_LOCALE_KEY = "ld";

			public const string URI_CACHE_ONLY_KEY = "cacheOnly";

			public const string URI_DECRYPTIONKEY_KEY = "dk";

			public readonly string Key;

			public readonly string DeviceList;

			public readonly string Extension;

			public readonly string BundleKey;

			public readonly string PathPrefix;

			public readonly bool IsLocalized;

			public readonly string DefaultLocale;

			public readonly bool IsCacheOnly;

			public readonly string DecryptionKey;

			public readonly object UserData;

			public string AssetPath
			{
				get
				{
					return getAssetPath(Key, Extension, PathPrefix);
				}
			}

			public static AssetEntry Parse(string payload)
			{
				int num = payload.IndexOf('?');
				if (num < 0)
				{
					throw new ContentManifestException("URI payload does not contain a ? character\n" + payload);
				}
				string key = payload.Substring(0, num);
				string deviceList = null;
				string extension = null;
				string bundleKey = null;
				string pathPrefix = null;
				bool isLocalized = false;
				string defaultLocale = null;
				bool isCacheOnly = false;
				string decryptionKey = null;
				int num2 = num + 1;
				while (num2 < payload.Length)
				{
					int num3 = payload.IndexOf('=', num2);
					if (num3 != -1)
					{
						string text = payload.Substring(num2, num3 - num2);
						num3++;
						int num4 = payload.IndexOf('&', num3);
						if (num4 == -1)
						{
							num4 = payload.Length;
						}
						string text2 = payload.Substring(num3, num4 - num3);
						num2 = num4 + 1;
						switch (text)
						{
						case "dl":
							deviceList = text2;
							break;
						case "x":
							extension = text2;
							break;
						case "b":
							bundleKey = text2;
							break;
						case "p":
							pathPrefix = text2;
							break;
						case "l":
							isLocalized = (text2 == "true");
							break;
						case "ld":
							defaultLocale = text2;
							break;
						case "cacheOnly":
							isCacheOnly = (text2 == "true");
							break;
						case "dk":
							decryptionKey = text2;
							break;
						}
						continue;
					}
					throw new ContentManifestException("Asset entry URI payload is missing a '=' character after index " + num2 + "\n" + payload);
				}
				return new AssetEntry(key, deviceList, extension, bundleKey, pathPrefix, isLocalized, defaultLocale, isCacheOnly, decryptionKey);
			}

			public static AssetEntry Create(string key, string deviceList, bool isLocalized = false, string defaultLocale = null, bool isCacheOnly = false, string decryptionKey = null)
			{
				Assert.IsNotNull(key);
				Assert.IsNotNull(deviceList);
				key = key.ToLower();
				deviceList = deviceList.ToLower();
				string extension = Path.GetExtension(key).TrimStart('.');
				key = Path.ChangeExtension(key, null);
				return new AssetEntry(key, deviceList, extension, null, null, isLocalized, defaultLocale, isCacheOnly, decryptionKey);
			}

			public AssetEntry(string key, string deviceList, string extension, string bundleKey = null, string pathPrefix = null, bool isLocalized = false, string defaultLocale = null, bool isCacheOnly = false, string decryptionKey = null, object userData = null)
			{
				Key = key;
				DeviceList = deviceList;
				Extension = extension;
				BundleKey = bundleKey;
				PathPrefix = pathPrefix;
				IsLocalized = isLocalized;
				DefaultLocale = defaultLocale;
				IsCacheOnly = isCacheOnly;
				DecryptionKey = decryptionKey;
				UserData = userData;
			}

			[Conditional("NOT_RC_BUILD")]
			public void Validate()
			{
				if (string.IsNullOrEmpty(Key))
				{
					throw new ContentManifestException("Asset entry is missing a key!");
				}
				if (Key.StartsWith("assets/"))
				{
					throw new ContentManifestException("Asset entry key cannot start with assets/");
				}
				if (string.IsNullOrEmpty(DeviceList))
				{
					throw new ContentManifestException("Asset entry '" + Key + "' is missing a device list!");
				}
				if (string.IsNullOrEmpty(Extension))
				{
					throw new ContentManifestException("Asset entry '" + Key + "' is missing an extension!");
				}
				if (Extension[0] == '.')
				{
					throw new ContentManifestException("Asset entry '" + Key + "' has an extension that starts with '.'!");
				}
				if (BundleKey != null)
				{
					if (string.IsNullOrEmpty(BundleKey))
					{
						throw new ContentManifestException("Asset entry '" + Key + "' has an empty bundle key!");
					}
					if (PathPrefix == null)
					{
						throw new ContentManifestException("Asset entry '" + Key + "' has a bundle key but not a path prefix!");
					}
				}
				if (IsLocalized && string.IsNullOrEmpty(DefaultLocale))
				{
					throw new ContentManifestException("Asset entry '" + Key + "' is localized but does not have a default locale!");
				}
			}

			public string ToUri()
			{
				StringBuilder stringBuilder = new StringBuilder("asset:");
				stringBuilder.Append(Key);
				appendParam(stringBuilder, "dl", DeviceList, '?');
				appendParam(stringBuilder, "x", Extension);
				if (!string.IsNullOrEmpty(BundleKey))
				{
					appendParam(stringBuilder, "b", BundleKey);
					appendParam(stringBuilder, "p", PathPrefix);
				}
				if (IsLocalized)
				{
					appendParam(stringBuilder, "l", "true");
					appendParam(stringBuilder, "ld", DefaultLocale);
				}
				if (IsCacheOnly)
				{
					appendParam(stringBuilder, "cacheOnly", "true");
				}
				if (!string.IsNullOrEmpty(DecryptionKey))
				{
					appendParam(stringBuilder, "dk", DecryptionKey);
				}
				return stringBuilder.ToString();
			}

			private static string getPathPrefix(string key, string path)
			{
				Assert.IsTrue(path.EndsWith(key));
				int num = path.StartsWith("assets/") ? "assets/".Length : 0;
				int num2 = path.Length - key.Length;
				Assert.IsTrue(num2 >= num, key + " is shorter then relative " + path);
				int num3 = num2 - num;
				if (num3 > 0)
				{
					num3--;
				}
				return path.Substring(num, num3);
			}

			private static string getAssetPath(string key, string extension, string pathPrefix)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (pathPrefix != null)
				{
					stringBuilder.Append("assets/");
					stringBuilder.Append(pathPrefix);
					stringBuilder.Append('/');
				}
				stringBuilder.Append(key);
				stringBuilder.Append('.');
				stringBuilder.Append(extension);
				return stringBuilder.ToString();
			}

			public bool Equals(AssetEntry other)
			{
				return Key == other.Key && DeviceList == other.DeviceList && Extension == other.Extension && BundleKey == other.BundleKey && PathPrefix == other.PathPrefix && IsLocalized == other.IsLocalized && DefaultLocale == other.DefaultLocale && IsCacheOnly == other.IsCacheOnly && DecryptionKey == other.DecryptionKey;
			}

			public override bool Equals(object other)
			{
				if (other is AssetEntry)
				{
					return Equals((AssetEntry)other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				int num = Key.GetHashCode() ^ DeviceList.GetHashCode() ^ Extension.GetHashCode();
				if (!string.IsNullOrEmpty(BundleKey))
				{
					num ^= (BundleKey.GetHashCode() ^ PathPrefix.GetHashCode());
				}
				if (IsLocalized)
				{
					num ^= (IsLocalized.GetHashCode() ^ DefaultLocale.GetHashCode());
				}
				return num ^ (IsCacheOnly.GetHashCode() ^ DecryptionKey.GetHashCode());
			}

			public static bool operator ==(AssetEntry lhs, AssetEntry rhs)
			{
				return lhs.Equals(rhs);
			}

			public static bool operator !=(AssetEntry lhs, AssetEntry rhs)
			{
				return !lhs.Equals(rhs);
			}

			public override string ToString()
			{
				return ToUri();
			}
		}

		[Serializable]
		public struct BundleEntry : IEquatable<BundleEntry>
		{
			public const string URI_BUNDLE_DEPENDENCIES_KEY = "d";

			public const string URI_PRIORITY_KEY = "p";

			public const string PRIORITY_FIELD_NAME = "Priority";

			public const string URI_CRC_KEY = "crc";

			public readonly string Key;

			public readonly string[] DependencyBundles;

			public readonly int Priority;

			public readonly string Crc;

			public static BundleEntry Parse(string payload)
			{
				int num = payload.IndexOf('?');
				if (num < 0)
				{
					throw new ContentManifestException("URI payload does not contain a ? character\n" + payload);
				}
				string key = payload.Substring(0, num);
				string[] array = null;
				int priority = 0;
				string crc = null;
				int num2 = num + 1;
				while (num2 < payload.Length)
				{
					int num3 = payload.IndexOf('=', num2);
					if (num3 != -1)
					{
						string text = payload.Substring(num2, num3 - num2);
						num3++;
						int num4 = payload.IndexOf('&', num3);
						if (num4 == -1)
						{
							num4 = payload.Length;
						}
						string text2 = payload.Substring(num3, num4 - num3);
						num2 = num4 + 1;
						switch (text)
						{
						case "d":
							Assert.IsNull(array);
							array = (string.IsNullOrEmpty(text2) ? new string[0] : text2.Split(','));
							break;
						case "p":
							priority = int.Parse(text2);
							break;
						case "crc":
							crc = text2;
							break;
						default:
							throw new ContentManifestException("Bundle entry URI payload contains an unknown query parameter '" + text + "' at index " + num2 + "\n" + payload);
						}
						continue;
					}
					throw new ContentManifestException("Bundle entry URI payload is missing a '=' character after index " + num2 + "\n" + payload);
				}
				return new BundleEntry(key, array, priority, crc);
			}

			public static BundleEntry Create(string key, string[] dependencyBundles, int priority = 0, string crc = null)
			{
				Assert.IsNotNull(key);
				Assert.IsNotNull(dependencyBundles);
				key = key.ToLower();
				string[] array = new string[dependencyBundles.Length];
				for (int i = 0; i < dependencyBundles.Length; i++)
				{
					array[i] = dependencyBundles[i].ToLower();
				}
				Array.Sort(array);
				return new BundleEntry(key, array, priority, crc);
			}

			public BundleEntry(string key, string[] dependencyBundles, int priority = 0, string crc = null)
			{
				Assert.IsNotNull(dependencyBundles);
				Key = key;
				Priority = priority;
				Crc = crc;
				DependencyBundles = new string[dependencyBundles.Length];
				Array.Copy(dependencyBundles, DependencyBundles, dependencyBundles.Length);
				Array.Sort(DependencyBundles);
			}

			[Conditional("NOT_RC_BUILD")]
			public void Validate()
			{
				if (string.IsNullOrEmpty(Key))
				{
					throw new ContentManifestException("Bundle entry missing a key!");
				}
				if (DependencyBundles == null)
				{
					throw new ContentManifestException("Bundle entry '" + Key + "' is missing a list of dependency bundles!");
				}
			}

			public string ToUri()
			{
				StringBuilder stringBuilder = new StringBuilder("bundle:");
				stringBuilder.Append(Key);
				appendParam(stringBuilder, "d", string.Join(",", DependencyBundles), '?');
				appendParam(stringBuilder, "p", Priority.ToString());
				if (!string.IsNullOrEmpty(Crc))
				{
					appendParam(stringBuilder, "crc", Crc);
				}
				return stringBuilder.ToString();
			}

			public bool Equals(BundleEntry other)
			{
				bool flag = Key == other.Key && Priority == other.Priority && DependencyBundles.Length == other.DependencyBundles.Length && Crc == other.Crc;
				if (DependencyBundles.Length == other.DependencyBundles.Length)
				{
					int num = 0;
					while (flag && num < DependencyBundles.Length)
					{
						flag = (DependencyBundles[num] != other.DependencyBundles[num]);
						num++;
					}
				}
				return flag;
			}

			public override bool Equals(object other)
			{
				if (other is BundleEntry)
				{
					return Equals((BundleEntry)other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				int num = Key.GetHashCode() ^ Priority.GetHashCode();
				string[] dependencyBundles = DependencyBundles;
				foreach (string text in dependencyBundles)
				{
					num ^= text.GetHashCode();
				}
				if (!string.IsNullOrEmpty(Crc))
				{
					num ^= Crc.GetHashCode();
				}
				return num;
			}

			public static bool operator ==(BundleEntry lhs, BundleEntry rhs)
			{
				return lhs.Equals(rhs);
			}

			public static bool operator !=(BundleEntry lhs, BundleEntry rhs)
			{
				return !lhs.Equals(rhs);
			}
		}

		public const string MANIFEST_RUNTIME_PATH = "Configuration/embedded_content_manifest";

		public const string MANIFEST_RUNTIME_PATH_TXT = "Configuration/embedded_content_manifest.txt";

		public const string MANIFEST_EDITOR_PATH_TXT = "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt";

		public const string MANIFEST_EDITOR_PATH_ASSET = "Assets/Generated/Resources/Configuration/embedded_content_manifest.asset";

		public const string CONTENT_VERSION_TXT = "assets/game/resources/configuration/contentversion.txt";

		public const string URI_SCHEME_ASSET = "asset";

		public const string URI_SCHEME_BUNDLE = "bundle";

		public const string URI_SCHEME_BASE_URI = "baseuri";

		public const string URI_SCHEME_CONTENTVERSION = "contentversion";

		public const string URI_SCHEME_CONTENTMANIFESTHASH = "contentmanifesthash";

		public const uint URI_MIN_LENGTH = 20u;

		public const string DEVICE_LIST_BUNDLE = "bundle:www-bundle";

		public const string ASSET_PATH_PREFIX = "assets/";

		[NonSerialized]
		public Dictionary<string, AssetEntry> AssetEntryMap = new Dictionary<string, AssetEntry>();

		[NonSerialized]
		public Dictionary<string, BundleEntry> BundleEntryMap = new Dictionary<string, BundleEntry>();

		[NonSerialized]
		public Dictionary<BundleEntry, BundleEntry[]> ReverseDependencyMap = new Dictionary<BundleEntry, BundleEntry[]>();

		public string BaseUri = "";

		public string ContentVersion = "";

		public string ContentManifestHash = "";

		public ContentManifest()
		{
		}

		public ContentManifest(TextAsset textAsset)
		{
			using (StreamReader reader = new StreamReader(new MemoryStream(textAsset.bytes)))
			{
				updateManifest(reader);
			}
		}

		public ContentManifest(TextReader reader)
		{
			updateManifest(reader);
		}

		private void updateManifest(TextReader reader)
		{
			int num = 0;
			string text = reader.ReadLine();
			while (true)
			{
				if (text == null)
				{
					return;
				}
				num++;
				int num2 = text.IndexOf(':');
				if (num2 > 0)
				{
					string text2 = text.Substring(num2 + 1);
					if (text.StartsWith("asset"))
					{
						AssetEntry value = AssetEntry.Parse(text2);
						AssetEntryMap.Add(value.Key, value);
					}
					else if (text.StartsWith("bundle"))
					{
						BundleEntry value2 = BundleEntry.Parse(text2);
						BundleEntryMap.Add(value2.Key, value2);
					}
					else if (text.StartsWith("baseuri"))
					{
						BaseUri = text2;
					}
					else if (text.StartsWith("contentmanifesthash"))
					{
						ContentManifestHash = text2;
					}
					else
					{
						if (!text.StartsWith("contentversion"))
						{
							break;
						}
						ContentVersion = text2;
					}
				}
				text = reader.ReadLine();
			}
			throw new ContentManifestException("Unknown URI scheme on line " + num + ": " + text);
		}

		public void UpdateEntryMaps()
		{
			generateReverseDependencyMap();
		}

		private void generateReverseDependencyMap()
		{
			ReverseDependencyMap.Clear();
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			BundleEntry value;
			foreach (BundleEntry value4 in BundleEntryMap.Values)
			{
				value = value4;
				string[] dependencyBundles = value.DependencyBundles;
				foreach (string key in dependencyBundles)
				{
					List<string> value2;
					if (!dictionary.TryGetValue(key, out value2))
					{
						value2 = new List<string>();
						dictionary.Add(key, value2);
					}
					if (!value2.Contains(value.Key))
					{
						value2.Add(value.Key);
					}
				}
			}
			foreach (KeyValuePair<string, List<string>> item in dictionary)
			{
				if (!BundleEntryMap.TryGetValue(item.Key, out value))
				{
					throw new ContentManifestException("Failed to lookup bundle entry '" + item.Key + "' while generating reverse dependency map");
				}
				BundleEntry[] array = new BundleEntry[item.Value.Count];
				int num = 0;
				foreach (string item2 in item.Value)
				{
					BundleEntry value3;
					if (!BundleEntryMap.TryGetValue(item2, out value3))
					{
						throw new ContentManifestException("Failed to lookup bundle entry '" + item2 + "' while generating reverse dependency map for entry '" + item.Key + "'");
					}
					array[num++] = value3;
				}
				ReverseDependencyMap[value] = array;
			}
		}

		public void UpdateFrom(ContentManifest other)
		{
			AssetEntryMap.Clear();
			BundleEntryMap.Clear();
			AssetEntryMap = new Dictionary<string, AssetEntry>(other.AssetEntryMap);
			BundleEntryMap = new Dictionary<string, BundleEntry>(other.BundleEntryMap);
			ReverseDependencyMap = new Dictionary<BundleEntry, BundleEntry[]>(other.ReverseDependencyMap);
			BaseUri = other.BaseUri;
			ContentVersion = other.ContentVersion;
		}

		public bool TryGetBundleEntry(string bundleKey, out BundleEntry bundleEntry)
		{
			return BundleEntryMap.TryGetValue(bundleKey, out bundleEntry);
		}

		public bool TryGetDependentEntries(string bundleKey, out BundleEntry[] dependents)
		{
			dependents = null;
			BundleEntry value;
			return BundleEntryMap.TryGetValue(bundleKey, out value) && ReverseDependencyMap.TryGetValue(value, out dependents);
		}

		public bool TryGetDependentEntries(BundleEntry bundleEntry, out BundleEntry[] dependents)
		{
			return ReverseDependencyMap.TryGetValue(bundleEntry, out dependents);
		}

		public void AddAssetEntries(IEnumerable<AssetEntry> list)
		{
			foreach (AssetEntry item in list)
			{
				AddAssetEntry(item);
			}
		}

		public void AddAssetEntry(AssetEntry entry)
		{
			validateNewAssetEntry(ref entry);
			AssetEntryMap.Add(entry.Key, entry);
		}

		public void AddBundleEntries(IEnumerable<BundleEntry> list)
		{
			foreach (BundleEntry item in list)
			{
				AddBundleEntry(item);
			}
		}

		public void AddBundleEntry(BundleEntry entry)
		{
			validateNewBundleEntry(ref entry);
			BundleEntryMap.Add(entry.Key, entry);
		}

		public void UpdateAssetEntry(AssetEntry entry)
		{
			if (!AssetEntryMap.ContainsKey(entry.Key))
			{
				throw new ContentManifestException("Can't update asset entry since key is not found: " + entry.Key);
			}
			AssetEntryMap[entry.Key] = entry;
		}

		public void UpdateBundleEntry(BundleEntry entry)
		{
			if (!BundleEntryMap.ContainsKey(entry.Key))
			{
				throw new ContentManifestException("Can't update bundle entry since key is not found: " + entry.Key);
			}
			BundleEntryMap[entry.Key] = entry;
		}

		private void validateNewAssetEntry(ref AssetEntry entry)
		{
			if (AssetEntryMap.ContainsKey(entry.Key))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("Two assets have the same key and this is not allowed: key = '{0}'. ", entry.Key));
				stringBuilder.AppendLine("The asset will not be added to the manifest.");
				throw new ContentManifestException(stringBuilder.ToString());
			}
		}

		private void validateNewBundleEntry(ref BundleEntry entry)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(entry.Key))
			{
				stringBuilder.Append("Bundle key cannot be empty for bundle entry = " + entry);
			}
			if (BundleEntryMap.ContainsKey(entry.Key))
			{
				stringBuilder.AppendFormat("Two bundles have the same key and this is not allowed: key = '{0}' in bundleEntry={1}. ", entry.Key, entry);
				stringBuilder.AppendLine("The bundle will not be added to the manifest.");
			}
			if (stringBuilder.Length > 0)
			{
				throw new ContentManifestException(stringBuilder.ToString());
			}
		}

		public void ReadFromFile(string path)
		{
			try
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new Exception("Path to read manifest file is null or empty");
				}
				if (!File.Exists(path))
				{
					throw new FileNotFoundException(string.Format("No file found at path {0}", path));
				}
				using (StreamReader reader = File.OpenText(path))
				{
					updateManifest(reader);
				}
			}
			catch (Exception ex)
			{
				Log.LogError(this, ex.Message);
			}
		}

		public void WriteToFile(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("Path to write manifest file is null or empty");
			}
			File.WriteAllText(path, ToText());
		}

		public string ToText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AssetEntry value in AssetEntryMap.Values)
			{
				stringBuilder.AppendLine(value.ToUri());
			}
			foreach (BundleEntry value2 in BundleEntryMap.Values)
			{
				stringBuilder.AppendLine(value2.ToUri());
			}
			stringBuilder.Append("baseuri:");
			stringBuilder.AppendLine(BaseUri);
			stringBuilder.Append("contentversion:");
			stringBuilder.AppendLine(ContentVersion);
			stringBuilder.Append("contentmanifesthash:");
			stringBuilder.AppendLine(ContentManifestHash);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return string.Format("[ContentManifest] ContentVersion:{0}, ContentManifestHash{1}, AssetEntries:{2}, Bundles:{3}", ContentVersion, ContentManifestHash, AssetEntryMap.Count, BundleEntryMap.Count);
		}

		private static void appendParam(StringBuilder sb, string key, string value, char separator = '&')
		{
			sb.Append(separator);
			sb.Append(key);
			sb.Append('=');
			sb.Append(value);
		}
	}
}
