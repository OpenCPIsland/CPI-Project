#if UNITY_WEBGL
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
    public class StreamingAssetBundleDevice : Device
    {
        public const string DEVICE_TYPE = "sa-bundle";

        private string streamingAssetsPath;

        public override string DeviceType
        {
            get
            {
                return "sa-bundle";
            }
        }

        public StreamingAssetBundleDevice(DeviceManager deviceManager)
            : base(deviceManager)
        {
            streamingAssetsPath = GetStreamingAssetsBaseURL();
            Debug.Log("StreamingAssets Base URL: " + streamingAssetsPath);
        }

        private string GetStreamingAssetsBaseURL()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string streamingAssetsUrl = Application.streamingAssetsPath;
            
            if (!streamingAssetsUrl.EndsWith("/"))
            {
                streamingAssetsUrl += "/";
            }
            
            Debug.Log("WebGL StreamingAssets URL from Application.streamingAssetsPath: " + streamingAssetsUrl);
            return streamingAssetsUrl;
#else
            string dataPath = Application.dataPath;
            
            if (dataPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                if (!dataPath.EndsWith("/"))
                {
                    dataPath += "/";
                }
                return dataPath + "StreamingAssets/";
            }
            else
            {
                string streamingPath = Path.Combine(dataPath, "StreamingAssets");
                streamingPath = streamingPath.Replace('\\', '/');
                return "file:///" + streamingPath + "/";
            }
#endif
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
        {
            StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
            AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
            CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
            return result;
        }

        private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
        {
            string key = entry.Key;

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Asset entry key is null or empty.");
                yield break;
            }

            string url = streamingAssetsPath + key + ".txt";

            url = url.Replace("//StreamingAssets/", "/StreamingAssets/")
                     .Replace(":///", "://");

            Debug.Log($"Loading asset bundle: key='{key}', url='{url}'");

            wrapper.LoadFromDownload(url);
            yield return wrapper.WebRequest;

            AssetBundle assetBundle = wrapper.AssetBundle;
            
            if (assetBundle == null)
            {
                Debug.LogError($"Failed to load AssetBundle from URL: {url}");
            }
            
            if (handler != null)
            {
                handler(key, (TAsset)(object)assetBundle);
            }
            
            yield return null;
        }

        public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
#else
            string path = Path.Combine(Application.streamingAssetsPath, entry.Key + ".txt");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
            {
                throw new InvalidOperationException("Could not read streaming asset bundle '" + path + "' synchronously.");
            }
            return (TAsset)(object)assetBundle;
#endif
        }
    }
}
#else
#if UNITY_ANDROID
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class StreamingAssetBundleDevice : Device
	{
		private string streamingAssetsPath;

		public const string DEVICE_TYPE = "sa-bundle";

		public override string DeviceType
		{
			get
			{
				return "sa-bundle";
			}
		}

		public StreamingAssetBundleDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
            streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets";

            Debug.Log(streamingAssetsPath);
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
			AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
			CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
			return result;
		}

		private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			string key = entry.Key;
			wrapper.LoadFromDownload(Path.Combine(streamingAssetsPath, entry.Key + ".txt"));
			yield return wrapper.WebRequest;
			AssetBundle assetBundle = wrapper.AssetBundle;
			if (handler != null)
			{
				handler(key, (TAsset)(object)assetBundle);
			}
			yield return null;
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
		}
	}
}
#elif UNITY_IOS || UNITY_IPHONE
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class StreamingAssetBundleDevice : Device
	{
		private string streamingAssetsPath;

		public const string DEVICE_TYPE = "sa-bundle";

		public override string DeviceType
		{
			get
			{
				return "sa-bundle";
			}
		}

		public StreamingAssetBundleDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
            // iOS app bundles are case-sensitive; StreamingAssets lives at Data/Raw.
            streamingAssetsPath = "file://" + Application.streamingAssetsPath;
            Debug.Log(streamingAssetsPath);
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
			AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
			CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
			return result;
		}

		private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			string key = entry.Key;
			wrapper.LoadFromDownload(Path.Combine(streamingAssetsPath, entry.Key + ".txt"));
			yield return wrapper.WebRequest;
			AssetBundle assetBundle = wrapper.AssetBundle;
			if (handler != null)
			{
				handler(key, (TAsset)(object)assetBundle);
			}
			yield return null;
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
		}
	}
}
#else
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
    public class StreamingAssetBundleDevice : Device
    {
        public const string DEVICE_TYPE = "sa-bundle";

        private string streamingAssetsPath;

        public override string DeviceType
        {
            get
            {
                return "sa-bundle";
            }
        }

        public StreamingAssetBundleDevice(DeviceManager deviceManager)
            : base(deviceManager)
        {
            streamingAssetsPath = "file://" + Application.streamingAssetsPath;
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
        {
            StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
            AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
            CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
            return result;
        }

        private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
        {
            string key = entry.Key;
            wrapper.LoadFromDownload(Path.Combine(streamingAssetsPath, entry.Key + ".txt"));
            yield return wrapper.WebRequest;
            AssetBundle assetBundle = wrapper.AssetBundle;
            if (handler != null)
            {
                handler(key, (TAsset)(object)assetBundle);
            }
            yield return null;
        }

        public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            
            
            throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
#else
            string path = Path.Combine(Application.streamingAssetsPath, entry.Key + ".txt");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
            {
                throw new InvalidOperationException("Could not read streaming asset bundle '" + path + "' synchronously.");
            }
            return (TAsset)(object)assetBundle;
#endif
        }
    }
}
#endif
#endif
