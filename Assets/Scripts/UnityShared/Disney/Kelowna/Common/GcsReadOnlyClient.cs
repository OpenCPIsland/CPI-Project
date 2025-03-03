using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common
{
	public class GcsReadOnlyClient
	{
		public class ReadResponse
		{
			public string Data;

			public bool HasError;

			public long ErrorCode;
		}

		protected readonly string bucket;

		protected readonly IGcsAccessTokenService gcsAccessTokenService;

		public GcsReadOnlyClient(string bucket, IGcsAccessTokenService gcsAccessTokenService)
		{
			this.bucket = bucket;
			this.gcsAccessTokenService = gcsAccessTokenService;
			gcsAccessTokenService.AccessType = GcsAccessType.READ_ONLY;
		}

		private string getReadAssetUrl(string assetName, string accessToken)
		{
			return string.Format("https://www.googleapis.com/storage/v1/b/{0}/o/{1}?alt=media&access_token={2}", bucket, Uri.EscapeDataString(assetName), accessToken);
		}

		public IEnumerator ReadJson(string assetName, ReadResponse response)
		{
			GcsAccessTokenResponse gcsAccessTokenResponse = new GcsAccessTokenResponse();
			yield return gcsAccessTokenService.GetAccessToken(gcsAccessTokenResponse);
			yield return followRedirects(getReadAssetUrl(assetName, gcsAccessTokenResponse.AccessToken), response);
		}

		private IEnumerator followRedirects(string url, ReadResponse response)
		{
			UnityWebRequest request = UnityWebRequest.Get(url);
			request.redirectLimit = 0;
			yield return request.SendWebRequest();
			while (!request.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(request.GetResponseHeader("LOCATION")))
			{
				yield return followRedirects(request.GetResponseHeader("LOCATION"), response);
			}
			else if (request.isHttpError || request.isNetworkError)
			{
				Log.LogErrorFormatted(this, "GCS request to {0} failed with error: {1}", request.url, request.error);
				response.HasError = true;
				response.ErrorCode = request.responseCode;
			}
			else if (request.responseCode == 200)
			{
				response.Data = request.downloadHandler.text;
			}
			else
			{
				response.HasError = true;
				response.ErrorCode = request.responseCode;
				Log.LogErrorFormatted(this, "GCS request to {0} failed with status: {1} and body: {2} and error: {3}", request.url, request.responseCode, request.downloadHandler.text, request.error);
			}
		}
	}
}
