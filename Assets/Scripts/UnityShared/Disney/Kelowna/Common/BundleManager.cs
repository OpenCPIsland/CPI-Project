using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class BundleManager : IDisposable
	{
		public const string BUNDLE_EXTENSION = "unity3d";

		private readonly ContentManifest manifest;

		private readonly Dictionary<string, BundleMount> mountedBundles;

		private readonly List<BundleMount> bundlesToUnmount;

		private readonly Dictionary<string, BundleMount> unmountingBundles;

		private readonly ICoroutine monitorCoroutine;

		public int UnmountFrameCount = 250;

		public int MonitorFrameFrequency = 50;

		public BundleManager(ContentManifest manifest)
		{
			if (manifest == null)
			{
				manifest = new ContentManifest();
			}
			this.manifest = manifest;
			mountedBundles = new Dictionary<string, BundleMount>();
			bundlesToUnmount = new List<BundleMount>();
			unmountingBundles = new Dictionary<string, BundleMount>();
		}

		public void Dispose()
		{
			if (monitorCoroutine != null)
			{
				monitorCoroutine.Cancel();
			}
			UnmountAllBundles();
		}

		public BundleMount MountBundle(string bundleKey, AssetBundle bundle, bool pinned = false)
		{
			if (IsMounted(bundleKey))
			{
				throw new InvalidOperationException("A bundle with key '" + bundleKey + "' is already mounted.");
			}
			HashSet<BundleMount> hashSet = null;
			ContentManifest.BundleEntry bundleEntry;
			bool flag = manifest.TryGetBundleEntry(bundleKey, out bundleEntry);
			if (!flag)
			{
			}
			ContentManifest.BundleEntry[] dependents;
			if (flag && manifest.TryGetDependentEntries(bundleEntry, out dependents))
			{
				hashSet = new HashSet<BundleMount>();
				for (int i = 0; i < dependents.Length; i++)
				{
					ContentManifest.BundleEntry bundleEntry2 = dependents[i];
					if (IsMounted(bundleEntry2.Key))
					{
						hashSet.Add(GetBundle(bundleEntry2.Key));
					}
				}
			}
			BundleMount bundleMount = new BundleMount(bundle, bundleKey, hashSet);
			if (hashSet == null && flag)
			{
				for (int i = 0; i < bundleEntry.DependencyBundles.Length; i++)
				{
					string bundleKey2 = bundleEntry.DependencyBundles[i];
					BundleMount mount;
					if (TryGetBundle(bundleKey2, out mount))
					{
						mount.DependentMounts.Add(bundleMount);
					}
				}
			}
			mountedBundles[bundleKey] = bundleMount;
			bundleMount.IsPinned = pinned;
			bundleMount.EUnmounted += delegate
			{
				UnmountBundle(bundleKey, false);
			};
			return bundleMount;
		}

		public void UnmountBundle(string bundleKey, bool unloadAllLoadedObjects)
		{
			BundleMount mount;
			if (!mountedBundles.TryGetValue(bundleKey, out mount))
			{
				throw new ArgumentException("A bundle with key '" + bundleKey + "' is not currently mounted.");
			}
			if (mount != null)
			{
				if (mount.ActiveRequestCount > 0)
				{
					unmountingBundles[bundleKey] = mount;
					mount.EFinishedLoading += delegate
					{
						unloadMount(mount, unloadAllLoadedObjects);
					};
				}
				else
				{
					unmountingBundles[bundleKey] = mount;
					unloadMount(mount, unloadAllLoadedObjects);
				}
			}
		}

		public void CancelUnmount(string bundleKey)
		{
			if (!IsUnmounting(bundleKey))
			{
				throw new InvalidOperationException("Cannot cancel unmount for a bundle not being unmounted: " + bundleKey);
			}
			unmountingBundles.Remove(bundleKey);
		}

		public void UnmountAllBundles()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, BundleMount> mountedBundle in mountedBundles)
			{
				BundleMount value = mountedBundle.Value;
				if (!IsUnmounting(mountedBundle.Key) && value.IsMounted)
				{
					if (value.IsPinned)
					{
					}
					list.Add(value.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				UnmountBundle(list[i], false);
			}
		}

		public BundleMount GetBundle(string bundleKey)
		{
			BundleMount value;
			if (!mountedBundles.TryGetValue(bundleKey, out value))
			{
				throw new InvalidOperationException("The requested bundle is not currently mounted: " + bundleKey);
			}
			if (IsUnmounting(bundleKey))
			{
				throw new InvalidOperationException("The requested bundle is currently being unmounted and can no longer be accessed: " + bundleKey);
			}
			return value;
		}

		public bool TryGetBundle(string bundleKey, out BundleMount mount)
		{
			mount = null;
			if (!string.IsNullOrEmpty(bundleKey) && mountedBundles.TryGetValue(bundleKey, out mount) && mount != null)
			{
				return true;
			}
			return false;
		}

		public bool IsMounted(string bundleKey)
		{
			return mountedBundles.ContainsKey(bundleKey) && !IsUnmounting(bundleKey);
		}

		public bool IsUnmounting(string bundleKey)
		{
			return unmountingBundles.ContainsKey(bundleKey);
		}

		public int GetActiveRequestCount()
		{
			int num = 0;
			foreach (KeyValuePair<string, BundleMount> mountedBundle in mountedBundles)
			{
				num += mountedBundle.Value.ActiveRequestCount;
			}
			return num;
		}

		public string[] GetDependencyKeys(string key)
		{
			ContentManifest.BundleEntry value;
			return manifest.BundleEntryMap.TryGetValue(key, out value) ? value.DependencyBundles : new string[0];
		}

		public ContentManifest.BundleEntry GetBundleEntry(string key)
		{
			ContentManifest.BundleEntry value;
			manifest.BundleEntryMap.TryGetValue(key, out value);
			return value;
		}

		private void unloadMount(BundleMount mount, bool unloadAllLoadedObjects)
		{
			if (!IsUnmounting(mount.Key))
			{
				return;
			}
			if (mount.IsMounted)
			{
				mount.Unload(unloadAllLoadedObjects);
			}
			ContentManifest.BundleEntry bundleEntry;
			if (!mount.IsDependency && manifest.TryGetBundleEntry(mount.Key, out bundleEntry))
			{
				for (int i = 0; i < bundleEntry.DependencyBundles.Length; i++)
				{
					string bundleKey = bundleEntry.DependencyBundles[i];
					BundleMount mount2;
					if (TryGetBundle(bundleKey, out mount2))
					{
						mount2.DependentMounts.Remove(mount);
					}
				}
			}
			mountedBundles.Remove(mount.Key);
			unmountingBundles.Remove(mount.Key);
		}

		private IEnumerator monitorMountedBundles()
		{
			yield return null;
			while (true)
			{
				yield return new WaitForFrame(MonitorFrameFrequency);
				UnmountUnusedBundles();
			}
		}

		public void UnmountUnusedBundles()
		{
			foreach (KeyValuePair<string, BundleMount> mountedBundle in mountedBundles)
			{
				BundleMount value = mountedBundle.Value;
				if (!IsUnmounting(mountedBundle.Key) && !value.IsPinned && value.ActiveRequestCount == 0)
				{
					int num = Time.frameCount - value.LastLoadFrame;
					if (num >= UnmountFrameCount)
					{
						bundlesToUnmount.Add(value);
					}
				}
			}
			for (int i = 0; i < bundlesToUnmount.Count; i++)
			{
				UnmountBundle(bundlesToUnmount[i].Key, false);
			}
			bundlesToUnmount.Clear();
		}
	}
}
