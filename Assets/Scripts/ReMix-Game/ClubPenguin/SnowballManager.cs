using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin
{
	public class SnowballManager : MonoBehaviour
	{
		private GameObjectPool snowballPool;

		private GameObjectPool smooshedSnowballPool;

		private GameObject[] impactVfxPrefabs;

		private static SnowballManager mInstance;

		public PenguinSnowballThrowData MasterData;

		[Header("ImpactEffects")]
		public SurfaceEffectsData SurfaceImpactData;

		public int PoolCapacity = 32;

		public static SnowballManager Instance
		{
			get
			{
				return mInstance;
			}
		}

		private void Awake()
		{
			if (mInstance == null)
			{
				mInstance = this;
				snowballPool = base.transform.Find("Pool[Snowball]").GetComponent<GameObjectPool>();
				snowballPool.Capacity = PoolCapacity;
				smooshedSnowballPool = base.transform.Find("Pool[SmooshedSnowball]").GetComponent<GameObjectPool>();
				smooshedSnowballPool.Capacity = PoolCapacity;
				impactVfxPrefabs = new GameObject[SurfaceImpactData.Effects.Length];
				for (int i = 0; i < SurfaceImpactData.Effects.Length; i++)
				{
					impactVfxPrefabs[i] = SurfaceImpactData.Effects[i].System.gameObject;
				}
			}
		}

		public PenguinSnowballThrowData GetData()
		{
			return Object.Instantiate(MasterData);
		}

		public SnowballController SpawnSnowball()
		{
			GameObject gameObject = snowballPool.Spawn();
			if (gameObject != null)
			{
				return gameObject.GetComponent<SnowballController>();
			}
			return null;
		}

		public void UnspawnSnowball(SnowballController snowball)
		{
			snowballPool.Unspawn(snowball.gameObject);
		}

		public bool OnSnowballCollision(Collision collision)
		{
			ContactPoint contactPoint = collision.contacts[0];
			return onSnowballImpactColliderAtPoint(collision.collider, contactPoint.point, contactPoint.normal);
		}

		public bool OnSnowballEnterTrigger(Collider collider, Vector3 snowballPos)
		{
			return onSnowballImpactColliderAtPoint(collider, snowballPos, Vector3.up);
		}

		private bool onSnowballImpactColliderAtPoint(Collider collider, Vector3 point, Vector3 normal)
		{
			Quaternion rotation = Quaternion.LookRotation(normal);
			int num = -1;
			int num2 = 1 << collider.gameObject.layer;
			for (int i = 0; i < SurfaceImpactData.Effects.Length; i++)
			{
				SurfaceEffectsData.Effect effect = SurfaceImpactData.Effects[i];
				if ((num2 & (int)effect.SurfaceLayer) != 0)
				{
					if (!string.IsNullOrEmpty(effect.SurfaceTag))
					{
						if (collider.CompareTag(effect.SurfaceTag))
						{
							num = i;
						}
					}
					else
					{
						num = i;
					}
					break;
				}
				if (!string.IsNullOrEmpty(effect.SurfaceTag) && collider.CompareTag(effect.SurfaceTag))
				{
					num = i;
					break;
				}
			}
			if (num > 0)
			{
				if (SurfaceImpactData.Effects[num].UseCollisionHeight)
				{
					smooshedSnowballPool.Spawn(point, rotation);
				}
				Object.Instantiate(impactVfxPrefabs[num], point, rotation);
			}
			else
			{
				Object.Instantiate(impactVfxPrefabs[0], point, rotation);
			}
			return num > 0;
		}
	}
}
