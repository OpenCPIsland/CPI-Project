using MinigameFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_Resources : MinigameResources<mg_ss_EResourceList>
	{
		private mg_ss_EGameMode m_lastLoadedGameMode;

		private bool m_hasLoadedGame = false;

		private bool m_hasLoadedShared = false;

		private List<GameObject> m_loadedGenericSounds = new List<GameObject>();

		private List<GameObject> m_loadedSpecificSounds = new List<GameObject>();

		public mg_ss_ConveyorSpeedData ConveyorSpeedData
		{
			get;
			private set;
		}

		public mg_ss_ItemGeneratorData ItemGeneratorData
		{
			get;
			private set;
		}

		public mg_ss_OrderLengthData OrderLengthData
		{
			get;
			private set;
		}

		public mg_ss_SpecialOrderData SpecialOrderData
		{
			get;
			private set;
		}

		public mg_ss_ChaosModeData ChaosModeData
		{
			get;
			private set;
		}

		public mg_ss_CoinTierData CoinTierData
		{
			get;
			private set;
		}

		public override void LoadResources()
		{
			LoadResource("SmoothieSmash/mg_ss_pf_Title", mg_ss_EResourceList.TITLE_LOGIC);
		}

		public void LoadGameResources(mg_ss_EGameMode p_gameMode)
		{
			if (m_hasLoadedGame && m_lastLoadedGameMode != p_gameMode)
			{
				UnloadGameMode();
			}
			if (!m_hasLoadedGame)
			{
				if (p_gameMode == mg_ss_EGameMode.NORMAL)
				{
					LoadGameResources_Normal();
				}
				else
				{
					LoadGameResources_Survival();
				}
				LoadGameSounds();
				m_lastLoadedGameMode = p_gameMode;
				m_hasLoadedGame = true;
			}
			LoadSharedResources();
		}

		private void LoadGameResources_Normal()
		{
			LoadResource("SmoothieSmash/mg_ss_pf_GameNormal", mg_ss_EResourceList.GAME_SPECIFIC);
			LoadResource("SmoothieSmash/mg_ss_pf_GameSounds_Single_Normal", mg_ss_EResourceList.GAME_SPECIFIC_SOUNDS_SINGLE);
			LoadResource("SmoothieSmash/mg_ss_pf_clock", mg_ss_EResourceList.GAME_ITEM_CLOCK);
			LoadResource("SmoothieSmash/mg_ss_pf_aunt_arctic", mg_ss_EResourceList.GAME_AUNT_ARTIC);
			LoadResource("SmoothieSmash/mg_ss_pf_rock_hopper", mg_ss_EResourceList.GAME_ROCK_HOPPER);
			LoadResource("SmoothieSmash/mg_ss_pf_herbert", mg_ss_EResourceList.GAME_HERBERT);
			ConveyorSpeedData = new mg_ss_ConveyorSpeedData("SmoothieSmash/mg_ss_ConveyorSpeedData_Normal");
			ItemGeneratorData = new mg_ss_ItemGeneratorData();
			ItemGeneratorData.LoadXML("SmoothieSmash/mg_ss_ItemGeneratorData_Normal");
			OrderLengthData = new mg_ss_OrderLengthData();
			OrderLengthData.LoadXML("SmoothieSmash/mg_ss_OrderLengthData");
			SpecialOrderData = new mg_ss_SpecialOrderData();
			SpecialOrderData.LoadXML("SmoothieSmash/mg_ss_SpecialOrderData");
		}

		private void LoadGameResources_Survival()
		{
			LoadResource("SmoothieSmash/mg_ss_pf_GameSurvival", mg_ss_EResourceList.GAME_SPECIFIC);
			LoadResource("SmoothieSmash/mg_ss_pf_GameSounds_Survival", mg_ss_EResourceList.GAME_SPECIFIC_SOUNDS);
			LoadResource("SmoothieSmash/mg_ss_pf_GameSounds_Single_Survival", mg_ss_EResourceList.GAME_SPECIFIC_SOUNDS_SINGLE);
			LoadResource("SmoothieSmash/mg_ss_pf_heart_icon", mg_ss_EResourceList.GAME_HEART_ICON);
			LoadResource("SmoothieSmash/mg_ss_pf_bomb", mg_ss_EResourceList.GAME_ITEM_BOMB);
			LoadResource("SmoothieSmash/mg_ss_pf_anvil", mg_ss_EResourceList.GAME_ITEM_ANVIL);
			ConveyorSpeedData = new mg_ss_ConveyorSpeedData("SmoothieSmash/mg_ss_ConveyorSpeedData_Survival");
			ItemGeneratorData = new mg_ss_ItemGeneratorData();
			ItemGeneratorData.LoadXML("SmoothieSmash/mg_ss_ItemGeneratorData_Survival");
		}

		private void LoadSharedResources()
		{
			if (!m_hasLoadedShared)
			{
				LoadResource("SmoothieSmash/mg_ss_pf_Game", mg_ss_EResourceList.GAME_GENERIC);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_apple", mg_ss_EResourceList.GAME_ITEM_APPLE);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_banana", mg_ss_EResourceList.GAME_ITEM_BANANA);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_blackberry", mg_ss_EResourceList.GAME_ITEM_BLACKBERRY);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_blueberry", mg_ss_EResourceList.GAME_ITEM_BLUEBERRY);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_fig", mg_ss_EResourceList.GAME_ITEM_FIG);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_grapes", mg_ss_EResourceList.GAME_ITEM_GRAPES);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_mango", mg_ss_EResourceList.GAME_ITEM_MANGO);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_orange", mg_ss_EResourceList.GAME_ITEM_ORANGE);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_peach", mg_ss_EResourceList.GAME_ITEM_PEACH);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_pineapple", mg_ss_EResourceList.GAME_ITEM_PINEAPPLE);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_rasberry", mg_ss_EResourceList.GAME_ITEM_RASPBERRY);
				LoadResource("SmoothieSmash/mg_ss_pf_fruit_strawberry", mg_ss_EResourceList.GAME_ITEM_STRAWBERRY);
				LoadResource("SmoothieSmash/mg_ss_pf_golden_apple", mg_ss_EResourceList.GAME_ITEM_GOLDEN_APPLE);
				LoadResource("SmoothieSmash/mg_ss_pf_golden_apple_rotation", mg_ss_EResourceList.GAME_GOLDEN_APPLE_ROTATE);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_01", mg_ss_EResourceList.COMBO_01);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_02", mg_ss_EResourceList.COMBO_02);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_03", mg_ss_EResourceList.COMBO_03);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_04", mg_ss_EResourceList.COMBO_04);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_05", mg_ss_EResourceList.COMBO_05);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_06", mg_ss_EResourceList.COMBO_06);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_07", mg_ss_EResourceList.COMBO_07);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_08", mg_ss_EResourceList.COMBO_08);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_09", mg_ss_EResourceList.COMBO_09);
				LoadResource("SmoothieSmash/mg_ss_pf_combotext_10", mg_ss_EResourceList.COMBO_10);
				LoadResource("SmoothieSmash/mg_ss_pf_splatter_blob", mg_ss_EResourceList.GAME_SPLAT_BLOB);
				LoadResource("SmoothieSmash/mg_ss_pf_splatter_splat", mg_ss_EResourceList.GAME_SPLAT);
				LoadResource("SmoothieSmash/mg_ss_pf_splatter_splat_little", mg_ss_EResourceList.GAME_SPLAT_LITTLE);
				LoadResource("SmoothieSmash/mg_ss_pf_GameSounds", mg_ss_EResourceList.GAME_GENERIC_SOUNDS);
				LoadResource("SmoothieSmash/mg_ss_pf_GameSounds_Single", mg_ss_EResourceList.GAME_GENERIC_SOUNDS_SINGLE);
				LoadSharedSounds();
				ChaosModeData = new mg_ss_ChaosModeData();
				ChaosModeData.LoadXML("SmoothieSmash/mg_ss_ChaosModeData");
				CoinTierData = new mg_ss_CoinTierData();
				CoinTierData.LoadXML("SmoothieSmash/mg_ss_CoinTierData");
				m_hasLoadedShared = true;
			}
		}

		public GameObject GetCombo(int p_combo)
		{
			mg_ss_EResourceList assetTag = (mg_ss_EResourceList)(50 + (Mathf.Min(p_combo, 10) - 1));
			return GetInstancedResource(assetTag);
		}

		public GameObject GetCustomer(string p_tag)
		{
			GameObject result = null;
			switch (p_tag)
			{
			case "auntarctic":
				result = GetInstancedResource(mg_ss_EResourceList.GAME_AUNT_ARTIC);
				break;
			case "rockhopper":
				result = GetInstancedResource(mg_ss_EResourceList.GAME_ROCK_HOPPER);
				break;
			case "herbert":
				result = GetInstancedResource(mg_ss_EResourceList.GAME_HERBERT);
				break;
			}
			return result;
		}

		private void LoadGameSounds()
		{
			GameObject gameObject = null;
			for (int i = 0; i < 3; i++)
			{
				gameObject = GetInstancedResource(mg_ss_EResourceList.GAME_SPECIFIC_SOUNDS);
				if (gameObject != null)
				{
					m_loadedSpecificSounds.Add(gameObject);
				}
			}
			gameObject = GetInstancedResource(mg_ss_EResourceList.GAME_SPECIFIC_SOUNDS_SINGLE);
			m_loadedSpecificSounds.Add(gameObject);
		}

		private void LoadSharedSounds()
		{
			GameObject gameObject = null;
			for (int i = 0; i < 3; i++)
			{
				gameObject = GetInstancedResource(mg_ss_EResourceList.GAME_GENERIC_SOUNDS);
				m_loadedGenericSounds.Add(gameObject);
			}
			gameObject = GetInstancedResource(mg_ss_EResourceList.GAME_GENERIC_SOUNDS_SINGLE);
			m_loadedGenericSounds.Add(gameObject);
		}

		private void UnloadGameGenericSounds()
		{
			foreach (GameObject loadedGenericSound in m_loadedGenericSounds)
			{
				UnityEngine.Object.Destroy(loadedGenericSound);
			}
			m_loadedGenericSounds.Clear();
		}

		private void UnloadGameSpecificSounds()
		{
			foreach (GameObject loadedSpecificSound in m_loadedSpecificSounds)
			{
				UnityEngine.Object.Destroy(loadedSpecificSound);
			}
			m_loadedGenericSounds.Clear();
		}

		private void UnloadGameMode()
		{
			UnloadResource(mg_ss_EResourceList.GAME_SPECIFIC);
			UnloadResource(mg_ss_EResourceList.GAME_SPECIFIC_SOUNDS_SINGLE);
			UnloadGameSpecificSounds();
			ConveyorSpeedData = null;
			m_hasLoadedGame = false;
		}

		public static void LoadXMLClass(object p_class, XmlNode p_xmlNode)
		{
			Type type = p_class.GetType();
			foreach (XmlAttribute attribute in p_xmlNode.Attributes)
			{
				PropertyInfo property = type.GetProperty(attribute.Name);
				if (property != null)
				{
					property.SetValue(p_class, Convert.ChangeType(attribute.InnerText, property.PropertyType, CultureInfo.InvariantCulture), null);
				}
			}
		}
	}
}
