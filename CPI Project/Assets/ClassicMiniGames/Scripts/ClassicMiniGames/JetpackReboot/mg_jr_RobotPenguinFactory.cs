using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_RobotPenguinFactory : mg_jr_GameObjectFactory
	{
		public override GameObject Create()
		{
			GameObject gameObject = null;
			gameObject = base.Resources.GetInstancedResource(mg_jr_ResourceList.GAME_PREFAB_ROBOT_PENGUIN);
			Assert.NotNull(gameObject, "No gameobject created.");
			mg_jr_RobotPenguin component = gameObject.GetComponent<mg_jr_RobotPenguin>();
			component.Init(1);
			gameObject.AddComponent<mg_jr_DestroyWhenPastScreen>();
			gameObject.AddComponent<mg_jr_Follow>();
			gameObject.AddComponent<mg_jr_Collector>();
			gameObject.AddComponent<mg_jr_Blinker>();
			mg_jr_ObstacleDestroyer mg_jr_ObstacleDestroyer = gameObject.AddComponent<mg_jr_ObstacleDestroyer>();
			mg_jr_ObstacleDestroyer.enabled = false;
			GameObject gameObject2 = gameObject.transform.Find("mg_jr_pf_robot_penguin_actual").gameObject;
			SpriteRenderer component2 = gameObject2.GetComponent<SpriteRenderer>();
			component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
			return gameObject;
		}
	}
}
