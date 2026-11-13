using Disney.LaunchPadFramework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common
{
    public class GcsReadWriteClient : GcsReadOnlyClient
    {
        public GcsReadWriteClient(string bucket, IGcsAccessTokenService gcsAccessTokenService)
            : base(bucket, gcsAccessTokenService)
        {
            gcsAccessTokenService.AccessType = GcsAccessType.READ_WRITE;
        }

        private string getWriteAssetUrl(string assetName, string accessToken)
        {
            return string.Format("https://www.googleapis.com/upload/storage/v1/b/{0}/o?uploadType=media&name={1}&access_token={2}", bucket, assetName, accessToken);
        }

        public IEnumerator WriteJson(string assetName, string json)
        {
            GcsAccessTokenResponse gcsAccessTokenResponse = new GcsAccessTokenResponse();
            yield return gcsAccessTokenService.GetAccessToken(gcsAccessTokenResponse);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new UnityWebRequest(getWriteAssetUrl(assetName, gcsAccessTokenResponse.AccessToken), "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Log.LogErrorFormatted(this, "GCS request to {0} failed with error: {1}", request.url, request.error);
            }
        }
    }
}