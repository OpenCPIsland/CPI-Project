using System;
using UnityEngine;

namespace Fabric
{
	public class Generic
	{
		public static System.Random _random = new System.Random();

		public static void SetGameObjectActive(GameObject gameObject, bool active)
		{
			gameObject.SetActive(active);
		}
	}
}
