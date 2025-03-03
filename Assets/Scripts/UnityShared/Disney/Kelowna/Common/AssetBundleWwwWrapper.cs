using UnityEngine;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common
{
	public class AssetBundleWwwWrapper : CoroutineReturn
	{
		private const uint CACHE_VERSION_NUMBER = 1u;

		private UnityWebRequest webRequest;

		private AssetBundle cachedAssetBundle;

		private bool isDisposed;

		private string bundlePath;

		private IGcsAccessTokenService gcsAccessTokenService;

		internal UnityWebRequest WebRequest
		{
			get
			{
				return webRequest;
			}
		}

		public bool IsComplete
		{
			get;
			set;
		}

		public string BundlePath
		{
			get
			{
				return bundlePath;
			}
		}

		public IGcsAccessTokenService GcsAccessTokenService
		{
			get
			{
				return gcsAccessTokenService;
			}
		}

		public AssetBundle AssetBundle
		{
			get
			{
				if (cachedAssetBundle == null && webRequest == null)
				{
					return null;
				}
				if (cachedAssetBundle == null && !isDisposed && webRequest.isDone && !webRequest.isNetworkError)
				{
					cachedAssetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
					if (!(cachedAssetBundle == null))
					{
					}
				}
				return cachedAssetBundle;
			}
		}

		public override bool Finished
		{
			get
			{
				if (isDisposed && IsComplete)
				{
					return true;
				}
				if (webRequest == null)
				{
					return false;
				}
				return webRequest.isDone && IsComplete;
			}
		}

		public AssetBundleWwwWrapper(string bundlePath, IGcsAccessTokenService gcsAccessTokenService)
		{
			this.bundlePath = bundlePath;
			this.gcsAccessTokenService = gcsAccessTokenService;
		}

		public void LoadFromCacheOrDownload(string url)
		{
			webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, 1u, 0u);
		}

		public void LoadFromCacheOrDownload(string url, uint crc)
		{
			webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, 1u, crc);
		}

		public void LoadFromDownload(string url)
		{
			webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
		}

		public void LoadFromDownload(string url, uint crc)
		{
			webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, crc);
		}

		public AsyncOperation Send()
		{
			return webRequest.SendWebRequest();
		}

		public void CacheAndDispose()
		{
			cachedAssetBundle = AssetBundle;
			isDisposed = true;
			if (webRequest != null)
			{
				webRequest.Dispose();
			}
		}
	}
}
