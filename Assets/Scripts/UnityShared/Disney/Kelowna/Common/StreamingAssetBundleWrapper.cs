using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class StreamingAssetBundleWrapper : CoroutineReturn
	{
		private WWW webRequest;

		public WWW WebRequest
		{
			get
			{
				return webRequest;
			}
		}

		public AssetBundle AssetBundle
		{
			get
			{
				if (webRequest == null)
				{
					return null;
				}
				return webRequest.assetBundle;
			}
		}

		public override bool Finished
		{
			get
			{
				if (webRequest == null)
				{
					return false;
				}
				return webRequest.isDone;
			}
		}

		public void LoadFromDownload(string url)
		{
			webRequest = new WWW(url);
		}

		public void Dispose()
		{
			if (webRequest != null)
			{
				webRequest.Dispose();
			}
		}
	}
}
