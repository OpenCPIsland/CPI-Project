using UnityEngine;

namespace Fabric
{
	public class GetFabricManager
	{
		private static FabricManager fabricManager;

		public static FabricManager Instance()
		{
			if (fabricManager != null)
			{
				return fabricManager;
			}
			if (FabricManager.Instance != null)
			{
				fabricManager = FabricManager.Instance;
			}
			else
			{
				fabricManager = (FabricManager)Object.FindObjectOfType(typeof(FabricManager));
			}
			return fabricManager;
		}
	}
}
