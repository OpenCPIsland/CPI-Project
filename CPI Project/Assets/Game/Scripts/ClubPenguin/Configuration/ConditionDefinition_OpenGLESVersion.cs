using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/OpenGLESVersion")]
	public class ConditionDefinition_OpenGLESVersion : ConditionDefinition
	{
		public string[] OpenGLESVersionContains;

		public override bool IsSatisfied()
		{
			string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
			bool flag = false;
			int num = OpenGLESVersionContains.Length;
			for (int i = 0; i < num; i++)
			{
				flag |= graphicsDeviceVersion.Contains(OpenGLESVersionContains[i]);
			}
			return flag;
		}
	}
}
