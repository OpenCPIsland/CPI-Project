using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Obstacle : MonoBehaviour
	{
		public const float BIG_OBSTACLE_SIZE = 1.5f;

		public const int BIG_OBSTACLE_COIN_VALUE = 4;

		public const int SMALL_OBSTACLE_COIN_VALUE = 1;

		public int CoinValue
		{
			get
			{
				return (!IsBig) ? 1 : 4;
			}
		}

		public bool IsBig
		{
			get
			{
				return WidestBounds().size.x >= 1.5f;
			}
		}

		private Bounds WidestBounds()
		{
			Bounds result = default(Bounds);
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(true);
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				if (spriteRenderer.bounds.size.x > result.size.x)
				{
					result = spriteRenderer.bounds;
				}
			}
			return result;
		}

		public virtual void Explode()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			mg_jr_Resources resources = active.Resources;
			GameObject pooledResource = resources.GetPooledResource(mg_jr_ResourceList.GAME_PREFAB_EFFECT_OBSTACLE_EXPLOSION);
			Bounds bounds = WidestBounds();
			if (bounds.size.x < 1.5f)
			{
				pooledResource.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			}
			SpriteRenderer component = pooledResource.GetComponent<SpriteRenderer>();
			Assert.NotNull(component, "Explosion effect must have a spriterenderer");
			component.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
			Vector3 center = component.bounds.center;
			Vector3 b = pooledResource.transform.position - center;
			pooledResource.transform.position = bounds.center + b;
			pooledResource.transform.parent = active.GameLogic.transform;
			if (Random.value >= 0.5f)
			{
				active.PlaySFX(mg_jr_Sound.OBSTACLE_EXPLODE_01.ClipName());
			}
			else
			{
				active.PlaySFX(mg_jr_Sound.OBSTACLE_EXPLODE_02.ClipName());
			}
			resources.ReturnPooledResource(base.gameObject);
		}
	}
}
