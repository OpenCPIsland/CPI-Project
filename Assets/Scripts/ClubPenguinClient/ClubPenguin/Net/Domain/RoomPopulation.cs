using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class RoomPopulation : IComparable<RoomPopulation>
	{
		public readonly RoomIdentifier identifier;

		public readonly RoomPopulationScale populationScaled;

		public RoomPopulation()
		{
		}

		public RoomPopulation(RoomIdentifier identifier, RoomPopulationScale populationScaled)
		{
			this.identifier = identifier;
			this.populationScaled = populationScaled;
		}

		public int CompareTo(RoomPopulation other)
		{
			return populationScaled.CompareTo(other.populationScaled);
		}
	}
}
