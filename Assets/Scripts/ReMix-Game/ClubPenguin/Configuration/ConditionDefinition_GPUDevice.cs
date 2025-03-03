using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/DeviceGPU")]
	public class ConditionDefinition_GPUDevice : ConditionDefinition
	{
		public string[] DeviceModelContains;

		public override bool IsSatisfied()
		{
			string graphicsDeviceName = SystemInfo.graphicsDeviceName;
			bool flag = false;
			int num = DeviceModelContains.Length;
			for (int i = 0; i < num; i++)
			{
				flag |= graphicsDeviceName.Contains(DeviceModelContains[i]);
			}
			return flag;
		}
	}
}
