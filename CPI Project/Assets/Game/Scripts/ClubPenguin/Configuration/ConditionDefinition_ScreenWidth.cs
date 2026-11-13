using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Screen Width")]
	public class ConditionDefinition_ScreenWidth : ConditionDefinition
	{
		public int GreaterThanEqualToWidth;

		public override bool IsSatisfied()
		{
			int width = Screen.width;
			return width >= GreaterThanEqualToWidth;
		}
	}
}
