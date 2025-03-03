using ClubPenguin;
using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace Assets.Game.MiniGames.Scripts.Fishing.FsmActions
{
	[ActionCategory("MiniGames")]
	public class CheckDailyLimitAction : FsmStateAction
	{
		public FsmString GameId = "fishing";

		[Tooltip("Event to raise when the Daily limit is reached (or exceeded).")]
		public FsmEvent OutOfBaitEvent;

		[Tooltip("Event to raise when the Daily limit has not yet been reached.")]
		public FsmEvent OkToPlayEvent;

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(Service.Get<QuestService>().CurrentFishingPrize))
			{
				base.Fsm.Event(OkToPlayEvent);
			}
			else
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				MiniGamePlayCountData component;
				if (!cPDataEntityCollection.LocalPlayerHandle.IsNull && cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					if (component.MinigamePlayCounts.ContainsKey(GameId.Value))
					{
						int num = 10 - component.MinigamePlayCounts[GameId.Value];
						if (num <= 0)
						{
							base.Fsm.Event(OutOfBaitEvent);
						}
						else
						{
							base.Fsm.Event(OkToPlayEvent);
						}
					}
					else
					{
						component.SetMinigamePlayCount(GameId.Value, 0);
						base.Fsm.Event(OkToPlayEvent);
					}
				}
			}
			Finish();
		}
	}
}
