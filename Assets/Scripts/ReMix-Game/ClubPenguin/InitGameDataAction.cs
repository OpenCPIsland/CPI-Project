using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.CellPhone;
using ClubPenguin.Chat;
using ClubPenguin.Collectibles;
using ClubPenguin.Commerce;
using ClubPenguin.Configuration;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.DisneyStore;
using ClubPenguin.FeatureToggle;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.PartyGames;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using ClubPenguin.Rewards;
using ClubPenguin.Tags;
using ClubPenguin.Tubes;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitGameDataAction : InitActionComponent
	{
		public static Type[] DefinitionTypesToLoadAfterBoot = new Type[29]
		{
			typeof(EmoteDefinition),
			typeof(SizzleClipDefinition),
			typeof(EquipmentCategoryDefinition),
			typeof(FabricDefinition),
			typeof(DecalDefinition),
			typeof(PropDefinition),
			typeof(TubeDefinition),
			typeof(PromptDefinition),
			typeof(DisneyStoreFranchiseDefinition),
			typeof(CatalogThemeDefinition),
			typeof(CatalogThemeScheduleDefinition),
			typeof(MascotDefinition),
			typeof(CollectibleDefinition),
			typeof(TemporaryHeadStatusDefinition),
			typeof(PartyGameDefinition),
			typeof(PartyGameLauncherDefinition),
			typeof(MarketingLoadingScreenDefinition),
			typeof(LightingDefinition),
			typeof(MusicTrackDefinition),
			typeof(CollisionRuleSetDefinition),
			typeof(CollisionRuleDefinition),
			typeof(ZoneDefinition),
			typeof(LotDefinition),
			typeof(LoginZoneDefinition),
			typeof(DecorationCategoryDefinition),
			typeof(MusicGenreDefinition),
			typeof(GroupDefinition),
			typeof(CommerceResourceURLsDefinition),
			typeof(CellPhoneSaleActivityDefinition)
		};

		private static Type[] definitionTypesToLoadDuringBoot = new Type[16]
		{
			typeof(MembershipPlansDefinition),
			typeof(AvatarColorDefinition),
			typeof(ProgressionUnlockDefinition),
			typeof(AllAccessEventDefinition),
			typeof(FeatureDefinition),
			typeof(FeatureLabelBreadcrumbDefinition),
			typeof(ConditionalDefinition),
			typeof(DecorationDefinition),
			typeof(StructureDefinition),
			typeof(ScheduledEventDateDefinition),
			typeof(HomeScreenTakeoverDefinition),
			typeof(TutorialDefinition),
			typeof(ClaimableRewardDefinition),
			typeof(TemplateDefinition),
			typeof(ProductDefinition),
			typeof(CommerceResourceURLsDefinition)
		};

		private bool isInitComplete = false;

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
			GameData data = new GameData();
			data.EInitialized += OnGameDataInitialized;
			data.Init(definitionTypesToLoadDuringBoot);
			while (!isInitComplete)
			{
				yield return null;
			}
		}

		private void OnGameDataInitialized(GameData gameData)
		{
			Service.Set(gameData);
			Service.Set((IGameData)gameData);
			Service.Set(new TagsManager());
			isInitComplete = true;
			gameData.EInitialized -= OnGameDataInitialized;
		}
	}
}
