using ClubPenguin.Chat;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Props;
using ClubPenguin.Rewards;
using ClubPenguin.Tubes;
using ClubPenguin.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Progression
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Progression/Unlock")]
	public class ProgressionUnlockDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Level;

		public RewardThemeDefinition ThemeDefinition;

		public List<string> colourPacks = new List<string>();

		public DecalDefinition[] decals = new DecalDefinition[0];

		public FabricDefinition[] fabrics = new FabricDefinition[0];

		public EmoteDefinition[] emotePacks = new EmoteDefinition[0];

		public SizzleClipDefinition[] sizzleClips = new SizzleClipDefinition[0];

		public TemplateDefinition[] equipmentTemplates = new TemplateDefinition[0];

		public LotDefinition[] lots = new LotDefinition[0];

		public DecorationDefinition[] decorationPurchaseRights = new DecorationDefinition[0];

		public StructureDefinition[] structurePurchaseRights = new StructureDefinition[0];

		public MusicTrackDefinition[] musicTracks = new MusicTrackDefinition[0];

		public LightingDefinition[] lighting = new LightingDefinition[0];

		public PropDefinition[] durables = new PropDefinition[0];

		public int savedOutfitSlots;

		public int iglooSlots;

		public PropDefinition[] partySupplies = new PropDefinition[0];

		public TubeDefinition[] tubes = new TubeDefinition[0];
	}
}
