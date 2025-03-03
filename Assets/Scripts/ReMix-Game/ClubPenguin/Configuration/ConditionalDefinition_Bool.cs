using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Definition/Bool")]
	public class ConditionalDefinition_Bool : ConditionalDefinition<bool>
	{
		public ConditionalTier_Bool[] _Tiers;

		public override ConditionalTier<bool>[] Tiers
		{
			get
			{
				return _Tiers;
			}
		}
	}
}
