using ClubPenguin.UI;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class ShowPromptAction : FsmStateAction
	{
		public string i18nTitleText;

		public string i18nBodyText;

		public bool OkButton = true;

		public bool CancelButton;

		public bool YesButton;

		public bool NoButton;

		public bool CloseButton;

		public Sprite Image;

		public bool IsModal = true;

		public PromptController PrefabOverride = null;

		public FsmEvent OkEvent;

		public FsmEvent CancelEvent;

		public FsmEvent YesEvent;

		public FsmEvent NoEvent;

		public FsmEvent CloseEvent;

		public override void OnEnter()
		{
			DPrompt.ButtonFlags buttonFlags = DPrompt.ButtonFlags.None;
			if (OkButton)
			{
				buttonFlags |= DPrompt.ButtonFlags.OK;
			}
			if (CancelButton)
			{
				buttonFlags |= DPrompt.ButtonFlags.CANCEL;
			}
			if (YesButton)
			{
				buttonFlags |= DPrompt.ButtonFlags.YES;
			}
			if (NoButton)
			{
				buttonFlags |= DPrompt.ButtonFlags.NO;
			}
			if (CloseButton)
			{
				buttonFlags |= DPrompt.ButtonFlags.CLOSE;
			}
			DPrompt data = new DPrompt(i18nTitleText, i18nBodyText, buttonFlags, Image, IsModal);
			Service.Get<PromptManager>().ShowPrompt(data, onButtonPressed, PrefabOverride);
		}

		private void onButtonPressed(DPrompt.ButtonFlags pressed)
		{
			switch (pressed)
			{
			case DPrompt.ButtonFlags.OK:
				base.Fsm.Event(OkEvent);
				break;
			case DPrompt.ButtonFlags.CANCEL:
				base.Fsm.Event(CancelEvent);
				break;
			case DPrompt.ButtonFlags.YES:
				base.Fsm.Event(YesEvent);
				break;
			case DPrompt.ButtonFlags.NO:
				base.Fsm.Event(NoEvent);
				break;
			case DPrompt.ButtonFlags.CLOSE:
				base.Fsm.Event(CloseEvent);
				break;
			}
			Finish();
		}
	}
}
