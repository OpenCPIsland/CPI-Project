using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_CoinFactory : mg_jr_GameObjectFactory
	{
		public override GameObject Create()
		{
			GameObject gameObject = null;
			gameObject = base.Resources.GetInstancedResource(mg_jr_ResourceList.GAME_PREFAB_SMALL_COIN);
			Assert.NotNull(gameObject, "No gameobject created.");
			mg_jr_Coin component = gameObject.GetComponent<mg_jr_Coin>();
			component.Init(1);
			gameObject.AddComponent<mg_jr_DestroyWhenPastScreen>();
			SpriteRenderer component2 = gameObject.GetComponent<SpriteRenderer>();
			component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
			return gameObject;
		}
	}
}
