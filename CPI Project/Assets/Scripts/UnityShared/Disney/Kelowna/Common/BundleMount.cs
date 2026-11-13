using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class BundleMount
	{
		public const string ASSET_PATH_PREFIX = "assets/rootassets/";

		public readonly string Key;

		private bool isPinned;

		private readonly Dictionary<string, CoroutineReturn> activeRequests;

		public int AssetCount
		{
			get;
			private set;
		}

		public int LastLoadFrame
		{
			get;
			private set;
		}

		public bool IsMounted
		{
			get;
			private set;
		}

		public HashSet<BundleMount> DependentMounts
		{
			get;
			private set;
		}

		public bool IsPinned
		{
			get
			{
				foreach (BundleMount dependentMount in DependentMounts)
				{
					if (dependentMount.IsMounted)
					{
						return true;
					}
				}
				return isPinned;
			}
			set
			{
				isPinned = value;
			}
		}

		public bool IsDependency
		{
			get
			{
				return DependentMounts.Count > 0;
			}
		}

		public int ActiveRequestCount
		{
			get
			{
				return activeRequests.Count;
			}
		}

		internal AssetBundle Bundle
		{
			get;
			private set;
		}

		public event Action EFinishedLoading;

		public event Action EUnmounted;

		public BundleMount(AssetBundle bundle, string key, HashSet<BundleMount> dependentMounts)
			: this(key, dependentMounts)
		{
			MountBundle(bundle);
		}

		public BundleMount(string key, HashSet<BundleMount> dependentMounts)
		{
			IsMounted = false;
			Key = key;
			if (dependentMounts == null)
			{
				dependentMounts = new HashSet<BundleMount>();
			}
			DependentMounts = dependentMounts;
			activeRequests = new Dictionary<string, CoroutineReturn>();
		}

		public void MountBundle(AssetBundle bundle)
		{
			if (IsMounted)
			{
				throw new InvalidOperationException("BundleMount '" + Key + "' is already mounted and cannot be bound to a different asset bundle. Unmount it before binding to a different bundle.");
			}
			Bundle = bundle;
			IsMounted = true;
			LastLoadFrame = Time.frameCount;
			AssetCount = bundle.GetAllAssetNames().Length;
		}

		public AsyncAssetBundleRequest<TAsset> LoadAsync<TAsset>(string key, string assetPath, AssetLoadedHandler<TAsset> handler = null) where TAsset : class
		{
			CoroutineReturn value;
			AsyncAssetBundleRequest<TAsset> asyncAssetBundleRequest;
			if (activeRequests.TryGetValue(key, out value))
			{
				asyncAssetBundleRequest = (AsyncAssetBundleRequest<TAsset>)value;
			}
			else
			{
				asyncAssetBundleRequest = new AsyncAssetBundleRequest<TAsset>(key, null);
				activeRequests[key] = asyncAssetBundleRequest;
				CoroutineRunner.StartPersistent(waitForAssetToLoad(key, assetPath, asyncAssetBundleRequest, handler), this, "waitForAssetToLoad");
			}
			LastLoadFrame = Time.frameCount;
			return asyncAssetBundleRequest;
		}

		private IEnumerator waitForAssetToLoad<TAsset>(string key, string assetPath, AsyncAssetBundleRequest<TAsset> assetRequest, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			AssetBundleRequest unityRequest = assetRequest.Request = Bundle.LoadAssetAsync<TAsset>(assetPath);
			yield return unityRequest;
			activeRequests.Remove(key);
			if (handler != null)
			{
				handler(key, (TAsset)(object)unityRequest.asset);
			}
			if (this.EFinishedLoading != null && activeRequests.Count == 0)
			{
				yield return null;
				this.EFinishedLoading();
			}
		}

		public TAsset LoadImmediate<TAsset>(string key, string assetPath) where TAsset : class
		{
			LastLoadFrame = Time.frameCount;
			return (TAsset)(object)Bundle.LoadAsset(assetPath, typeof(TAsset));
		}

		public void Unload(bool unloadAllLoadedObjects)
		{
			if (!IsMounted)
			{
				throw new InvalidOperationException("Cannot unload a bundle that is not mounted: " + Key);
			}
			if (activeRequests.Count > 0)
			{
			}
			IsMounted = false;
			Bundle.Unload(unloadAllLoadedObjects);
			Bundle = null;
			activeRequests.Clear();
			if (this.EUnmounted != null)
			{
				this.EUnmounted();
				this.EUnmounted = null;
			}
		}
	}
}
