using ClubPenguin.Cinematography;
using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Prop))]
	public class PropProjectileOnRelease : MonoBehaviour
	{
		[SerializeField]
		private PropProjectile ProjectilePrefab;

		private Prop prop;

		public event Action<PropProjectile> ProjectileInstanceCreated;

		private void Awake()
		{
			prop = GetComponent<Prop>();
			prop.EActionEventReceived += onActionEventReceived;
		}

		private void OnDestroy()
		{
			prop.EActionEventReceived -= onActionEventReceived;
			this.ProjectileInstanceCreated = null;
		}

		private void onActionEventReceived(string actionEvent)
		{
			if (actionEvent == "release")
			{
				spawnProjectile();
			}
		}

		private void spawnProjectile()
		{
			PropProjectile propProjectile = UnityEngine.Object.Instantiate(ProjectilePrefab);
			CameraCullingMaskHelper.SetLayerRecursive(propProjectile.transform, "AllPlayerInteractibles");
			propProjectile.WorldStart = prop.transform.position;
			propProjectile.WorldDestination = prop.OnUseDestination;
			propProjectile.transform.forward = prop.PropUserRef.transform.forward;
			if (this.ProjectileInstanceCreated != null)
			{
				this.ProjectileInstanceCreated(propProjectile);
			}
		}
	}
}
