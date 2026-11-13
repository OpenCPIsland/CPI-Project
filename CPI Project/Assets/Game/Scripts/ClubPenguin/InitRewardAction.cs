using ClubPenguin.Chat;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Collectibles;
using ClubPenguin.Consumable;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Depercated;
using ClubPenguin.Durable;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Rewards;
using ClubPenguin.Tubes;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitRewardAction : InitActionComponent
	{
		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			Type[] rewardables = new Type[23]
			{
				typeof(CoinReward),
				typeof(MascotXPReward),
				typeof(CollectibleReward),
				typeof(ConsumableInstanceReward),
				typeof(DecalReward),
				typeof(EquipmentInstanceReward),
				typeof(EquipmentTemplateReward),
				typeof(DecorationInstanceReward),
				typeof(DecorationReward),
				typeof(IglooSlotsReward),
				typeof(LotReward),
				typeof(StructureInstanceReward),
				typeof(StructureReward),
				typeof(MusicTrackReward),
				typeof(LightingReward),
				typeof(FabricReward),
				typeof(ConsumableReward),
				typeof(DurableReward),
				typeof(TubeReward),
				typeof(EmoteReward),
				typeof(SizzleClipReward),
				typeof(ColourPackReward),
				typeof(SavedOutfitSlotReward)
			};
			RewardableLoader.Init(rewardables);
			yield break;
		}
	}
}
