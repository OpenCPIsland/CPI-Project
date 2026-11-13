using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct SavedOutfit
	{
		public int outfitSlot;

		public long[] parts;
	}
}
