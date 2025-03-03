using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class GiveRewardAction : FsmStateAction
	{
		public int Xp;

		public string MascotName = "AuntArctic";

		public int Coins;

		public override void OnEnter()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			if (Xp > 0)
			{
				Reward reward = new Reward();
				reward.Add(new MascotXPReward(MascotName, Xp));
				eventDispatcher.DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.QUEST_OBJECTIVE, base.Name, reward));
			}
			if (Coins > 0)
			{
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(Coins);
			}
			Finish();
		}
	}
}
