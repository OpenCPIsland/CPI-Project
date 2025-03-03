using ClubPenguin.Analytics;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class AllAccessRewardPopupStateHandler : AbstractAccountStateHandler
	{
		public string RewardPopupCompleteEvent;

		public string TitleToken;

		public PrefabContentKey AllAccessRewardScreenKey;

		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				Service.Get<ICPSwrveService>().Action("free_member_weekend", "start");
				Service.Get<EventDispatcher>().AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
				ShowRewardPopup.Builder builder = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.generic, null);
				builder.setHeaderText(TitleToken);
				builder.setCustomScreenKey(AllAccessRewardScreenKey);
				ShowRewardPopup showRewardPopup = builder.Build();
				showRewardPopup.Execute();
			}
		}

		private bool onRewardPopupComplete(RewardEvents.RewardPopupComplete evt)
		{
			if (rootStateMachine != null)
			{
				rootStateMachine.SendEvent(RewardPopupCompleteEvent);
			}
			else
			{
				Log.LogError(this, "rootStateMachine was null");
			}
			return false;
		}
	}
}
