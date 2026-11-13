using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Processor Frequency")]
	public class ConditionDefinition_ProcessorFrequency : ConditionDefinition
	{
		public int LessThanEqualToFrequency;

		public override bool IsSatisfied()
		{
			int processorFrequency = SystemInfo.processorFrequency;
			return processorFrequency <= LessThanEqualToFrequency;
		}
	}
}
