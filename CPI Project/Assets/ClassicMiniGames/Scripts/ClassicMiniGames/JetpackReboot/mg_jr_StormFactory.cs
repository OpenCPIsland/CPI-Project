using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_StormFactory : mg_jr_ObstacleFactory
	{
		public mg_jr_StormFactory()
			: base(mg_jr_ResourceList.GAME_PREFAB_STORMY_CLOUD)
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
				case "Cloud":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
					break;
				case "Effect":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_1);
					break;
				case "Lightning":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_2);
					break;
				default:
					Assert.IsTrue(false, "Stormy cloud - unknown renderer object name: " + spriteRenderer.gameObject.name);
					break;
				}
			}
			mg_jr_PlayOnAppearance mg_jr_PlayOnAppearance = gameObject.AddComponent<mg_jr_PlayOnAppearance>();
			mg_jr_PlayOnAppearance.SfxToPlay = mg_jr_Sound.STORM_CLOUD.ClipName();
			return gameObject;
		}
	}
}
