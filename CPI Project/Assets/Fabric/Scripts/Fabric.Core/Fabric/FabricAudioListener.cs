using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/AudioListener")]
	public class FabricAudioListener : MonoBehaviour
	{
		private void Start()
		{
			FabricManager instance = FabricManager.Instance;
			if (instance != null)
			{
				FabricManager.Instance._audioListener = base.gameObject.AddComponent<AudioListener>();
			}
		}

		private void OnDestroy()
		{
			if (FabricManager.Instance != null)
			{
				Object.Destroy(FabricManager.Instance._audioListener);
			}
		}
	}
}
