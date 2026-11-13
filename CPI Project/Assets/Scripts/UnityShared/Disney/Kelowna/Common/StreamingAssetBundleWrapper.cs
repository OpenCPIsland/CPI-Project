using UnityEngine;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common
{
    public class StreamingAssetBundleWrapper : CoroutineReturn
    {
        private UnityWebRequest webRequest;
        private AssetBundle assetBundle;

        public UnityWebRequest WebRequest
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
                if (webRequest == null || !webRequest.isDone)
                {
                    return null;
                }
                if (assetBundle == null)
                {
                    assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                }
                return assetBundle;
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
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            webRequest.SendWebRequest();
        }

        public void Dispose()
        {
            if (webRequest != null)
            {
                webRequest.Dispose();
                webRequest = null;
            }
        }
    }
}