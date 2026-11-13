using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Definition/FloatArray")]
	public class ConditionalDefinition_FloatArray : ConditionalDefinition<float[]>
	{
		public ConditionalTier_FloatArray[] _Tiers;

		public override ConditionalTier<float[]>[] Tiers
		{
			get
			{
				return _Tiers;
			}
		}
	}
}
