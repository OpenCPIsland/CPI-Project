using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Device")]
	public class ConditionDefinition_Device : ConditionDefinition
	{
		public string[] DeviceModelContains;

		public override bool IsSatisfied()
		{
			string deviceModel = SystemInfo.deviceModel;
			bool flag = false;
			int num = DeviceModelContains.Length;
			for (int i = 0; i < num; i++)
			{
				flag |= deviceModel.Contains(DeviceModelContains[i]);
			}
			return flag;
		}
	}
}
