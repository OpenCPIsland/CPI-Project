using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Rewards
{
	[ActionCategory("GUI")]
	public class ShowRewardPopupAction : FsmStateAction
	{
		public string PopupSplashText;

		public DRewardPopup.RewardPopupType PopupType;

		public RewardDefinition Reward;

		public bool ShowXpAndCoinsUI = true;

		public override void OnEnter()
		{
			// The FINISHED state of DJC001Q005Concert has no reward assigned, and a
			// reference that has gone missing deserializes to null as well. The NRE
			// takes Finish() with it, so the quest FSM never moves on
			if (Reward == null)
			{
				Disney.LaunchPadFramework.Log.LogError(this, "Cannot show the reward popup, no Reward is set on the action");
				Finish();
				return;
			}
			ShowRewardPopup showRewardPopup = new ShowRewardPopup.Builder(PopupType, Reward.ToReward()).setHeaderText(PopupSplashText).setShowXpAndCoinsUI(ShowXpAndCoinsUI).Build();
			showRewardPopup.Execute();
			Service.Get<EventDispatcher>().AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
		}

		private bool onRewardPopupComplete(RewardEvents.RewardPopupComplete evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			Finish();
			return false;
		}
	}
}
