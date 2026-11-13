using ClubPenguin.Cinematography;
using UnityEngine;

namespace ClubPenguin.Props
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Prop))]
	public class PropSpawnOnUse : MonoBehaviour
	{
		public PropExperience PrefabToSpawn;

		public bool IsInstance = false;

		public bool HideInitially = false;

		public bool ParentToUser = false;

		public bool ApplyUserForwardVect = true;

		private Prop prop;

		public PropExperience SpawnedInstance
		{
			get;
			private set;
		}

		private void Awake()
		{
			prop = GetComponent<Prop>();
			prop.EUsed += onPropUsed;
		}

		private void OnDestroy()
		{
			prop.EUsed -= onPropUsed;
		}

		private void onPropUsed()
		{
			SpawnedInstance = (IsInstance ? PrefabToSpawn : Object.Instantiate(PrefabToSpawn));
			if (ParentToUser)
			{
				SpawnedInstance.transform.SetParent(prop.PropUserRef.transform, false);
			}
			else
			{
				SpawnedInstance.transform.position = prop.OnUseDestination;
				CameraCullingMaskHelper.SetLayerRecursive(SpawnedInstance.transform, "AllPlayerInteractibles");
			}
			SpawnedInstance.InstanceId = prop.ExperienceInstanceId;
			SpawnedInstance.OwnerId = prop.OwnerId;
			SpawnedInstance.IsOwnerLocalPlayer = prop.IsOwnerLocalPlayer;
			SpawnedInstance.PropDef = prop.PropDef;
			if (HideInitially)
			{
				SpawnedInstance.gameObject.SetActive(false);
			}
			else
			{
				SpawnedInstance.StartExperience();
			}
			if (ApplyUserForwardVect)
			{
				SpawnedInstance.transform.forward = prop.PropUserRef.transform.forward;
			}
		}
	}
}
