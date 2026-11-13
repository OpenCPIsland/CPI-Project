using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ShowQuestMessageAction : FsmStateAction
	{
		[LocalizationToken]
		[UIHint(UIHint.TextArea)]
		public string i18nContents;

		public bool UseDialogList;

		public DialogList DialogList;

		public int DialogListItemIndex;

		public bool RandomDialog;

		public string MascotName;

		public int DismissTime;

		public float AutoCloseTime;

		public string ImageContentKey;

		public bool RichText;

		public bool ClickToClose = true;

		public bool WaitForFinish = true;

		private EventDispatcher dispatcher;

		public FsmString AudioEventName;

		public bool AdvanceSequence = true;

		public bool PlayDialogAnimation = true;

		public FsmString DialogAnimationOverride;

		public bool AutoStopDialogAnimation = true;

		public FsmString EndDialogAnimationOverride;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			DQuestMessage dQuestMessage = new DQuestMessage();
			DialogList.Entry entry = default(DialogList.Entry);
			if (!UseDialogList)
			{
				dQuestMessage.Text = Service.Get<Localizer>().GetTokenTranslation(i18nContents);
			}
			else if (DialogList != null)
			{
				entry = ((!RandomDialog) ? DialogList.Entries[DialogListItemIndex] : DialogList.Entries[Random.Range(0, DialogList.Entries.Length)]);
				dQuestMessage.Text = Service.Get<Localizer>().GetTokenTranslation(entry.ContentToken);
			}
			dQuestMessage.MascotName = MascotName;
			dQuestMessage.DismissTime = ((AutoCloseTime > 0f) ? AutoCloseTime : ((float)DismissTime));
			dQuestMessage.Buttons = null;
			dQuestMessage.ContentImageKey = ImageContentKey;
			dQuestMessage.RichText = RichText;
			dQuestMessage.ClickToClose = ClickToClose;
			dispatcher.AddListener<HudEvents.QuestMessageComplete>(OnSpeechComplete);
			dispatcher.DispatchEvent(new HudEvents.ShowQuestMessage(dQuestMessage));
			string text = "";
			string overrideAnimationName = "";
			string overrideStopAnimationName = "";
			bool flag = false;
			if (UseDialogList && DialogList != null)
			{
				text = entry.AudioEventName;
				overrideAnimationName = entry.DialogAnimationTrigger;
				overrideStopAnimationName = entry.DialogAnimationEndTrigger;
				flag = entry.AdvanceSequence;
			}
			else if (!AudioEventName.IsNone)
			{
				text = AudioEventName.Value;
				overrideAnimationName = DialogAnimationOverride.Value;
				overrideStopAnimationName = EndDialogAnimationOverride.Value;
				flag = AdvanceSequence;
			}
			if (!string.IsNullOrEmpty(text))
			{
				EventAction eventAction = EventAction.PlaySound;
				if (flag)
				{
					eventAction = EventAction.AdvanceSequence;
				}
				GameObject mascotObject = Service.Get<MascotService>().GetMascotObject(MascotName);
				if (mascotObject != null && PlayDialogAnimation)
				{
					MascotController component = mascotObject.GetComponent<MascotController>();
					EventManager.Instance.PostEventNotify(text, eventAction, base.Owner, component.dialogAudioCallback);
					component.StartDialogAnimation(text, overrideAnimationName, AutoStopDialogAnimation, overrideStopAnimationName);
				}
				else
				{
					EventManager.Instance.PostEventNotify(text, eventAction, base.Owner, onMascotAudioComplete);
				}
			}
			if (!WaitForFinish)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<HudEvents.QuestMessageComplete>(OnSpeechComplete);
		}

		private void onMascotAudioComplete(EventNotificationType type, string eventName, object info, GameObject gameObject)
		{
			if (eventName == AudioEventName.Value && type == EventNotificationType.OnFinished)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(QuestEvents.MascotAudioComplete));
			}
		}

		private bool OnSpeechComplete(HudEvents.QuestMessageComplete evt)
		{
			CoroutineRunner.StopAllForOwner(this);
			Finish();
			return false;
		}
	}
}
