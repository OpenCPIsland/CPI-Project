using ClubPenguin.MiniGames;
using ClubPenguin.MiniGames.Fishing;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Fishing")]
	public class GetFishingGamePrizeDefinitionsAction : FsmStateAction
	{
		[RequiredField]
		public FsmString PrizeName;

		public FsmString PrizeAssetPath;

		public FsmString ShadowAssetPath;

		public FsmFloat TimingWindow;

		public override void OnEnter()
		{
			MinigameService minigameService = Service.Get<MinigameService>();
			LootTableRewardDefinition lootRewardDefinition = minigameService.GetLootRewardDefinition(PrizeName.Value);
			if (lootRewardDefinition != null)
			{
				PrizeAssetPath.Value = lootRewardDefinition.ModelAsset.Key;
				ShadowAssetPath.Value = lootRewardDefinition.ShadowAsset.Key;
				TimingWindow.Value = lootRewardDefinition.TimingWindow;
			}
			Finish();
		}
	}
}
