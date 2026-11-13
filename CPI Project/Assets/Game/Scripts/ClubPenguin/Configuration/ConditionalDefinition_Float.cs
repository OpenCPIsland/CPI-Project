using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Definition/Float")]
	public class ConditionalDefinition_Float : ConditionalDefinition<float>
	{
		public ConditionalTier_Float[] _Tiers;

		public override ConditionalTier<float>[] Tiers
		{
			get
			{
				return _Tiers;
			}
		}
	}
}
