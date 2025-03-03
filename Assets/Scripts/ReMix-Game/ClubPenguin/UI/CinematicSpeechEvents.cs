using ClubPenguin.Adventure;
using System.Runtime.InteropServices;

namespace ClubPenguin.UI
{
	public static class CinematicSpeechEvents
	{
		public struct ShowSpeechEvent
		{
			public readonly DCinematicSpeech SpeechData;

			public readonly bool HideQuestHud;

			public ShowSpeechEvent(DCinematicSpeech speechData, bool hideQuestHud = true)
			{
				SpeechData = speechData;
				HideQuestHud = hideQuestHud;
			}
		}

		public struct HideSpeechEvent
		{
			public readonly DCinematicSpeech SpeechData;

			public HideSpeechEvent(DCinematicSpeech speechData)
			{
				SpeechData = speechData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideAllSpeechEvent
		{
		}

		public struct SpeechCompleteEvent
		{
			public readonly DCinematicSpeech SpeechData;

			public SpeechCompleteEvent(DCinematicSpeech speechData)
			{
				SpeechData = speechData;
			}
		}

		public struct PlayMascotAnimation
		{
			public readonly MascotController Mascot;

			public readonly string AnimationTrigger;

			public PlayMascotAnimation(MascotController mascot, string animationTrigger)
			{
				Mascot = mascot;
				AnimationTrigger = animationTrigger;
			}
		}

		public struct SetMascotIdleDialogOverride
		{
			public readonly string MascotName;

			public readonly string AnimationTrigger;

			public SetMascotIdleDialogOverride(string mascotName, string animationTrigger)
			{
				MascotName = mascotName;
				AnimationTrigger = animationTrigger;
			}
		}
	}
}
