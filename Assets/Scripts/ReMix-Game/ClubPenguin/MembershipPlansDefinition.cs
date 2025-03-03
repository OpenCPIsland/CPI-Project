using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/MembershipPlansData")]
	public class MembershipPlansDefinition : StaticGameDataDefinition
	{
		[Serializable]
		public struct MembershipPlanGroup
		{
			public bool overrideDefaultSKU;

			public string defaultSKU;

			public bool overrideAllSKUs;

			public string[] AllSKUs;

			public bool overrideOfferSKUs;

			public string[] OfferSKUs;
		}

		[StaticGameDataDefinitionId]
		public string Id;

		public string[] Countries;

		public MembershipPlanGroup FirstTime;

		public MembershipPlanGroup Resubscribe;
	}
}
