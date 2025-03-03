using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class BundlePrecacheManager
	{
		public delegate void PrecacheComplete();

		private ContentManifest manifest;

		private ContentPreCachingConfig config;

		private PrecacheComplete completeCallback = null;

		private int numCurrentDownloads = 0;

		private bool caching = false;

		private bool background = false;

		private float completeRatio = 0f;

		public bool IsCaching
		{
			get
			{
				return caching;
			}
		}

		public bool IsReady
		{
			get
			{
				return config != null;
			}
		}

		public float CompleteRatio
		{
			get
			{
				return completeRatio;
			}
		}

		public ContentPreCachingConfig Config
		{
			get
			{
				return config;
			}
		}

		public BundlePrecacheManager(ContentManifest contentManifest)
		{
			SetManifest(contentManifest);
			CoroutineRunner.StartPersistent(loadConfig(), this, "BundlePrecacheManager.loadConfig()");
		}

		public ContentManifest GetManifest()
		{
			return manifest;
		}

		public void SetManifest(ContentManifest contentManifest)
		{
			manifest = contentManifest;
		}

		private IEnumerator loadConfig()
		{
			AssetRequest<ContentPreCachingConfig> request = Content.LoadAsync<ContentPreCachingConfig>("Configuration/ContentPreCachingConfig");
			yield return request;
			config = request.Asset;
		}

		public void StartCaching(PrecacheComplete callback, bool cacheInBackground = false)
		{
			completeRatio = 0f;
			caching = true;
			background = cacheInBackground;
			completeCallback = callback;
			CoroutineRunner.StartPersistent(CacheBundles(), this, "BundlePrecacheManager.cacheBundles()");
		}

		public void StartCaching(IList<ContentManifest.BundleEntry> sortedBundles, PrecacheComplete callback, bool cacheInBackground = false)
		{
			caching = true;
			background = cacheInBackground;
			completeCallback = callback;
			CoroutineRunner.StartPersistent(CacheBundles(sortedBundles), this, "BundlePrecacheManager.cacheBundles()");
		}

		public void MoveToBackground()
		{
			background = true;
		}

		public void StopCaching()
		{
			caching = false;
		}

		public IEnumerator CacheBundles()
		{
			List<ContentManifest.BundleEntry> sortedBundles = new List<ContentManifest.BundleEntry>(manifest.BundleEntryMap.Values);
			sortedBundles.Sort(compareBundlesByPriority);
			yield return cacheBundles(sortedBundles, false);
		}

		private IEnumerator cacheBundles(IList<ContentManifest.BundleEntry> sortedBundles, bool ignorePriority)
		{
			for (int i = 0; i < sortedBundles.Count; i++)
			{
				completeRatio = (float)(i - numCurrentDownloads) / (float)sortedBundles.Count;
				uint maxConcurrentDownloads = background ? config.MaxConcurrentBackgroundDownloads : config.MaxConcurrentForegroundDownloads;
				while (numCurrentDownloads >= maxConcurrentDownloads)
				{
					maxConcurrentDownloads = (background ? config.MaxConcurrentBackgroundDownloads : config.MaxConcurrentForegroundDownloads);
					yield return null;
				}
				if (!caching)
				{
					break;
				}
				ContentManifest.BundleEntry bundle = sortedBundles[i];
				if (ignorePriority || bundle.Priority > 0)
				{
					CoroutineRunner.StartPersistent(downloadBundle(bundle), this, "BundlePrecacheManager.downloadBundle()");
					continue;
				}
				break;
			}
			if (sortedBundles.Count > 0)
			{
				while (numCurrentDownloads > 0)
				{
					completeRatio = (float)(sortedBundles.Count - numCurrentDownloads) / (float)sortedBundles.Count;
					yield return null;
				}
				completeRatio = (float)(sortedBundles.Count - numCurrentDownloads) / (float)sortedBundles.Count;
			}
			if (caching && completeCallback != null)
			{
				completeCallback();
			}
			yield return null;
		}

		private IEnumerator downloadBundle(ContentManifest.BundleEntry bundleEntry)
		{
			ContentManifest.AssetEntry entry = new ContentManifest.AssetEntry(bundleEntry.Key, "www-bundle", "unity3d");
			numCurrentDownloads++;
			AssetRequest<AssetBundle> assetRequest = Content.DeviceManager.LoadAsync<AssetBundle>("www-bundle", ref entry);
			yield return assetRequest;
			assetRequest.Asset.Unload(true);
			numCurrentDownloads--;
		}

		private static int compareBundlesByPriority(ContentManifest.BundleEntry bundle1, ContentManifest.BundleEntry bundle2)
		{
			return bundle2.Priority.CompareTo(bundle1.Priority);
		}

		public IEnumerator LoadBundlesForContentPaths(ContentPath[] contentPaths)
		{
			Dictionary<string, ContentManifest.BundleEntry> bundlesToLoad = new Dictionary<string, ContentManifest.BundleEntry>();
			foreach (ContentPath contentPath in contentPaths)
			{
				string[] array = contentPath.Path.Split(new string[1]
				{
					"/Resources/"
				}, StringSplitOptions.None);
				string text = "";
				if (array.Length == 2)
				{
					text = array[1];
				}
				foreach (KeyValuePair<string, ContentManifest.AssetEntry> item in manifest.AssetEntryMap)
				{
					bool flag = false;
					if (string.IsNullOrEmpty(text) || item.Value.Key.IndexOf("/") == -1 || item.Value.Key.StartsWith(text.ToLower() + "/"))
					{
						flag = true;
					}
					if (flag)
					{
						string bundleKey = item.Value.BundleKey;
						if (!string.IsNullOrEmpty(bundleKey) && !bundlesToLoad.ContainsKey(bundleKey) && manifest.BundleEntryMap.ContainsKey(bundleKey))
						{
							ContentManifest.BundleEntry value = manifest.BundleEntryMap[bundleKey];
							bundlesToLoad.Add(bundleKey, value);
						}
					}
				}
			}
			caching = true;
			background = false;
			yield return cacheBundles(new List<ContentManifest.BundleEntry>(bundlesToLoad.Values), true);
		}

		public IEnumerator CacheBundles(IEnumerable<ContentManifest.BundleEntry> bundles)
		{
			caching = true;
			background = false;
			yield return cacheBundles(new List<ContentManifest.BundleEntry>(bundles), true);
		}

		public IEnumerator LoadBundlesForContent(AssetContentKey[] includingAssets)
		{
			Dictionary<string, ContentManifest.BundleEntry> bundlesToLoad = new Dictionary<string, ContentManifest.BundleEntry>();
			foreach (AssetContentKey assetContentKey in includingAssets)
			{
				if (manifest.AssetEntryMap.ContainsKey(assetContentKey.Key))
				{
					string bundleKey = manifest.AssetEntryMap[assetContentKey.Key].BundleKey;
					if (!string.IsNullOrEmpty(bundleKey) && !bundlesToLoad.ContainsKey(bundleKey) && manifest.BundleEntryMap.ContainsKey(bundleKey))
					{
						ContentManifest.BundleEntry value = manifest.BundleEntryMap[bundleKey];
						bundlesToLoad.Add(bundleKey, value);
					}
				}
			}
			caching = true;
			background = false;
			yield return cacheBundles(new List<ContentManifest.BundleEntry>(bundlesToLoad.Values), true);
		}
	}
}
