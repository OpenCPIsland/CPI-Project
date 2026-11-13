using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct TransientData
	{
		public string equippedConsumable;

		public string equippedConsumableProperties;

		public object questCurrentObjectiveData;

		public int? questObjectiveState;
	}
}
