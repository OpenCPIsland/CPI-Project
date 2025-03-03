using UnityEngine;

namespace ClubPenguin.Props
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(PropProjectileOnRelease))]
	[RequireComponent(typeof(Prop))]
	[RequireComponent(typeof(PropSpawnOnUse))]
	public class PropProjectileRevealsSpawned : MonoBehaviour
	{
		private PropProjectileOnRelease propProjectileOnRelease;

		private PropSpawnOnUse propSpawnOnUse;

		private void Awake()
		{
			propProjectileOnRelease = GetComponent<PropProjectileOnRelease>();
			propProjectileOnRelease.ProjectileInstanceCreated += onProjectileInstanceCreated;
			propSpawnOnUse = GetComponent<PropSpawnOnUse>();
		}

		private void OnDestroy()
		{
			propProjectileOnRelease.ProjectileInstanceCreated += onProjectileInstanceCreated;
		}

		private void onProjectileInstanceCreated(PropProjectile propProjectile)
		{
			propProjectile.RevealSpawned = true;
			propProjectile.SpawnedPropExperience = propSpawnOnUse.SpawnedInstance;
		}
	}
}
