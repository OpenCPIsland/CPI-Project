using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct PlayerAssets : IOfflineData
	{
		public ClubPenguin.Net.Domain.PlayerAssets Assets;

		public int IglooSlots;

		public void Init()
		{
			Assets = new ClubPenguin.Net.Domain.PlayerAssets();
			Assets.mascotXP = new Dictionary<string, long>();
			Assets.collectibleCurrencies = new Dictionary<string, int>();
			Assets.colourPacks = new List<string>();
			Assets.decals = new List<int>();
			Assets.fabrics = new List<int>();
			Assets.emotePacks = new List<string>();
			Assets.sizzleClips = new List<int>();
			Assets.equipmentTemplates = new List<int>();
			Assets.lots = new List<string>();
			Assets.decorations = new List<int>();
			Assets.structures = new List<int>();
			Assets.musicTracks = new List<int>();
			Assets.lighting = new List<int>();
			Assets.durables = new List<int>();
			Assets.partySupplies = new List<int>();
			Assets.tubes = new List<int>();
		}
	}
}
