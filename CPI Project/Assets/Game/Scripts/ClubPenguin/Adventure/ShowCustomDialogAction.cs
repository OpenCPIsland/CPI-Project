using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Advanced)")]
	public class ShowCustomDialogAction : FsmStateAction
	{
		public string Contents;

		[LocalizationToken]
		public string i18nContents;

		public bool UseDialogList;

		public DialogList DialogList;

		public int DialogListItemIndex;

		public bool RandomDialog;

		public string DialogPrefabKey;

		public bool WaitForFinish = true;

		public int DismissTime;

		public float AutoCloseTime;

		public bool CenterX;

		public bool CenterY;

		public float OffsetY;

		public float OffsetYPercent;

		public bool HideTail;

		public string ContentImageKey;

		public DTextStyle TextStyle;

		public bool RichText;

		public string BackgroundImageKey;

		public string MascotName;

		public string CustomName;

		public bool HideQuestHud = true;

		public bool ClickToClose = true;

		public FsmString AudioEventName;

		public bool AdvanceSequence = true;

		public bool PlayDialogAnimation = true;

		public FsmString DialogAnimationOverride;

		public bool AutoStopDialogAnimation = true;

		public FsmString EndDialogAnimationOverride;

		private EventDispatcher dispatcher;

		public override void Reset()
		{
			TextStyle = new DTextStyle();
		}

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			DCinematicSpeech dCinematicSpeech = new DCinematicSpeech();
			DialogList.Entry entry = default(DialogList.Entry);
			if (!UseDialogList)
			{
				dCinematicSpeech.Text = Service.Get<Localizer>().GetTokenTranslation(i18nContents);
			}
			else if (DialogList != null)
			{
				entry = ((!RandomDialog) ? DialogList.Entries[DialogListItemIndex] : DialogList.Entries[Random.Range(0, DialogList.Entries.Length)]);
				dCinematicSpeech.Text = Service.Get<Localizer>().GetTokenTranslation(entry.ContentToken);
			}
			dCinematicSpeech.BubbleContentKey = DialogPrefabKey;
			dCinematicSpeech.Buttons = null;
			dCinematicSpeech.BackgroundImageKey = BackgroundImageKey;
			dCinematicSpeech.ContentImageKey = ContentImageKey;
			dCinematicSpeech.TextStyle = TextStyle;
			dCinematicSpeech.RichText = RichText;
			dCinematicSpeech.MascotName = (string.IsNullOrEmpty(CustomName) ? MascotName : CustomName);
			dCinematicSpeech.DismissTime = ((AutoCloseTime > 0f) ? AutoCloseTime : ((float)DismissTime));
			dCinematicSpeech.CenterX = CenterX;
			dCinematicSpeech.CenterY = CenterY;
			dCinematicSpeech.OffsetY = OffsetY;
			dCinematicSpeech.OffsetYPercent = OffsetYPercent;
			dCinematicSpeech.HideTail = HideTail;
			dCinematicSpeech.ClickToClose = ClickToClose;
			dispatcher.DispatchEvent(new CinematicSpeechEvents.ShowSpeechEvent(dCinematicSpeech, HideQuestHud));
			dispatcher.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(OnSpeechComplete);
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
			dispatcher.RemoveListener<CinematicSpeechEvents.SpeechCompleteEvent>(OnSpeechComplete);
		}

		private void onMascotAudioComplete(EventNotificationType type, string eventName, object info, GameObject gameObject)
		{
			if (eventName == AudioEventName.Value && type == EventNotificationType.OnFinished)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(QuestEvents.MascotAudioComplete));
			}
		}

		private bool OnSpeechComplete(CinematicSpeechEvents.SpeechCompleteEvent evt)
		{
			if (base.Active)
			{
				Finish();
			}
			return false;
		}
	}
}
