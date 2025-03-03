using Disney.LaunchPadFramework;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class GcsReadWriteClient : GcsReadOnlyClient
	{
		private Dictionary<string, string> headers = new Dictionary<string, string>();

		public GcsReadWriteClient(string bucket, IGcsAccessTokenService gcsAccessTokenService)
			: base(bucket, gcsAccessTokenService)
		{
			gcsAccessTokenService.AccessType = GcsAccessType.READ_WRITE;
			headers.Add("Content-Type", "application/json");
		}

		private string getWriteAssetUrl(string assetName, string accessToken)
		{
			return string.Format("https://www.googleapis.com/upload/storage/v1/b/{0}/o?uploadType=media&name={1}&access_token={2}", bucket, assetName, accessToken);
		}

		public IEnumerator WriteJson(string assetName, string json)
		{
			GcsAccessTokenResponse gcsAccessTokenResponse = new GcsAccessTokenResponse();
			yield return gcsAccessTokenService.GetAccessToken(gcsAccessTokenResponse);
			WWW request = new WWW(getWriteAssetUrl(assetName, gcsAccessTokenResponse.AccessToken), Encoding.UTF8.GetBytes(json), headers);
			yield return request;
			if (!string.IsNullOrEmpty(request.error))
			{
				Log.LogErrorFormatted(this, "GCS request to {0} failed with error: {1}", request.url, request.error);
			}
		}
	}
}
