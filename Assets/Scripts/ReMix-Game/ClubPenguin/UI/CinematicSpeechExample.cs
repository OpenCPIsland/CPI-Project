using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CinematicSpeechExample : MonoBehaviour
	{
		public InputField SpeechInput;

		public void OnShowSpeechClick()
		{
			DCinematicSpeech dCinematicSpeech = new DCinematicSpeech();
			dCinematicSpeech.Text = Service.Get<Localizer>().GetTokenTranslation(SpeechInput.text);
			dCinematicSpeech.BubbleContentKey = "Prefabs/Quest/CinematicSpeedBubbles/CinematicSpeechBubbleDynamic";
			DButton dButton = new DButton();
			dButton.Text = "Yes";
			dButton.ButtonPrefabKey = "Prefabs/Buttons/Button_Test";
			DButton dButton2 = new DButton();
			dButton2.Text = "No";
			dButton2.ButtonPrefabKey = "Prefabs/Buttons/Button_Test";
			dCinematicSpeech.Buttons = new DButton[2]
			{
				dButton,
				dButton2
			};
			DTextStyle dTextStyle = new DTextStyle();
			dTextStyle.ColorHex = "08FF08";
			dTextStyle.FontContentKey = "Fonts/Draculon-Regular";
			dCinematicSpeech.TextStyle = dTextStyle;
			dCinematicSpeech.BackgroundImageKey = "Sprites/CinematicSpeechBubbles/CinematicSpeachBubble_RockHopper";
			Service.Get<EventDispatcher>().DispatchEvent(new CinematicSpeechEvents.ShowSpeechEvent(dCinematicSpeech));
		}
	}
}
