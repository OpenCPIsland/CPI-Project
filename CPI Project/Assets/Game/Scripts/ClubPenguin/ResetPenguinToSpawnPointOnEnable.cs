using UnityEngine;

namespace ClubPenguin
{
	internal class ResetPenguinToSpawnPointOnEnable : MonoBehaviour
	{
		public void OnEnable()
		{
			if (SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject != null)
			{
				PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
				if (component != null)
				{
					component.spawnAtSceneLocation();
				}
			}
		}
	}
}
