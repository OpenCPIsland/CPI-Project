using ClubPenguin.Core;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin
{
	[ActionCategory("PlayerPrefs")]
	public class LocalAccountPlayerPrefsHasKey : FsmStateAction
	{
		[RequiredField]
		public FsmString key;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		public override void Reset()
		{
			key = "";
		}

		public override void OnEnter()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			string displayName = cPDataEntityCollection.GetComponent<DisplayNameData>(cPDataEntityCollection.LocalPlayerHandle).DisplayName;
			string text = displayName + key.Value;
			base.Fsm.Event(PlayerPrefs.HasKey(text) ? trueEvent : falseEvent);
			Finish();
		}
	}
}
