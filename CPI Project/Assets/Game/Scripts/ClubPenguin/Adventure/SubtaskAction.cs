using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SubtaskAction : FsmStateAction
	{
		[UIHint(UIHint.TextArea)]
		public FsmString SubtaskId;

		public FsmString i18nContents;

		public bool UseDialogList;

		public DialogList dialogList;

		public int dialogListIndex = 0;

		public FsmInt Integer;

		public FsmInt IntegerCount;

		public FsmBool Bool;

		public override void Reset()
		{
			SubtaskId = new FsmString
			{
				UseVariable = true
			};
			i18nContents = new FsmString
			{
				UseVariable = true
			};
			Integer = new FsmInt
			{
				UseVariable = true
			};
			IntegerCount = new FsmInt
			{
				UseVariable = true
			};
			Bool = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			string text = i18nContents.Value;
			if (UseDialogList && dialogList != null)
			{
				text = dialogList.Entries[dialogListIndex].ContentToken;
			}
			if (!string.IsNullOrEmpty(text))
			{
				EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
				HudEvents.SetSubtaskText evt = default(HudEvents.SetSubtaskText);
				evt.ID = ((!SubtaskId.IsNone) ? SubtaskId.Value : text);
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(text);
				if (!IntegerCount.IsNone && !Integer.IsNone)
				{
					int num = Mathf.Clamp(Integer.Value, 0, IntegerCount.Value);
					evt.SubtaskText = string.Format("{0}/{1} {2}", num, IntegerCount.Value, tokenTranslation);
					evt.IsComplete = (num == IntegerCount.Value);
				}
				else if (!Bool.IsNone)
				{
					evt.SubtaskText = tokenTranslation;
					evt.IsComplete = Bool.Value;
				}
				eventDispatcher.DispatchEvent(evt);
			}
			Finish();
		}
	}
}
