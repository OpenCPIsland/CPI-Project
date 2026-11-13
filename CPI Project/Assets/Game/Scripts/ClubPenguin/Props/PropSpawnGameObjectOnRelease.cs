using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	[DisallowMultipleComponent]
	public class PropSpawnGameObjectOnRelease : MonoBehaviour
	{
		[SerializeField]
		private PrefabContentKey GameObjectContentKey;

		[SerializeField]
		private bool ParentToUser = false;

		private Prop prop;

		private void Awake()
		{
			prop = GetComponent<Prop>();
			prop.EActionEventReceived += onActionEventReceived;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (prop != null)
			{
				prop.EActionEventReceived -= onActionEventReceived;
			}
		}

		private void onActionEventReceived(string actionEvent)
		{
			if (actionEvent == "release")
			{
				Content.LoadAsync(OnPropEffectSpawned, GameObjectContentKey);
			}
		}

		private void OnPropEffectSpawned(string path, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			if (ParentToUser && prop.PropUserRef != null)
			{
				gameObject.transform.SetParent(prop.PropUserRef.transform, false);
				CameraCullingMaskHelper.SetLayerRecursive(gameObject.transform, LayerMask.LayerToName(prop.PropUserRef.transform.gameObject.layer));
			}
		}
	}
}
