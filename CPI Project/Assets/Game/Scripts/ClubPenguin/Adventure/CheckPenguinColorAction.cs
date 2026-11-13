using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckPenguinColorAction : FsmStateAction
	{
		public AvatarColorDefinition[] ColorsToCheck;

		public FsmEvent MatchEvent;

		public FsmEvent FailEvent;

		public override void OnEnter()
		{
			Color lhs = Color.black;
			bool flag = false;
			bool flag2 = false;
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				AvatarDetailsData component = Service.Get<CPDataEntityCollection>().GetComponent<AvatarDetailsData>(localPlayerHandle);
				if (component != null)
				{
					lhs = component.BodyColor;
					flag = true;
				}
			}
			if (flag && ColorsToCheck != null)
			{
				for (int i = 0; i < ColorsToCheck.Length; i++)
				{
					Color color;
					if (ColorUtility.TryParseHtmlString("#" + ColorsToCheck[i].Color, out color) && lhs == color)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				base.Fsm.Event(MatchEvent);
			}
			else
			{
				base.Fsm.Event(FailEvent);
			}
		}
	}
}
