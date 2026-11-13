using System.Collections.Generic;
using UnityEngine.Networking;

namespace SwrveUnity
{
	public static class CrossPlatformUtils
	{
		public static UnityWebRequest MakeRequest(string url, string requestMethod, byte[] encodedData, Dictionary<string, string> headers)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(url);
			UploadHandlerRaw uploadHandler = new UploadHandlerRaw(encodedData);
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.uploadHandler = uploadHandler;
			unityWebRequest.downloadHandler = downloadHandler;
			unityWebRequest.method = requestMethod;
			List<string> list = new List<string>(headers.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				string value = "";
				if (headers.TryGetValue(text, out value))
				{
					unityWebRequest.SetRequestHeader(text, value);
				}
			}
			return unityWebRequest;
		}
	}
}
