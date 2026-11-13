using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Nested Condition Definition")]
	public class NestedConditionDefinition : ConditionDefinition
	{
		[Tooltip("Flag to determine if this conditions should be AND/OR when checking to use this.")]
		public Operators ConditionOperation = Operators.AND;

		public ConditionDefinition[] Conditions;

		public override bool IsSatisfied()
		{
			bool result = false;
			switch (ConditionOperation)
			{
			case Operators.AND:
				result = ANDConditions(this);
				break;
			case Operators.OR:
				result = ORConditions(this);
				break;
			}
			return result;
		}

		private bool ANDConditions(NestedConditionDefinition tier)
		{
			bool flag = true;
			for (int i = 0; i < tier.Conditions.Length; i++)
			{
				flag &= tier.Conditions[i].IsSatisfied();
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		private bool ORConditions(NestedConditionDefinition tier)
		{
			bool flag = false;
			for (int i = 0; i < tier.Conditions.Length; i++)
			{
				flag |= tier.Conditions[i].IsSatisfied();
				if (flag)
				{
					break;
				}
			}
			return flag;
		}
	}
}
