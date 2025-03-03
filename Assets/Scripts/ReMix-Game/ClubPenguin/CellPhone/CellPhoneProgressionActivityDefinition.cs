using ClubPenguin.Adventure;
using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneProgressionActivityDefinition : CellPhoneActivityDefinition
	{
		public Mascot Mascot;

		public string TipToken;

		public Reward RewardItems;
	}
}
