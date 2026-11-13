using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_LanternFactory : mg_jr_ObstacleFactory
	{
		public mg_jr_LanternFactory()
			: base(mg_jr_ResourceList.GAME_PREFAB_CAVE_LANTERN)
		{
		}

		public override GameObject Create()
		{
			GameObject gameObject = base.Create();
			SpriteRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				switch (spriteRenderer.gameObject.name)
				{
				case "mg_jr_pf_cave_lantern":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_1);
					break;
				case "mg_jr_pf_cave_lantern_glow":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
					break;
				default:
					Assert.IsTrue(false, "mg_jr_LanternFactory - unknown renderer object name: " + spriteRenderer.gameObject.name);
					break;
				}
			}
			return gameObject;
		}
	}
}
