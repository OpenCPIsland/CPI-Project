using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ScenePrereqContentManager
	{
		private readonly Dictionary<string, ContentManifest.BundleEntry[]> scenePrereqBundles;

		private BundlePrecacheManager bundlePrecacheManager;

		public ScenePrereqContentManager(BundlePrecacheManager bundlePrecacheManager, ScenePrereqContentBundlesManifest prereqs)
		{
			scenePrereqBundles = new Dictionary<string, ContentManifest.BundleEntry[]>();
			SetPrereqs(bundlePrecacheManager, prereqs);
		}

		public void SetPrereqs(BundlePrecacheManager bundlePrecacheManager, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest)
		{
			if (bundlePrecacheManager == null)
			{
				throw new ArgumentNullException("bundlePrecacheManager");
			}
			this.bundlePrecacheManager = bundlePrecacheManager;
			if (scenePrereqBundlesManifest != null)
			{
				ContentManifest manifest = bundlePrecacheManager.GetManifest();
				if (manifest == null)
				{
					throw new ContentManifestException("No Contentmanifest set in BundlePreCacheManager to reference for scene prerequisites.");
				}
				foreach (KeyValuePair<string, string[]> item in scenePrereqBundlesManifest)
				{
					string key = item.Key;
					string[] value = item.Value;
					if (value != null)
					{
						List<ContentManifest.BundleEntry> list = new List<ContentManifest.BundleEntry>();
						string[] array = value;
						foreach (string text in array)
						{
							if (!manifest.BundleEntryMap.ContainsKey(text))
							{
								throw new ContentManifestException("Expected bundle key not found in BundleEntryMap for bundleKey=" + text);
							}
							list.Add(manifest.BundleEntryMap[text]);
						}
						if (list.Count > 0)
						{
							scenePrereqBundles.Add(item.Key, list.ToArray());
						}
					}
				}
			}
		}

		public IEnumerator LoadPrereqBundlesForScene(string sceneName)
		{
			if (scenePrereqBundles.ContainsKey(sceneName))
			{
				bool isDone = false;
				bundlePrecacheManager.StartCaching(scenePrereqBundles[sceneName], delegate
				{
					bundlePrecacheManager.MoveToBackground();
					isDone = true;
				});
				uint PrecacheTime = bundlePrecacheManager.Config.BundlePrecacheSeconds;
				float delta = 0f;
				while (!isDone && (uint)delta < PrecacheTime)
				{
					delta += Time.deltaTime;
					yield return null;
				}
				yield return null;
			}
		}
	}
}
