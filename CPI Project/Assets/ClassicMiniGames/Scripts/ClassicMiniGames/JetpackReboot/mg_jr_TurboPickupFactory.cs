using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_TurboPickupFactory : mg_jr_GameObjectFactory
	{
		private mg_jr_TurboPickUpData m_data = new mg_jr_TurboPickUpData();

		public override GameObject Create()
		{
			GameObject gameObject = null;
			gameObject = base.Resources.GetInstancedResource(mg_jr_ResourceList.GAME_PREFAB_TURBO_PICKUP);
			Assert.NotNull(gameObject, "No gameobject created.");
			mg_jr_TurboPickup component = gameObject.GetComponent<mg_jr_TurboPickup>();
			component.Init(m_data.PickupValue);
			gameObject.AddComponent<mg_jr_DestroyWhenPastScreen>();
			SpriteRenderer component2 = gameObject.GetComponent<SpriteRenderer>();
			component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
			return gameObject;
		}
	}
}
