using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Default")]
	public class ConditionDefinition_Default : ConditionDefinition
	{
		public override bool IsSatisfied()
		{
			return true;
		}
	}
}
