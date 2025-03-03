using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public static class SavedOutfitServiceEvents
	{
		public struct SavedOutfitUpdated
		{
			public readonly int OutfitSlot;

			public SavedOutfitUpdated(int outfitSlot)
			{
				OutfitSlot = outfitSlot;
			}
		}

		public struct SavedOutfitsLoaded
		{
			public readonly List<SavedOutfit> SavedOutfits;

			public SavedOutfitsLoaded(List<SavedOutfit> savedOutfits)
			{
				SavedOutfits = savedOutfits;
			}
		}

		public struct SavedOutfitDeleted
		{
			public readonly int OutfitSlot;

			public SavedOutfitDeleted(int outfitSlot)
			{
				OutfitSlot = outfitSlot;
			}
		}
	}
}
