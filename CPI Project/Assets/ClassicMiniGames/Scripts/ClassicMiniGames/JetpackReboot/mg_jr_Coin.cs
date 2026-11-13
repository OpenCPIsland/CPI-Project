using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Coin : mg_jr_Collectable
	{
		public override void OnCollection()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			active.Resources.ReturnPooledResource(base.gameObject);
			GameObject instancedResource = active.Resources.GetInstancedResource(mg_jr_ResourceList.GAME_PREFAB_EFFECT_COIN_PICKUP);
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			SpriteRenderer component2 = instancedResource.GetComponent<SpriteRenderer>();
			Assert.NotNull(component2, "Effect must have a spriterenderer");
			component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
			Vector3 center = component2.bounds.center;
			Vector3 b = instancedResource.transform.position - center;
			if (component != null)
			{
				instancedResource.transform.position = component.bounds.center + b;
			}
			else
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(base.gameObject, "Gameobject" + base.gameObject.name + "spawning coin pickup effect has no spriterenderer.");
				instancedResource.transform.position = base.transform.position + b;
			}
			if (base.Quantity == 10)
			{
				active.PlaySFX(mg_jr_Sound.PICKUP_COIN_10.ClipName());
			}
			else
			{
				active.PlaySFX(mg_jr_Sound.PICKUP_COIN.ClipName());
			}
		}
	}
}
