using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
    public string url = "http://www.yourwebsite.com/logo.png";

    private Texture2D mTex;

    private IEnumerator Start()
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                mTex = DownloadHandlerTexture.GetContent(request);

                if (mTex != null)
                {
                    UITexture component = GetComponent<UITexture>();
                    component.mainTexture = mTex;
                    component.MakePixelPerfect();
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (mTex != null)
        {
            Object.Destroy(mTex);
        }
    }
}