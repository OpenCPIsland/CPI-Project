using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Screen Height")]
	public class ConditionDefinition_ScreenHeight : ConditionDefinition
	{
		public int GreaterThanEqualToHeight;

		public override bool IsSatisfied()
		{
			int height = Screen.height;
			return height >= GreaterThanEqualToHeight;
		}
	}
}
