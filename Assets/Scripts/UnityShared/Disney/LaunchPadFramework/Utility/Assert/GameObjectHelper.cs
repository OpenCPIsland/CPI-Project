using UnityEngine;

namespace Disney.LaunchPadFramework.Utility.Assert
{
	public class GameObjectHelper
	{
		public static Component GetComponentAndAssert(string gameObjectName, string gameComponentName)
		{
			GameObject gameObject = GameObject.Find(gameObjectName);
			return gameObject.GetComponent(gameComponentName);
		}

		public static Component GetComponentAndAssert(string gameObjectName)
		{
			GameObject gameObject = GameObject.Find(gameObjectName);
			return gameObject.GetComponent(gameObjectName);
		}
	}
}
