using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public static class ContentManifestUtility
	{
		public static ContentManifest FromDefinitionFile(string path)
		{
			return new ContentManifest(Resources.Load<TextAsset>(path));
		}

		public static ContentManifest Merge(ContentManifest first, ContentManifest second)
		{
			Dictionary<string, ContentManifest.AssetEntry> dictionary = new Dictionary<string, ContentManifest.AssetEntry>(first.AssetEntryMap);
			foreach (KeyValuePair<string, ContentManifest.AssetEntry> item in second.AssetEntryMap)
			{
				dictionary[item.Key] = item.Value;
			}
			Dictionary<string, ContentManifest.BundleEntry> dictionary2 = new Dictionary<string, ContentManifest.BundleEntry>(first.BundleEntryMap);
			foreach (KeyValuePair<string, ContentManifest.BundleEntry> item2 in second.BundleEntryMap)
			{
				dictionary2[item2.Key] = item2.Value;
			}
			ContentManifest contentManifest = new ContentManifest();
			if (second != null)
			{
				contentManifest.BaseUri = second.BaseUri;
				contentManifest.ContentVersion = second.ContentVersion;
				contentManifest.ContentManifestHash = second.ContentManifestHash;
			}
			else if (first != null)
			{
				contentManifest.BaseUri = first.BaseUri;
				contentManifest.ContentVersion = first.ContentVersion;
				contentManifest.ContentManifestHash = first.ContentManifestHash;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, ContentManifest.AssetEntry> item3 in dictionary)
			{
				ContentManifest.AssetEntry value = item3.Value;
				if (!string.IsNullOrEmpty(value.BundleKey))
				{
					hashSet.Add(value.BundleKey);
				}
			}
			foreach (KeyValuePair<string, ContentManifest.BundleEntry> item4 in dictionary2)
			{
				ContentManifest.BundleEntry value2 = item4.Value;
				if (hashSet.Contains(value2.Key))
				{
					string[] dependencyBundles = value2.DependencyBundles;
					foreach (string text in dependencyBundles)
					{
						if (!string.IsNullOrEmpty(text))
						{
							hashSet.Add(text);
						}
					}
				}
			}
			Dictionary<string, ContentManifest.BundleEntry> dictionary3 = new Dictionary<string, ContentManifest.BundleEntry>();
			foreach (string item5 in hashSet)
			{
				if (string.IsNullOrEmpty(item5))
				{
					throw new Exception("Bundle key cannot be empty when assigning it to Bundle map");
				}
				if (dictionary2.ContainsKey(item5))
				{
					ContentManifest.BundleEntry value3 = dictionary2[item5];
					dictionary3.Add(item5, value3);
				}
			}
			contentManifest.AssetEntryMap = dictionary;
			contentManifest.BundleEntryMap = dictionary3;
			return contentManifest;
		}
	}
}
