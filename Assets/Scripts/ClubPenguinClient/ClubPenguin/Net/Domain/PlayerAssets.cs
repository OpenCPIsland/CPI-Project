using LitJson;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	public class PlayerAssets
	{
		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public const long MASCOT_LEVEL_PARTS = 1000000L;

		public int coins;

		public Dictionary<string, long> mascotXP;

		public Dictionary<string, int> collectibleCurrencies;

		public List<string> colourPacks;

		public List<int> decals;

		public List<int> fabrics;

		public List<string> emotePacks;

		public List<int> sizzleClips;

		public List<int> equipmentTemplates;

		public List<string> lots;

		public List<int> decorations;

		public List<int> structures;

		public List<int> musicTracks;

		public List<int> lighting;

		public List<int> durables;

		public List<int> partySupplies;

		public List<int> tubes;

		public int savedOutfitSlots;
	}
}
