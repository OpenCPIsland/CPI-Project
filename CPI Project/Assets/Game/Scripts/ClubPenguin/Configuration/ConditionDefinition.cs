using UnityEngine;

namespace ClubPenguin.Configuration
{
	public abstract class ConditionDefinition : ScriptableObject
	{
		public abstract bool IsSatisfied();
	}
}
