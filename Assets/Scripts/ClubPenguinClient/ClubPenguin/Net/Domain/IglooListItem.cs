using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct IglooListItem
	{
		public OtherPlayerData ownerData;

		public RoomPopulationScale? iglooPopulation;
	}
}
