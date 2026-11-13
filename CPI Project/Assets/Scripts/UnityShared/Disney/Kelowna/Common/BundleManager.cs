using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

		private readonly Dictionary<string, CachedAddressable> addressableAssets = new Dictionary<string, CachedAddressable>();

		private readonly Dictionary<string, int> addressableReferenceCounts = new Dictionary<string, int>();

		private readonly HashSet<string> pinnedAddressableBundles = new HashSet<string>();

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
			foreach (CachedAddressable cachedAddressable in addressableAssets.Values)
			{
				if (cachedAddressable.Handle.IsValid())
				{
					Addressables.Release(cachedAddressable.Handle);
				}
			}
			addressableAssets.Clear();
			addressableReferenceCounts.Clear();
		}

		public AsyncOperationHandle<TAsset> LoadAddressableAsync<TAsset>(string assetKey, string bundleKey, string address) where TAsset : class
		{
			CachedAddressable cachedAddressable;
			if (addressableAssets.TryGetValue(assetKey, out cachedAddressable))
			{
				cachedAddressable.LastLoadFrame = Time.frameCount;
				return cachedAddressable.Handle.Convert<TAsset>();
			}
			AsyncOperationHandle<TAsset> handle = Addressables.LoadAssetAsync<TAsset>(address);
			addressableAssets.Add(assetKey, new CachedAddressable(handle, bundleKey));
			return handle;
		}

		public AsyncOperationHandle DownloadAddressableDependencies(string assetKey, string bundleKey, string address)
		{
			CachedAddressable cachedAddressable;
			if (addressableAssets.TryGetValue(assetKey, out cachedAddressable))
			{
				cachedAddressable.LastLoadFrame = Time.frameCount;
				return cachedAddressable.Handle;
			}
			AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(address);
			addressableAssets.Add(assetKey, new CachedAddressable(handle, bundleKey));
			return handle;
		}

		public TAsset LoadAddressableImmediate<TAsset>(string assetKey, string bundleKey, string address) where TAsset : class
		{
#if UNITY_WEBGL
			throw new InvalidOperationException("Addressable assets must be loaded asynchronously on WebGL.");
#else
			AsyncOperationHandle<TAsset> handle = LoadAddressableAsync<TAsset>(assetKey, bundleKey, address);
			TAsset asset = handle.WaitForCompletion();
			if (handle.Status != AsyncOperationStatus.Succeeded)
			{
				throw handle.OperationException ?? new InvalidOperationException("Addressable asset load failed: " + address);
			}
			return asset;
#endif
		}

		public void RetainAddressable(string assetKey)
		{
			int referenceCount;
			addressableReferenceCounts.TryGetValue(assetKey, out referenceCount);
			addressableReferenceCounts[assetKey] = referenceCount + 1;
			CachedAddressable cachedAddressable;
			if (addressableAssets.TryGetValue(assetKey, out cachedAddressable))
			{
				cachedAddressable.LastLoadFrame = Time.frameCount;
			}
		}

		public void ReleaseAddressableReference(string assetKey)
		{
			int referenceCount;
			if (!addressableReferenceCounts.TryGetValue(assetKey, out referenceCount))
			{
				return;
			}
			if (referenceCount <= 1)
			{
				addressableReferenceCounts.Remove(assetKey);
				CachedAddressable cachedAddressable;
				if (addressableAssets.TryGetValue(assetKey, out cachedAddressable))
				{
					cachedAddressable.LastLoadFrame = Time.frameCount;
				}
			}
			else
			{
				addressableReferenceCounts[assetKey] = referenceCount - 1;
			}
		}

		public bool ReleaseAddressable(string assetKey)
		{
			if (addressableReferenceCounts.ContainsKey(assetKey))
			{
				return false;
			}
			CachedAddressable cachedAddressable;
			if (addressableAssets.TryGetValue(assetKey, out cachedAddressable))
			{
				if (cachedAddressable.Handle.IsValid())
				{
					Addressables.Release(cachedAddressable.Handle);
				}
				addressableAssets.Remove(assetKey);
			}
			return true;
		}

		public void SetAddressableBundlePinned(string bundleKey, bool isPinned)
		{
			if (isPinned)
			{
				pinnedAddressableBundles.Add(bundleKey);
			}
			else
			{
				pinnedAddressableBundles.Remove(bundleKey);
			}
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
			List<string> addressablesToRelease = new List<string>();
			foreach (KeyValuePair<string, CachedAddressable> addressableAsset in addressableAssets)
			{
				CachedAddressable cachedAddressable = addressableAsset.Value;
				if (!addressableReferenceCounts.ContainsKey(addressableAsset.Key) &&
					!pinnedAddressableBundles.Contains(cachedAddressable.BundleKey) &&
					Time.frameCount - cachedAddressable.LastLoadFrame >= UnmountFrameCount)
				{
					addressablesToRelease.Add(addressableAsset.Key);
				}
			}
			for (int i = 0; i < addressablesToRelease.Count; i++)
			{
				ReleaseAddressable(addressablesToRelease[i]);
			}
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

		private sealed class CachedAddressable
		{
			public AsyncOperationHandle Handle;

			public string BundleKey;

			public int LastLoadFrame;

			public CachedAddressable(AsyncOperationHandle handle, string bundleKey)
			{
				Handle = handle;
				BundleKey = bundleKey;
				LastLoadFrame = Time.frameCount;
			}
		}
	}
}
