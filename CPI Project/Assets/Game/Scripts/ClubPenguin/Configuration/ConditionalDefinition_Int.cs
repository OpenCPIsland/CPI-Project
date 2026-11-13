using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Definition/Int")]
	public class ConditionalDefinition_Int : ConditionalDefinition<int>
	{
		public ConditionalTier_Int[] _Tiers;

		public override ConditionalTier<int>[] Tiers
		{
			get
			{
				return _Tiers;
			}
		}
	}
}
