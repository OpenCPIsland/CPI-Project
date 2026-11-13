using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameTopHud : MonoBehaviour
	{
		public enum HudStates
		{
			TextOnly,
			TextLeftOfLoader,
			TextAboveLoader
		}

		public GameObject HeaderPanel;

		public GameObject LoaderPanel;

		public GameObject HeaderSinglePanel;

		public GameObject InstructionPanel;

		public Text HeaderText;

		public Text HeaderSingleText;

		public Text InstructionText;

		public SpriteSelector[] ThemeSpriteSelectors;

		public TintSelector[] ThemeTintSelectors;

		private Localizer localizer;

		public void ShowHudState(HudStates state, string textToken, int themeId)
		{
			changeState(state, textToken);
			setTheme(themeId);
		}

		private void Awake()
		{
			localizer = Service.Get<Localizer>();
		}

		private void setTheme(int themeId)
		{
			for (int i = 0; i < ThemeSpriteSelectors.Length; i++)
			{
				ThemeSpriteSelectors[i].SelectSprite(themeId);
			}
			for (int i = 0; i < ThemeTintSelectors.Length; i++)
			{
				ThemeTintSelectors[i].SelectColor(themeId);
			}
		}

		private void changeState(HudStates newState, string textToken)
		{
			switch (newState)
			{
			case HudStates.TextAboveLoader:
				InstructionPanel.SetActive(false);
				HeaderSinglePanel.SetActive(false);
				HeaderPanel.SetActive(true);
				LoaderPanel.SetActive(true);
				HeaderText.text = localizer.GetTokenTranslation(textToken);
				break;
			case HudStates.TextLeftOfLoader:
				InstructionPanel.SetActive(false);
				HeaderSinglePanel.SetActive(true);
				HeaderPanel.SetActive(false);
				LoaderPanel.SetActive(false);
				HeaderSingleText.text = localizer.GetTokenTranslation(textToken);
				break;
			case HudStates.TextOnly:
				InstructionPanel.SetActive(true);
				HeaderSinglePanel.SetActive(false);
				HeaderPanel.SetActive(false);
				LoaderPanel.SetActive(false);
				InstructionText.text = localizer.GetTokenTranslation(textToken);
				break;
			}
		}
	}
}
