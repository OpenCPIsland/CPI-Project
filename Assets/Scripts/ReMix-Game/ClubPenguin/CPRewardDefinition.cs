using ClubPenguin.Chat;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Collectibles;
using ClubPenguin.Consumable;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Durable;
using ClubPenguin.NPC;
using ClubPenguin.Rewards;
using ClubPenguin.Tubes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Reward")]
	[JsonConverter(typeof(RewardDefinitionJsonConverter))]
	public class CPRewardDefinition : RewardDefinition
	{
		public RewardThemeDefinition ThemeDefinition;

		public CoinRewardableDefinition Coins;

		public MascotXPRewardDefinition[] MascotXP;

		public CollectibleRewardDefinition[] Collectibles;

		public ConsumableInstanceRewardDefinition[] ConsumableInstances;

		public DecalRewardDefinition[] Decals;

		public FabricRewardDefinition[] Fabrics;

		public EquipmentTemplateRewardDefinition[] EquipmentTemplates;

		public EquipmentInstanceRewardDefinition[] EquipmentInstances;

		public DecorationInstanceRewardDefinition[] DecorationInstances;

		public DecorationRewardDefinition[] DecorationPurchaseRights;

		public IglooSlotsRewardDefinition IglooSlots;

		public LotRewardDefinition[] Lots;

		public StructureInstanceRewardDefinition[] StructureInstances;

		public StructureRewardDefinition[] StructurePurchaseRights;

		public MusicTrackRewardDefinition[] MusicTracks;

		public LightingRewardDefinition[] Lighting;

		public EmoteRewardDefinition[] EmotePacks;

		public SizzleClipRewardDefinition[] SizzleClips;

		public ConsumableRewardDefinition[] Consumables;

		public DurableRewardDefinition[] Durables;

		public TubeRewardDefinition[] Tubes;

		public override IEnumerator<IRewardableDefinition> GetEnumerator()
		{
			yield return Coins;
			yield return IglooSlots;
			if (MascotXP != null)
			{
				for (int i14 = 0; i14 < MascotXP.Length; i14++)
				{
					yield return MascotXP[i14];
				}
			}
			if (Collectibles != null)
			{
				for (int i13 = 0; i13 < Collectibles.Length; i13++)
				{
					yield return Collectibles[i13];
				}
			}
			if (ConsumableInstances != null)
			{
				for (int i12 = 0; i12 < ConsumableInstances.Length; i12++)
				{
					yield return ConsumableInstances[i12];
				}
			}
			if (EquipmentTemplates != null)
			{
				for (int i11 = 0; i11 < EquipmentTemplates.Length; i11++)
				{
					yield return EquipmentTemplates[i11];
				}
			}
			if (EquipmentInstances != null)
			{
				for (int i10 = 0; i10 < EquipmentInstances.Length; i10++)
				{
					yield return EquipmentInstances[i10];
				}
			}
			if (DecorationInstances != null)
			{
				for (int i9 = 0; i9 < DecorationInstances.Length; i9++)
				{
					yield return DecorationInstances[i9];
				}
			}
			if (DecorationPurchaseRights != null)
			{
				for (int i8 = 0; i8 < DecorationPurchaseRights.Length; i8++)
				{
					yield return DecorationPurchaseRights[i8];
				}
			}
			if (Lots != null)
			{
				for (int i7 = 0; i7 < Lots.Length; i7++)
				{
					yield return Lots[i7];
				}
			}
			if (StructureInstances != null)
			{
				for (int i6 = 0; i6 < StructureInstances.Length; i6++)
				{
					yield return StructureInstances[i6];
				}
			}
			if (StructurePurchaseRights != null)
			{
				for (int i5 = 0; i5 < StructurePurchaseRights.Length; i5++)
				{
					yield return StructurePurchaseRights[i5];
				}
			}
			if (MusicTracks != null)
			{
				for (int i4 = 0; i4 < MusicTracks.Length; i4++)
				{
					yield return MusicTracks[i4];
				}
			}
			if (Lighting != null)
			{
				for (int i3 = 0; i3 < Lighting.Length; i3++)
				{
					yield return Lighting[i3];
				}
			}
			if (Fabrics != null)
			{
				for (int i2 = 0; i2 < Fabrics.Length; i2++)
				{
					yield return Fabrics[i2];
				}
			}
			if (Decals != null)
			{
				for (int n = 0; n < Decals.Length; n++)
				{
					yield return Decals[n];
				}
			}
			if (EmotePacks != null)
			{
				for (int m = 0; m < EmotePacks.Length; m++)
				{
					yield return EmotePacks[m];
				}
			}
			if (SizzleClips != null)
			{
				for (int l = 0; l < SizzleClips.Length; l++)
				{
					yield return SizzleClips[l];
				}
			}
			if (Consumables != null)
			{
				for (int k = 0; k < Consumables.Length; k++)
				{
					yield return Consumables[k];
				}
			}
			if (Durables != null)
			{
				for (int j = 0; j < Durables.Length; j++)
				{
					yield return Durables[j];
				}
			}
			if (Tubes != null)
			{
				for (int i = 0; i < Tubes.Length; i++)
				{
					yield return Tubes[i];
				}
			}
		}
	}
}
