#if !UNITY_WEBGL
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Net.Lan
{
	public class LanAutoStart : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Create()
		{
			if (PlayerPrefs.GetInt("LAN.Host", 0) != 1 && PlayerPrefs.GetInt("LAN.Join", 0) != 1)
			{
				return;
			}
			GameObject go = new GameObject("LanAutoStart");
			go.hideFlags = HideFlags.HideAndDontSave;
			Object.DontDestroyOnLoad(go);
			go.AddComponent<LanAutoStart>();
		}

		private IEnumerator Start()
		{
			while (!LanTweaks.RestorePrefs())
			{
				yield return new WaitForSeconds(1f);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
#endif
