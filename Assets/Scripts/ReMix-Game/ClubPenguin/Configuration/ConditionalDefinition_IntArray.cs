using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Definition/IntArray")]
	public class ConditionalDefinition_IntArray : ConditionalDefinition<int[]>
	{
		public ConditionalTier_IntArray[] _Tiers;

		public override ConditionalTier<int[]>[] Tiers
		{
			get
			{
				return _Tiers;
			}
		}
	}
}
