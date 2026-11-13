using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Collider))]
	public class ZoneTransition : MonoBehaviour
	{
		[Header("Use ZoneDefinitionKey, Zone is deprecated")]
		public ZoneDefinitionKey ZoneDefinitionKey;

		public ZoneDefinition Zone;

		public string LoadingScene;

		[Header("Destination Spawn Point Overrides")]
		[Tooltip("Coordinates of the spawn point")]
		public Vector3 DestinationSpawnPoint = Vector3.zero;

		[Tooltip("Rotation values of player when spawned")]
		public Vector3 DestinationSpawnRotation = Vector3.zero;

		private void Awake()
		{
			if (string.IsNullOrEmpty(ZoneDefinitionKey.Id))
			{
				return;
			}
			Dictionary<string, ZoneDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<string, ZoneDefinition>>();
			ZoneDefinition value;
			if (dictionary.TryGetValue(ZoneDefinitionKey.Id, out value))
			{
				if (Zone != null)
				{
				}
				Zone = value;
			}
			else
			{
				Log.LogErrorFormatted(this, "Value for key {0} not found", ZoneDefinitionKey.Id);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(ZoneTransition), null, delegate
				{
					bool flag = false;
					if (DestinationSpawnPoint != Vector3.zero)
					{
						PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
						if (component != null)
						{
							component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(DestinationSpawnPoint).Rotation(Quaternion.Euler(DestinationSpawnRotation)).Zone(Zone).SceneName(Zone.SceneName)
								.SpawnedAction(new SpawnedAction())
								.Build());
							flag = true;
						}
					}
					if (!flag)
					{
						Service.Get<ZoneTransitionService>().LoadZone(Zone, LoadingScene);
					}
				});
			}
		}
	}
}
