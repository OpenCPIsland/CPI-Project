using ClubPenguin.Chat;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Advanced)")]
	public class ShowMascotEmoteAction : FsmStateAction
	{
		public string[] Emotes;

		public string MascotName;

		public bool WaitForFinish = true;

		public bool ClickToClose = true;

		public int DismissTime;

		public float AutoCloseTime;

		public bool CenterX;

		public bool CenterY;

		public float OffsetY;

		public bool HideTail;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			Dictionary<string, EmoteDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>();
			EmoteDefinition value = null;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Emotes.Length; i++)
			{
				dictionary.TryGetValue(Emotes[i], out value);
				if (value != null)
				{
					stringBuilder.Append(EmoteManager.GetEmoteString(value));
				}
			}
			DCinematicSpeech dCinematicSpeech = new DCinematicSpeech();
			dCinematicSpeech.Text = stringBuilder.ToString();
			dCinematicSpeech.Buttons = null;
			dCinematicSpeech.MascotName = MascotName;
			dCinematicSpeech.DismissTime = ((AutoCloseTime > 0f) ? AutoCloseTime : ((float)DismissTime));
			dCinematicSpeech.CenterX = CenterX;
			dCinematicSpeech.CenterY = CenterY;
			dCinematicSpeech.OffsetY = OffsetY;
			dCinematicSpeech.HideTail = HideTail;
			dCinematicSpeech.ClickToClose = ClickToClose;
			dCinematicSpeech.ShowContinueImageImmediately = ClickToClose;
			dCinematicSpeech.KeepTextStyle = true;
			dCinematicSpeech.BubbleContentKey = "Prefabs/Quest/CinematicSpeechBubbles/CinematicSpeechBubbleEmote";
			dispatcher.DispatchEvent(new CinematicSpeechEvents.ShowSpeechEvent(dCinematicSpeech));
			dispatcher.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(OnSpeechComplete);
			if (!WaitForFinish)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<CinematicSpeechEvents.SpeechCompleteEvent>(OnSpeechComplete);
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
