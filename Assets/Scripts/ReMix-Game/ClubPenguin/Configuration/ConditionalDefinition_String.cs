using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Definition/String")]
	public class ConditionalDefinition_String : ConditionalDefinition<string>
	{
		public ConditionalTier_String[] _Tiers;

		public override ConditionalTier<string>[] Tiers
		{
			get
			{
				return _Tiers;
			}
		}
	}
}
