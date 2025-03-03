using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tweaker.Core;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class Content : IDisposable
	{
		public struct ContentManifestUpdated
		{
			public ContentManifest ContentManifest;

			public ContentManifestUpdated(ContentManifest manifest)
			{
				ContentManifest = manifest;
			}
		}

		[Tweakable("Content.Manifest")]
		private ContentManifest manifest;

		private WwwBundleDevice wwwBundleDevice;

		private readonly WeakIndex<object> assetIndex;

		private readonly Dictionary<string, CoroutineReturn> activeRequests;

		private readonly Dictionary<string, List<object>> activeRequestHandlers;

		private readonly DeviceManager deviceManager;

		private readonly BundleManager bundleManager;

		private readonly string currentLanguage;

		private string contentVersionFromFile;

		[Tweakable("Content.ContentVersion", Description = "Static Game Data content version hash used to sync to the server data")]
		private string contentVersionHash;

		[Tweakable("Content.ContentManifestHash", Description = "Hash of the asset bundle entries found in the downloaded Content Manifest")]
		private string contentManifestHash;

		public string ContentVersion
		{
			get
			{
				if (manifest != null && !string.IsNullOrEmpty(manifest.ContentVersion))
				{
					return manifest.ContentVersion;
				}
				if (string.IsNullOrEmpty(contentVersionFromFile))
				{
					try
					{
						contentVersionFromFile = LoadImmediate(new TypedAssetContentKey<TextAsset>("configuration/contentversion")).text;
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(this, "Failed to read content version from ContentVersion.txt file. Server will be instructed to use the bundled content. Exception: {0}", ex.Message);
						contentVersionFromFile = "UNKNOWN";
					}
				}
				return contentVersionFromFile;
			}
		}

		public static BundleManager BundleManager
		{
			get
			{
				return Service.Get<Content>().bundleManager;
			}
		}

		public static DeviceManager DeviceManager
		{
			get
			{
				return Service.Get<Content>().deviceManager;
			}
		}

		public Content(ContentManifest manifest, string currentLanguage = "en_US")
		{
			setManifest(manifest);
			assetIndex = new WeakIndex<object>();
			activeRequests = new Dictionary<string, CoroutineReturn>();
			activeRequestHandlers = new Dictionary<string, List<object>>();
			bundleManager = new BundleManager(this.manifest);
			deviceManager = new DeviceManager();
			this.currentLanguage = currentLanguage;
			mountStandardDevices(manifest.BaseUri);
			Service.Get<EventDispatcher>().DispatchEvent(new ContentManifestUpdated(manifest));
		}

		private void setManifest(ContentManifest newManifest)
		{
			manifest = newManifest;
			contentVersionHash = manifest.ContentVersion;
			contentManifestHash = manifest.ContentManifestHash;
		}

		public void UpdateContentManifest(ContentManifest newManifest)
		{
			setManifest(newManifest);
			wwwBundleDevice.UpdateBaseUri(manifest.BaseUri);
			manifest.UpdateEntryMaps();
			Service.Get<EventDispatcher>().DispatchEvent(new ContentManifestUpdated(manifest));
		}

		public void ReplaceManifest(ContentManifest newManifest)
		{
			manifest.UpdateFrom(newManifest);
		}

		public void Dispose()
		{
			bundleManager.Dispose();
			deviceManager.Dispose();
			assetIndex.RemoveAll();
			foreach (KeyValuePair<string, CoroutineReturn> activeRequest in activeRequests)
			{
				activeRequest.Value.Cancelled = true;
			}
		}

		private void mountStandardDevices(string baseUri)
		{
			deviceManager.Mount(new ResourceDevice(deviceManager));
			deviceManager.Mount(new BundleDevice(deviceManager, bundleManager));
			deviceManager.Mount(new StreamingAssetBundleDevice(deviceManager));
			deviceManager.Mount(new AssetDatabaseDevice(deviceManager));
			wwwBundleDevice = new WwwBundleDevice(deviceManager, baseUri);
			deviceManager.Mount(wwwBundleDevice);
			deviceManager.Mount(new FileBytesDevice(deviceManager));
			deviceManager.Mount(new BytesDevice(deviceManager));
			deviceManager.Mount(new JsonDevice(deviceManager));
			deviceManager.Mount(new StringDevice(deviceManager));
		}

		public static AssetRequest<TAsset> LoadAsync<TAsset>(TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			if (key == null)
			{
				throw new InvalidOperationException("Content system cannot load a null key");
			}
			string key2 = key.Expand(args);
			return LoadAsync<TAsset>(key2);
		}

		public static AssetRequest<TAsset> LoadAsync<TAsset>(AssetLoadedHandler<TAsset> handler, TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			string key2 = key.Expand(args);
			return LoadAsync(key2, handler);
		}

		public static bool TryLoadAsync<TAsset>(out AssetRequest<TAsset> result, TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			string key2 = key.Expand(args);
			if (ContainsKey(key2))
			{
				result = LoadAsync<TAsset>(key2);
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryLoadAsync<TAsset>(out AssetRequest<TAsset> result, AssetLoadedHandler<TAsset> handler, TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			string key2 = key.Expand(args);
			if (ContainsKey(key2))
			{
				result = LoadAsync(key2, handler);
				return true;
			}
			result = null;
			return false;
		}

		public static AssetRequest<TAsset> LoadAsync<TAsset>(string key) where TAsset : class
		{
			return Service.Get<Content>().loadAsync<TAsset>(key.ToLower());
		}

		public static AssetRequest<TAsset> LoadAsync<TAsset>(string key, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			return Service.Get<Content>().loadAsync(key.ToLower(), handler);
		}

		public static TAsset LoadImmediate<TAsset>(TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			string key2 = key.Expand(args);
			return LoadImmediate<TAsset>(key2);
		}

		public static bool TryLoadImmediate<TAsset>(out TAsset result, TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			string key2 = key.Expand(args);
			if (ContainsKey(key2))
			{
				result = LoadImmediate<TAsset>(key2);
				return true;
			}
			result = null;
			return false;
		}

		public static TAsset LoadImmediate<TAsset>(string key) where TAsset : class
		{
			return Service.Get<Content>().loadImmediate<TAsset>(key.ToLower());
		}

		public static bool ContainsKey(string key)
		{
			ContentManifest contentManifest = Service.Get<Content>().manifest;
			if (contentManifest != null)
			{
				return contentManifest.AssetEntryMap.ContainsKey(key.ToLower());
			}
			return false;
		}

		public static bool ContainsKey(AssetContentKey key, params string[] args)
		{
			string key2 = key.Expand(args);
			return ContainsKey(key2);
		}

		public static bool Unload<TAsset>(TypedAssetContentKey<TAsset> key, params string[] args) where TAsset : class
		{
			string key2 = key.Expand(args);
			return Service.Get<Content>().unload<TAsset>(key2, false);
		}

		public static bool Unload<TAsset>(string key, bool unloadAllObjects = false) where TAsset : class
		{
			if (ContainsKey(key))
			{
				return Service.Get<Content>().unload<TAsset>(key, unloadAllObjects);
			}
			return false;
		}

		public static bool TryPinBundle(string assetKey)
		{
			return trySetPinBundle(assetKey, true);
		}

		public static bool TryUnpinBundle(string assetKey)
		{
			return trySetPinBundle(assetKey, false);
		}

		private static bool trySetPinBundle(string assetKey, bool isPinned)
		{
			Content content = Service.Get<Content>();
			ContentManifest.AssetEntry assetEntry = content.getAssetEntry(assetKey.ToLower());
			if (!string.IsNullOrEmpty(assetEntry.BundleKey))
			{
				BundleMount mount;
				if (content.bundleManager.TryGetBundle(assetEntry.BundleKey, out mount))
				{
					mount.IsPinned = isPinned;
					return true;
				}
				Log.LogErrorFormatted(typeof(Content), "Tried to {2} bundle '{0}' for key '{1}', but bundle was not mounted in BundleManager", assetEntry.BundleKey, assetKey, isPinned ? "pin" : "unpin");
			}
			return false;
		}

		private AssetRequest<TAsset> loadAsync<TAsset>(string key, AssetLoadedHandler<TAsset> handler = null) where TAsset : class
		{
			ContentManifest.AssetEntry entry = getAssetEntry(key);
			TAsset indexedAsset = getIndexedAsset<TAsset>(entry.Key);
			AssetRequest<TAsset> assetRequest;
			CoroutineReturn value;
			if (indexedAsset != null)
			{
				assetRequest = new IndexedAssetRequest<TAsset>(entry.Key, indexedAsset);
				if (handler != null)
				{
					CoroutineRunner.StartPersistent(waitOneFrameBeforeCallingHandler(key, indexedAsset, handler), this, "waitOneFrameBeforeCallingHandler");
				}
			}
			else if (!activeRequests.TryGetValue(entry.Key, out value))
			{
				activeRequestHandlers.Add(entry.Key, new List<object>());
				assetRequest = loadAsyncEntry(ref entry, handler);
				activeRequests.Add(entry.Key, assetRequest);
			}
			else
			{
				if (handler != null)
				{
					activeRequestHandlers[entry.Key].Add(handler);
				}
				assetRequest = (AssetRequest<TAsset>)value;
			}
			if (assetRequest == null)
			{
				Log.LogError(this, "Failed to load " + key);
			}
			return assetRequest;
		}

		private ContentManifest.AssetEntry getAssetEntry(string key)
		{
			if (manifest != null)
			{
				ContentManifest.AssetEntry value;
				if (manifest.AssetEntryMap.TryGetValue(key, out value))
				{
					if (value.IsLocalized)
					{
						string key2 = key + "_" + currentLanguage.ToLower();
						if (!manifest.AssetEntryMap.ContainsKey(key2))
						{
							if (string.IsNullOrEmpty(value.DefaultLocale))
							{
								throw new InvalidOperationException("A localized asset entry must have a default locale specified. Key: " + key);
							}
							key2 = key + "_" + value.DefaultLocale.ToLower();
						}
						return getAssetEntry(key2);
					}
					return value;
				}
				throw new ArgumentException("Cannot find asset entry in manifest AssetEntryMap. Key: " + key);
			}
			throw new ArgumentException(string.Format("Cannot find asset entry in manifest for key: {0}. Manifest: {1}.", key, manifest));
		}

		private IEnumerator waitOneFrameBeforeCallingHandler<TAsset>(string key, TAsset indexedAsset, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			yield return null;
			handler(key, indexedAsset);
		}

		private AssetRequest<TAsset> loadAsyncEntry<TAsset>(ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null) where TAsset : class
		{
			bool flag = entry.Extension.Equals("unity");
			if (!flag)
			{
				assetIndex.Reserve(entry.Key);
			}
			try
			{
				AssetRequest<TAsset> assetRequest = deviceManager.LoadAsync(entry.DeviceList, ref entry, handler);
				CoroutineRunner.StartPersistent(waitForLoadToFinish(assetRequest, flag), this, "waitForLoadToFinish");
				return assetRequest;
			}
			catch (Exception)
			{
				assetIndex.Remove(entry.Key);
				throw;
			}
		}

		private IEnumerator waitForLoadToFinish<TAsset>(AssetRequest<TAsset> request, bool isSceneLoadRequest) where TAsset : class
		{
			int currentFrame = Time.frameCount;
			yield return request;
			if (currentFrame == Time.frameCount)
			{
				yield return null;
			}
			if (request.Cancelled)
			{
				yield break;
			}
			activeRequests.Remove(request.Key);
			if (!isSceneLoadRequest)
			{
				if (!assetIndex.IsReserved(request.Key))
				{
					throw new InvalidOperationException("Asset key was not reserved in index: " + request.Key);
				}
				if (request.Asset == null)
				{
					assetIndex.Remove(request.Key);
					throw new InvalidOperationException("Asset failed to load for key: " + request.Key);
				}
				assetIndex.Add(request.Key, request.Asset);
			}
			List<string> exceptions = null;
			foreach (object item in activeRequestHandlers[request.Key])
			{
				try
				{
					AssetLoadedHandler<TAsset> assetLoadedHandler = (AssetLoadedHandler<TAsset>)item;
					assetLoadedHandler(request.Key, request.Asset);
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Asset request handler for key '" + request.Key + "' threw an exception: " + ex.Message);
					Log.LogException(this, ex);
					if (exceptions == null)
					{
						exceptions = new List<string>();
					}
					exceptions.Add(ex.ToString());
				}
			}
			activeRequestHandlers.Remove(request.Key);
			if (exceptions == null)
			{
				yield break;
			}
			StringBuilder stringBuilder = new StringBuilder("Asset request handler(s) for key '" + request.Key + "' threw one or more exceptions:\n");
			foreach (string item2 in exceptions)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(item2);
			}
			throw new InvalidOperationException(stringBuilder.ToString());
		}

		private TAsset loadImmediate<TAsset>(string key) where TAsset : class
		{
			ContentManifest.AssetEntry entry = getAssetEntry(key);
			TAsset val = getIndexedAsset<TAsset>(entry.Key);
			if (val == null)
			{
				val = deviceManager.LoadImmediate<TAsset>(entry.DeviceList, ref entry);
				assetIndex.Add(entry.Key, val);
			}
			return val;
		}

		private TAsset getIndexedAsset<TAsset>(string key) where TAsset : class
		{
			TAsset val = assetIndex.Get<TAsset>(key);
			if (val is UnityEngine.Object && (UnityEngine.Object)(object)val == null)
			{
				assetIndex.Remove(key);
				val = null;
			}
			return val;
		}

		private bool unload<TAsset>(string key, bool unloadAllObjects) where TAsset : class
		{
			key = key.ToLower();
			ContentManifest.AssetEntry assetEntry = getAssetEntry(key);
			TAsset indexedAsset = getIndexedAsset<TAsset>(key);
			if (indexedAsset != null)
			{
				string bundleKey = assetEntry.BundleKey;
				if (!string.IsNullOrEmpty(bundleKey) && bundleManager.IsMounted(assetEntry.BundleKey))
				{
					bundleManager.UnmountBundle(bundleKey, unloadAllObjects);
				}
				assetIndex.Remove(assetEntry.Key);
				return true;
			}
			return false;
		}
	}
}
