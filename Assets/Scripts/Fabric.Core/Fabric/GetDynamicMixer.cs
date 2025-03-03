using UnityEngine;

namespace Fabric
{
	public class GetDynamicMixer
	{
		private static DynamicMixer dynamicMixer;

		public static DynamicMixer Instance()
		{
			if (dynamicMixer != null)
			{
				return dynamicMixer;
			}
			dynamicMixer = (DynamicMixer)Object.FindObjectOfType(typeof(DynamicMixer));
			return dynamicMixer;
		}
	}
}
