using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_WhaleFactory : mg_jr_GameObjectFactory
	{
		protected mg_jr_ResourceList m_resourceId;

		public mg_jr_WhaleFactory(EnvironmentVariant _variation)
		{
			m_resourceId = ((_variation == EnvironmentVariant.NIGHT) ? mg_jr_ResourceList.GAME_PREFAB_WATER_WHALE_NIGHT : mg_jr_ResourceList.GAME_PREFAB_WATER_WHALE);
		}

		public override GameObject Create()
		{
			GameObject gameObject = null;
			gameObject = base.Resources.GetInstancedResource(m_resourceId);
			Assert.NotNull(gameObject, "No gameobject created.");
			SpriteRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>();
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				switch (spriteRenderer.gameObject.name)
				{
				case "whale":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
					break;
				case "water":
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_1);
					break;
				default:
					Assert.IsTrue(false, "mg_jr_WhaleFactory - unknown renderer object name: " + spriteRenderer.gameObject.name);
					break;
				}
			}
			mg_jr_PlayOnAppearance mg_jr_PlayOnAppearance = gameObject.AddComponent<mg_jr_PlayOnAppearance>();
			mg_jr_PlayOnAppearance.SfxToPlay = mg_jr_Sound.WHALE.ClipName();
			Collider2D component = gameObject.GetComponent<Collider2D>();
			Assert.NotNull(component, "Whales must have a collider on the root object");
			if (component.gameObject.GetComponent<mg_jr_Whale>() == null)
			{
				component.gameObject.AddComponent<mg_jr_Whale>();
			}
			return gameObject;
		}
	}
}
