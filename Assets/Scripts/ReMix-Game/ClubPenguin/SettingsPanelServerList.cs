using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class SettingsPanelServerList : MonoBehaviour
	{
		private class LanguageWorldData
		{
			public readonly int DropdownIndex;

			public readonly Language Language;

			public bool WorldsQueried;

			public bool ButtonsLoaded;

			public LanguageWorldData(int index, Language language)
			{
				DropdownIndex = index;
				Language = language;
			}
		}

		public Transform ButtonParent;

		public PrefabContentKey ButtonContentKey;

		public SettingsPanelServerListButton CurrentServer;

		public Dropdown LanguageDropdown;

		private GameObject buttonPrefab;

		private CPDataEntityCollection dataEntityCollection;

		private HashSet<string> worldsWithFriends;

		private Dictionary<Language, LanguageWorldData> languageWorldData;

		private WorldDefinition currentWorldDefinition;

		private Dictionary<string, SettingsPanelServerListButton> worldButtons;

		public void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			worldsWithFriends = new HashSet<string>();
			languageWorldData = new Dictionary<Language, LanguageWorldData>();
			worldButtons = new Dictionary<string, SettingsPanelServerListButton>();
			initializeLanguageDropdown();
			showCurrentServerInfo();
			loadButtonPrefab();
			getFriendsPresenceData();
		}

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<WorldServiceEvents.WorldsWithRoomPopulationReceivedEvent>(onWorldsFound);
			LanguageDropdown.onValueChanged.AddListener(onLanguageFilterChanged);
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<WorldServiceEvents.WorldsWithRoomPopulationReceivedEvent>(onWorldsFound);
			LanguageDropdown.onValueChanged.RemoveListener(onLanguageFilterChanged);
		}

		private void OnDestroy()
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<PresenceData>>(OnPresenceDataAdded);
		}

		private void initializeLanguageDropdown()
		{
			this.languageWorldData.Clear();
			Localizer localizer = Service.Get<Localizer>();
			List<string> list = new List<string>();
			HashSet<WorldDefinition> worlds = Service.Get<ZoneTransitionService>().Worlds;
			foreach (WorldDefinition item in worlds)
			{
				if (!this.languageWorldData.ContainsKey(item.Language) && !item.Igloo)
				{
					this.languageWorldData.Add(item.Language, new LanguageWorldData(list.Count, item.Language));
					list.Add(localizer.GetTokenTranslation(LocalizationLanguage.GetLanguageToken(item.Language)));
				}
			}
			LanguageDropdown.ClearOptions();
			LanguageDropdown.AddOptions(list);
			Language language = LocalizationLanguage.GetLanguage();
			LanguageWorldData languageWorldData = this.languageWorldData[language];
			LanguageDropdown.value = languageWorldData.DropdownIndex;
			onLanguageFilterChanged(languageWorldData.DropdownIndex);
		}

		private void onLanguageFilterChanged(int index)
		{
			foreach (KeyValuePair<Language, LanguageWorldData> languageWorldDatum in languageWorldData)
			{
				if (languageWorldDatum.Value.DropdownIndex == index)
				{
					displayForLanguage(languageWorldDatum.Key);
					break;
				}
			}
		}

		private void displayForLanguage(Language language)
		{
			LanguageWorldData languageWorldData = this.languageWorldData[language];
			if (languageWorldData != null)
			{
				queryForWorlds(languageWorldData);
				displayButtonsForLanguage(languageWorldData.Language);
				loadButtonsForLanguage(languageWorldData.Language);
			}
		}

		private void loadButtonsForLanguage(Language language)
		{
			if (buttonPrefab != null && this.languageWorldData.ContainsKey(language) && !this.languageWorldData[language].ButtonsLoaded)
			{
				HashSet<WorldDefinition> worlds = Service.Get<ZoneTransitionService>().Worlds;
				foreach (WorldDefinition item in worlds)
				{
					if (item.Language == language && !item.Igloo)
					{
						loadWorldButton(item, language);
					}
				}
				LanguageWorldData languageWorldData = this.languageWorldData[language];
				languageWorldData.ButtonsLoaded = true;
			}
		}

		private void displayButtonsForLanguage(Language language)
		{
			foreach (SettingsPanelServerListButton value in worldButtons.Values)
			{
				value.gameObject.SetActive(value.World.Language == language);
			}
		}

		private bool isLanguageSelected(Language language)
		{
			return LanguageDropdown.value == languageWorldData[language].DropdownIndex;
		}

		private Language getSelectedLanguage()
		{
			Language result = Language.en_US;
			foreach (KeyValuePair<Language, LanguageWorldData> languageWorldDatum in languageWorldData)
			{
				if (LanguageDropdown.value == languageWorldDatum.Value.DropdownIndex)
				{
					result = languageWorldDatum.Value.Language;
					break;
				}
			}
			return result;
		}

		private void queryForWorlds(LanguageWorldData languageWorldData)
		{
			if (!languageWorldData.WorldsQueried)
			{
				string room = Service.Get<GameStateController>().SceneConfig.DefaultZoneName;
				PresenceData component;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component) && !string.IsNullOrEmpty(component.Room))
				{
					room = component.Room;
				}
				Service.Get<INetworkServicesManager>().WorldService.GetWorldsWithRoomPopulation(room, languageWorldData.Language.ToString());
				languageWorldData.WorldsQueried = true;
			}
		}

		private bool onWorldsFound(WorldServiceEvents.WorldsWithRoomPopulationReceivedEvent evt)
		{
			if (evt.WorldRoomPopulations.Count > 0)
			{
				foreach (WorldRoomPopulation worldRoomPopulation in evt.WorldRoomPopulations)
				{
					RoomPopulationScale populationScaled = worldRoomPopulation.populationScaled;
					SettingsPanelServerListButton buttonForWorld = getButtonForWorld(worldRoomPopulation.worldName);
					if (buttonForWorld != null)
					{
						buttonForWorld.UpdatePopulationScale(populationScaled);
					}
					if (currentWorldDefinition.WorldName == worldRoomPopulation.worldName)
					{
						CurrentServer.UpdatePopulationScale(populationScaled);
					}
				}
			}
			return false;
		}

		private void showCurrentServerInfo()
		{
			PresenceData component = Service.Get<CPDataEntityCollection>().GetComponent<PresenceData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			currentWorldDefinition = Service.Get<ZoneTransitionService>().GetWorld(component.World);
			CurrentServer.LoadWorld(currentWorldDefinition, true);
		}

		private void loadButtonPrefab()
		{
			Content.LoadAsync(onLoadButtonComplete, ButtonContentKey);
		}

		private void onLoadButtonComplete(string key, GameObject prefab)
		{
			buttonPrefab = prefab;
			Language selectedLanguage = getSelectedLanguage();
			displayForLanguage(selectedLanguage);
		}

		private void loadWorldButton(WorldDefinition definition, Language selectedLanguage)
		{
			if (!worldButtons.ContainsKey(definition.WorldName))
			{
				GameObject gameObject = Object.Instantiate(buttonPrefab, ButtonParent, false);
				gameObject.SetActive(definition.Language == selectedLanguage);
				SettingsPanelServerListButton component = gameObject.GetComponent<SettingsPanelServerListButton>();
				bool currentServer = currentWorldDefinition.Equals(definition);
				component.LoadWorld(definition, currentServer);
				if (worldsWithFriends.Contains(definition.WorldName))
				{
					component.ShowFriendIndicator();
				}
				worldButtons.Add(definition.WorldName, component);
			}
		}

		private void getFriendsPresenceData()
		{
			worldsWithFriends = new HashSet<string>();
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<FriendData>();
			List<string> list = new List<string>();
			for (int i = 0; i < entitiesByType.Length; i++)
			{
				PresenceData component;
				if (dataEntityCollection.TryGetComponent(entitiesByType[i], out component))
				{
					component.PresenceDataUpdated += OnPresenceDataUpdated;
				}
				SwidData component2;
				if (dataEntityCollection.TryGetComponent(entitiesByType[i], out component2))
				{
					list.Add(component2.Swid);
				}
				else
				{
					Log.LogError(this, "Friend data handle did not have a SwidData component");
				}
			}
			if (list.Count > 0)
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<PresenceData>>(OnPresenceDataAdded);
				Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataBySwids(list);
			}
		}

		private void OnPresenceDataUpdated(PresenceData presenceData)
		{
			handlePresenceData(presenceData);
			presenceData.PresenceDataUpdated -= OnPresenceDataUpdated;
		}

		private bool OnPresenceDataAdded(DataEntityEvents.ComponentAddedEvent<PresenceData> evt)
		{
			evt.Component.PresenceDataUpdated += OnPresenceDataUpdated;
			return false;
		}

		private void handlePresenceData(PresenceData presenceData)
		{
			if (!string.IsNullOrEmpty(presenceData.World))
			{
				SettingsPanelServerListButton buttonForWorld = getButtonForWorld(presenceData.World);
				if (buttonForWorld != null)
				{
					buttonForWorld.ShowFriendIndicator();
				}
				else
				{
					worldsWithFriends.Add(presenceData.World);
				}
			}
		}

		private SettingsPanelServerListButton getButtonForWorld(string worldName)
		{
			SettingsPanelServerListButton value;
			if (worldButtons.TryGetValue(worldName, out value))
			{
				return value;
			}
			return null;
		}
	}
}
