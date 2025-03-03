using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Utils/AssetLoader")]
	public class AssetLoader : MonoBehaviour
	{
		public AssetLoaderItem[] assetsToLoad;

		public AssetLoaderType loadType;

		public EventTrigger[] eventTriggers;

		private void Start()
		{
			if (loadType == AssetLoaderType.Start_Destroy)
			{
				LoadAssets();
			}
		}

		private void OnDestroy()
		{
			if (loadType == AssetLoaderType.Start_Destroy)
			{
				UnloadAssets();
			}
		}

		private void OnTriggerEnter()
		{
			if (loadType == AssetLoaderType.Trigger_Enter_Exit)
			{
				LoadAssets();
			}
		}

		private void OnTriggerExit()
		{
			if (loadType == AssetLoaderType.Trigger_Enter_Exit)
			{
				UnloadAssets();
			}
		}

		private void LoadAssets()
		{
			if (!(GetFabricManager.Instance() == null))
			{
				for (int i = 0; i < assetsToLoad.Length; i++)
				{
					FabricManager.Instance.LoadAsset(assetsToLoad[i].prefabPath + "/" + assetsToLoad[i].prefabToLoad, assetsToLoad[i].destinationPath);
				}
				for (int j = 0; j < eventTriggers.Length; j++)
				{
					eventTriggers[j].PostEvent();
				}
			}
		}

		private void UnloadAssets()
		{
			if (!(GetFabricManager.Instance() == null))
			{
				for (int i = 0; i < assetsToLoad.Length; i++)
				{
					FabricManager.Instance.UnloadAsset(assetsToLoad[i].destinationPath + "_" + assetsToLoad[i].prefabToLoad);
				}
			}
		}
	}
}
