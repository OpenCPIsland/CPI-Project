using ClubPenguin.Analytics;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class SettingsPanelServerListButton : MonoBehaviour
	{
		private const int TINT_WORLD_NOT_FULL_INDEX = 0;

		private const int TINT_WORLD_FULL_INDEX = 1;

		public Text NameText;

		public Text CurrentServerNameText;

		public Transform Arrow;

		public RoomPopulationDisplay PopulationDisplay;

		public GameObject FriendIndicator;

		private bool isCurrentServer;

		private TintSelector nameTint;

		private RoomPopulationScale populationScale;

		public WorldDefinition World
		{
			get;
			private set;
		}

		private bool isWorldFull
		{
			get
			{
				return populationScale == RoomPopulationScale.FIVE;
			}
		}

		public void Awake()
		{
			if (NameText != null)
			{
				nameTint = NameText.GetComponent<TintSelector>();
			}
		}

		public void LoadWorld(WorldDefinition definition, bool currentServer = false, RoomPopulationScale populationScale = RoomPopulationScale.ZERO)
		{
			World = definition;
			NameText.text = definition.WorldName;
			isCurrentServer = currentServer;
			if (currentServer && CurrentServerNameText != null)
			{
				CurrentServerNameText.text = definition.WorldName;
				CurrentServerNameText.gameObject.SetActive(true);
				if (NameText != null)
				{
					NameText.gameObject.SetActive(false);
				}
				if (Arrow != null)
				{
					Arrow.gameObject.SetActive(false);
				}
			}
			ShowFriendIndicator(false);
			UpdatePopulationScale(populationScale);
		}

		public void OnClick()
		{
			if (!isCurrentServer)
			{
				if (isWorldFull)
				{
					PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("WorldFullPrompt");
					PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, showWorldFullPrompt);
					promptLoaderCMD.Execute();
				}
				else if (World.Language != Service.Get<Localizer>().Language)
				{
					PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("ChangeServerPrompt");
					PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, showChangeServerPrompt);
					promptLoaderCMD.Execute();
				}
				else
				{
					changeServer();
				}
			}
		}

		private void showChangeServerPrompt(PromptLoaderCMD promptLoader)
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(LocalizationLanguage.GetLanguageToken(World.Language));
			promptLoader.PromptData.SetText("Prompt.Text.World", World.WorldName, true);
			promptLoader.PromptData.SetText("Prompt.Text.Language", tokenTranslation, true);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onChangeServerPromptButtonClicked, promptLoader.Prefab);
		}

		private void onChangeServerPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES || pressed == DPrompt.ButtonFlags.OK)
			{
				changeServer();
			}
		}

		private void changeServer()
		{
			GameStateController gameStateController = Service.Get<GameStateController>();
			Service.Get<ICPSwrveService>().Action("view.server_change", World.WorldName);
			gameStateController.ChangeServer(World.WorldName);
		}

		private void showWorldFullPrompt(PromptLoaderCMD promptLoader)
		{
			string i18nText = string.Format(Service.Get<Localizer>().GetTokenTranslation(promptLoader.PromptData.TextFields[DPrompt.PROMPT_TEXT_BODY].I18nText), World.WorldName);
			promptLoader.PromptData.SetText(DPrompt.PROMPT_TEXT_BODY, i18nText, true);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, null, promptLoader.Prefab);
		}

		public void ShowFriendIndicator(bool setActive = true)
		{
			if (FriendIndicator != null)
			{
				FriendIndicator.SetActive(setActive);
			}
		}

		public void UpdatePopulationScale(RoomPopulationScale populationScale)
		{
			this.populationScale = populationScale;
			if (PopulationDisplay != null)
			{
				PopulationDisplay.UpdatePopulationDisplay(populationScale);
			}
			if (nameTint != null)
			{
				if (isWorldFull)
				{
					nameTint.SelectColor(1);
				}
				else
				{
					nameTint.SelectColor(0);
				}
			}
		}
	}
}
