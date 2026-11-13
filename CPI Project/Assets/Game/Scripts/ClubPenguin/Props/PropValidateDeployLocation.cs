using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	[DisallowMultipleComponent]
	public class PropValidateDeployLocation : MonoBehaviour
	{
		[Tooltip("The prefab to use for validation, must have a PropSpawnLocationValidator component")]
		public PrefabContentKey ValidatorContentKey;

		private PropSpawnLocationValidator validator;

		public event Action<PropSpawnLocationValidator> OnValidatorSpawned;

		public IEnumerator Start()
		{
			GameObject localPlayer = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			PropUser propUser = GetComponentInParent<PropUser>();
			if (propUser.gameObject != localPlayer)
			{
				UnityEngine.Object.Destroy(this);
				yield break;
			}
			AssetRequest<GameObject> request = Content.LoadAsync(ValidatorContentKey);
			yield return request;
			validator = UnityEngine.Object.Instantiate(request.Asset).GetComponent<PropSpawnLocationValidator>();
			if (this.OnValidatorSpawned != null)
			{
				this.OnValidatorSpawned(validator);
			}
		}

		private void OnDestroy()
		{
			if (validator != null)
			{
				UnityEngine.Object.Destroy(validator.gameObject);
			}
		}

		public PropSpawnLocationValidator GetValidator()
		{
			return validator;
		}
	}
}
