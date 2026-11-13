using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SwrveUnity.Helpers
{
    public class UnityWwwHelper
    {
        public static WwwDeducedError DeduceWwwError(UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                SwrveLog.LogError("Request network error: " + request.error + " in " + request.url);
                return WwwDeducedError.NetworkError;
            }
            if (request.GetResponseHeaders() != null)
            {
                string value = null;
                Dictionary<string, string>.Enumerator enumerator = request.GetResponseHeaders().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string key = enumerator.Current.Key;
                    if (string.Equals(key, "X-Swrve-Error", StringComparison.OrdinalIgnoreCase))
                    {
                        request.GetResponseHeaders().TryGetValue(key, out value);
                        break;
                    }
                }
                if (value != null)
                {
                    SwrveLog.LogError("Request response headers [\"X-Swrve-Error\"]: " + value + " at " + request.url);
                    try
                    {
                        if (!string.IsNullOrEmpty(request.downloadHandler.text))
                        {
                            SwrveLog.LogError("Request response headers [\"X-Swrve-Error\"]: " + ((IDictionary<string, object>)Json.Deserialize(request.downloadHandler.text))["message"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        SwrveLog.LogError(ex.Message);
                    }
                    return WwwDeducedError.ApplicationErrorHeader;
                }
            }
            if (!string.IsNullOrEmpty(request.error))
            {
                SwrveLog.LogError("Request network error: " + request.error + " in " + request.url);
                return WwwDeducedError.NetworkError;
            }
            return WwwDeducedError.NoError;
        }
    }
}