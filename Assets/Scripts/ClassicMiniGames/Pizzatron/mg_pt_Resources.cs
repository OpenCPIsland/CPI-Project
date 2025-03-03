using MinigameFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_Resources : MinigameResources<mg_pt_EResourceList>
	{
		private int m_topping01Count;

		private int m_topping02Count;

		private int m_topping03Count;

		private int m_topping04Count;

		private Color m_sauceColor01;

		private Color m_sauceColor02;

		private bool m_areSharedAssetsLoaded;

		private bool m_isGameLoaded;

		private bool m_wasCandyLoaded;

		private List<GameObject> m_loadedGenericSounds = new List<GameObject>();

		private List<GameObject> m_loadedSpecificSounds = new List<GameObject>();

		public int PizzaBaseCount
		{
			get;
			private set;
		}

		public mg_pt_ComplexityData ComplexityDataHead
		{
			get;
			private set;
		}

		public mg_pt_ConveyorSpeedData ConveyorSpeedData
		{
			get;
			private set;
		}

		public override void LoadResources()
		{
		}

		public void LoadGameResources(bool _isCandy)
		{
			if (m_isGameLoaded)
			{
				if (m_wasCandyLoaded && !_isCandy)
				{
					UnloadGameMode();
				}
				else if (!m_wasCandyLoaded && _isCandy)
				{
					UnloadGameMode();
				}
			}
			if (!m_isGameLoaded)
			{
				if (!m_areSharedAssetsLoaded)
				{
					LoadSharedAssets();
				}
				LoadGameAssets(_isCandy);
			}
		}

		private void LoadGameAssets(bool _isCandy)
		{
			if (_isCandy)
			{
				LoadResource("Pizzatron/Cookie/mg_pt_pf_chef_cookie", mg_pt_EResourceList.GAME_SPECIFIC_MIN);
				m_sauceColor01 = new Color(0.24f, 0.11f, 0.05f, 1f);
				m_sauceColor02 = new Color(0.91f, 0.35f, 0.62f, 1f);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_board", mg_pt_EResourceList.GAME_PIZZA_BOARD);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_sauce_01", mg_pt_EResourceList.GAME_ORDER_SAUCE_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_sauce_02", mg_pt_EResourceList.GAME_ORDER_SAUCE_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_sprinkles", mg_pt_EResourceList.GAME_ORDER_CHEESE);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_group_01", mg_pt_EResourceList.GAME_ORDER_TOPPING_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_group_02", mg_pt_EResourceList.GAME_ORDER_TOPPING_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_group_03", mg_pt_EResourceList.GAME_ORDER_TOPPING_03);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_group_04", mg_pt_EResourceList.GAME_ORDER_TOPPING_04);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_order_mystery", mg_pt_EResourceList.GAME_ORDER_MYSTERY);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_sauce_choc_holder", mg_pt_EResourceList.GAME_SAUCE_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_sauce_pink_holder", mg_pt_EResourceList.GAME_SAUCE_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_holder_topping_sprinkles", mg_pt_EResourceList.GAME_HOLDER_CHEESE);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_holder_topping_01", mg_pt_EResourceList.GAME_HOLDER_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_holder_topping_02", mg_pt_EResourceList.GAME_HOLDER_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_holder_topping_03", mg_pt_EResourceList.GAME_HOLDER_03);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_holder_topping_04", mg_pt_EResourceList.GAME_HOLDER_04);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_sauce_choc_bottle", mg_pt_EResourceList.GAME_HOLDER_SAUCE_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_sauce_pink_bottle", mg_pt_EResourceList.GAME_HOLDER_SAUCE_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_sprinkles", mg_pt_EResourceList.GAME_TOPPING_CHEESE);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_liquorice_01", mg_pt_EResourceList.GAME_TOPPING_01_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_liquorice_02", mg_pt_EResourceList.GAME_TOPPING_01_02);
				m_topping01Count = 2;
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_chocchip_01", mg_pt_EResourceList.GAME_TOPPING_02_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_chocchip_02", mg_pt_EResourceList.GAME_TOPPING_02_02);
				m_topping02Count = 2;
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_marshmellow_01", mg_pt_EResourceList.GAME_TOPPING_03_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_marshmellow_02", mg_pt_EResourceList.GAME_TOPPING_03_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_marshmellow_03", mg_pt_EResourceList.GAME_TOPPING_03_03);
				m_topping03Count = 3;
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_jellybean_01", mg_pt_EResourceList.GAME_TOPPING_04_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_jellybean_02", mg_pt_EResourceList.GAME_TOPPING_04_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_jellybean_03", mg_pt_EResourceList.GAME_TOPPING_04_03);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_jellybean_04", mg_pt_EResourceList.GAME_TOPPING_04_04);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_jellybean_05", mg_pt_EResourceList.GAME_TOPPING_04_05);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_topping_jellybean_06", mg_pt_EResourceList.GAME_TOPPING_04_06);
				m_topping04Count = 6;
				LoadResource("Pizzatron/Cookie/mg_pt_pf_pizza", mg_pt_EResourceList.GAME_PIZZA_BASE_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_pizza_star", mg_pt_EResourceList.GAME_PIZZA_BASE_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_pizza_penguin", mg_pt_EResourceList.GAME_PIZZA_BASE_03);
				PizzaBaseCount = 3;
				LoadResource("Pizzatron/Cookie/mg_pt_pf_result_bad", mg_pt_EResourceList.RESULT_BG_01);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_result_good", mg_pt_EResourceList.RESULT_BG_02);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_result_awesome", mg_pt_EResourceList.RESULT_BG_03);
				LoadResource("Pizzatron/Cookie/mg_pt_pf_GameSounds_Single", mg_pt_EResourceList.GAME_GENERIC_SOUNDS_SINGLE_SPECIFIC);
				LoadSounds();
			}
			else
			{
				LoadResource("Pizzatron/Normal/mg_pt_pf_chef_pizza", mg_pt_EResourceList.GAME_SPECIFIC_MIN);
				m_sauceColor01 = new Color(0.98f, 0.29f, 0f, 1f);
				m_sauceColor02 = new Color(0.98f, 0.08f, 0f, 1f);
				LoadResource("Pizzatron/Normal/mg_pt_pf_board", mg_pt_EResourceList.GAME_PIZZA_BOARD);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_sauce_01", mg_pt_EResourceList.GAME_ORDER_SAUCE_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_sauce_02", mg_pt_EResourceList.GAME_ORDER_SAUCE_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_cheese", mg_pt_EResourceList.GAME_ORDER_CHEESE);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_group_01", mg_pt_EResourceList.GAME_ORDER_TOPPING_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_group_02", mg_pt_EResourceList.GAME_ORDER_TOPPING_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_group_03", mg_pt_EResourceList.GAME_ORDER_TOPPING_03);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_group_04", mg_pt_EResourceList.GAME_ORDER_TOPPING_04);
				LoadResource("Pizzatron/Normal/mg_pt_pf_order_mystery", mg_pt_EResourceList.GAME_ORDER_MYSTERY);
				LoadResource("Pizzatron/Normal/mg_pt_pf_sauce_pizza_holder", mg_pt_EResourceList.GAME_SAUCE_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_sauce_hot_holder", mg_pt_EResourceList.GAME_SAUCE_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_holder_topping_cheese", mg_pt_EResourceList.GAME_HOLDER_CHEESE);
				LoadResource("Pizzatron/Normal/mg_pt_pf_holder_topping_01", mg_pt_EResourceList.GAME_HOLDER_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_holder_topping_02", mg_pt_EResourceList.GAME_HOLDER_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_holder_topping_03", mg_pt_EResourceList.GAME_HOLDER_03);
				LoadResource("Pizzatron/Normal/mg_pt_pf_holder_topping_04", mg_pt_EResourceList.GAME_HOLDER_04);
				LoadResource("Pizzatron/Normal/mg_pt_pf_sauce_pizza_bottle", mg_pt_EResourceList.GAME_HOLDER_SAUCE_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_sauce_hot_bottle", mg_pt_EResourceList.GAME_HOLDER_SAUCE_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_cheese", mg_pt_EResourceList.GAME_TOPPING_CHEESE);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_seaweed_01", mg_pt_EResourceList.GAME_TOPPING_01_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_seaweed_02", mg_pt_EResourceList.GAME_TOPPING_01_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_seaweed_03", mg_pt_EResourceList.GAME_TOPPING_01_03);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_seaweed_04", mg_pt_EResourceList.GAME_TOPPING_01_04);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_seaweed_05", mg_pt_EResourceList.GAME_TOPPING_01_05);
				m_topping01Count = 5;
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_shrimp_01", mg_pt_EResourceList.GAME_TOPPING_02_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_shrimp_02", mg_pt_EResourceList.GAME_TOPPING_02_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_shrimp_03", mg_pt_EResourceList.GAME_TOPPING_02_03);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_shrimp_04", mg_pt_EResourceList.GAME_TOPPING_02_04);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_shrimp_05", mg_pt_EResourceList.GAME_TOPPING_02_05);
				m_topping02Count = 5;
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_squid_01", mg_pt_EResourceList.GAME_TOPPING_03_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_squid_02", mg_pt_EResourceList.GAME_TOPPING_03_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_squid_03", mg_pt_EResourceList.GAME_TOPPING_03_03);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_squid_04", mg_pt_EResourceList.GAME_TOPPING_03_04);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_squid_05", mg_pt_EResourceList.GAME_TOPPING_03_05);
				m_topping03Count = 5;
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_fish_01", mg_pt_EResourceList.GAME_TOPPING_04_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_topping_fish_02", mg_pt_EResourceList.GAME_TOPPING_04_02);
				m_topping04Count = 2;
				LoadResource("Pizzatron/Normal/mg_pt_pf_pizza", mg_pt_EResourceList.GAME_PIZZA_BASE_01);
				PizzaBaseCount = 1;
				LoadResource("Pizzatron/Normal/mg_pt_pf_result_bad", mg_pt_EResourceList.RESULT_BG_01);
				LoadResource("Pizzatron/Normal/mg_pt_pf_result_good", mg_pt_EResourceList.RESULT_BG_02);
				LoadResource("Pizzatron/Normal/mg_pt_pf_result_awesome", mg_pt_EResourceList.RESULT_BG_03);
				LoadResource("Pizzatron/Normal/mg_pt_pf_GameSounds_Single", mg_pt_EResourceList.GAME_GENERIC_SOUNDS_SINGLE_SPECIFIC);
				LoadSounds();
			}
			m_isGameLoaded = true;
			m_wasCandyLoaded = _isCandy;
		}

		private void UnloadGameMode()
		{
			for (int i = 10; i < 57; i++)
			{
				UnloadResource((mg_pt_EResourceList)i);
			}
			UnloadGameSpecificSounds();
			m_isGameLoaded = false;
		}

		private void LoadSharedAssets()
		{
			LoadComplexityData();
			ConveyorSpeedData = new mg_pt_ConveyorSpeedData("Pizzatron/mg_pt_conveyor_speed_data");
			LoadResource("Pizzatron/mg_pt_pf_game", mg_pt_EResourceList.GAME_GENERIC);
			LoadResource("Pizzatron/mg_pt_pf_customer_default", mg_pt_EResourceList.GAME_CUSTOMER_DEFAULT);
			LoadResource("Pizzatron/mg_pt_pf_customer_gary", mg_pt_EResourceList.GAME_CUSTOMER_GARY);
			LoadResource("Pizzatron/mg_pt_pf_customer_rockhopper", mg_pt_EResourceList.GAME_CUSTOMER_ROCKHOPPER);
			LoadResource("Pizzatron/mg_pt_pf_customer_sensei", mg_pt_EResourceList.GAME_CUSTOMER_SENSEI);
			LoadResource("Pizzatron/mg_pt_Coin", mg_pt_EResourceList.GAME_COIN);
			LoadResource("Pizzatron/mg_pt_pf_GameSounds", mg_pt_EResourceList.GAME_GENERIC_SOUNDS);
			LoadResource("Pizzatron/mg_pt_pf_GameSounds_Single", mg_pt_EResourceList.GAME_GENERIC_SOUNDS_SINGLE);
			LoadSharedSounds();
			m_areSharedAssetsLoaded = true;
		}

		private void LoadSharedSounds()
		{
			GameObject gameObject = null;
			for (int i = 0; i < 3; i++)
			{
				gameObject = GetInstancedResource(mg_pt_EResourceList.GAME_GENERIC_SOUNDS);
				m_loadedGenericSounds.Add(gameObject);
			}
			gameObject = GetInstancedResource(mg_pt_EResourceList.GAME_GENERIC_SOUNDS_SINGLE);
			m_loadedGenericSounds.Add(gameObject);
		}

		private void LoadSounds()
		{
			GameObject gameObject = null;
			gameObject = GetInstancedResource(mg_pt_EResourceList.GAME_GENERIC_SOUNDS_SINGLE_SPECIFIC);
			m_loadedSpecificSounds.Add(gameObject);
		}

		private void LoadComplexityData()
		{
			ComplexityDataHead = new mg_pt_ComplexityData();
			TextAsset textAsset = Resources.Load("Pizzatron/mg_pt_complexity_data") as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/data");
			mg_pt_ComplexityData mg_pt_ComplexityData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (mg_pt_ComplexityData == null)
				{
					mg_pt_ComplexityData = ComplexityDataHead;
				}
				else
				{
					mg_pt_ComplexityData.NextData = new mg_pt_ComplexityData();
					mg_pt_ComplexityData = mg_pt_ComplexityData.NextData;
				}
				LoadXMLClass(mg_pt_ComplexityData, item);
			}
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

		public GameObject GetTopping(mg_pt_EToppingType p_toppingType)
		{
			GameObject result = null;
			switch (p_toppingType)
			{
			case mg_pt_EToppingType.SAUCE_01:
				result = GetInstancedResource(mg_pt_EResourceList.GAME_HOLDER_SAUCE_01);
				break;
			case mg_pt_EToppingType.SAUCE_02:
				result = GetInstancedResource(mg_pt_EResourceList.GAME_HOLDER_SAUCE_02);
				break;
			case mg_pt_EToppingType.CHEESE:
				result = GetInstancedResource(mg_pt_EResourceList.GAME_TOPPING_CHEESE);
				break;
			case mg_pt_EToppingType.MIN_TOPPINGS:
				result = GetInstancedResource((mg_pt_EResourceList)(30 + UnityEngine.Random.Range(0, m_topping01Count)));
				break;
			case mg_pt_EToppingType.TOPPING_02:
				result = GetInstancedResource((mg_pt_EResourceList)(35 + UnityEngine.Random.Range(0, m_topping02Count)));
				break;
			case mg_pt_EToppingType.TOPPING_03:
				result = GetInstancedResource((mg_pt_EResourceList)(40 + UnityEngine.Random.Range(0, m_topping03Count)));
				break;
			case mg_pt_EToppingType.TOPPING_04:
				result = GetInstancedResource((mg_pt_EResourceList)(45 + UnityEngine.Random.Range(0, m_topping04Count)));
				break;
			}
			return result;
		}

		public Color GetSauceColor(mg_pt_EToppingType p_sauceType)
		{
			return (p_sauceType == mg_pt_EToppingType.SAUCE_01) ? m_sauceColor01 : m_sauceColor02;
		}

		private void UnloadGameSpecificSounds()
		{
			UnloadResource(mg_pt_EResourceList.GAME_GENERIC_SOUNDS_SINGLE_SPECIFIC);
			foreach (GameObject loadedSpecificSound in m_loadedSpecificSounds)
			{
				UnityEngine.Object.Destroy(loadedSpecificSound);
			}
			m_loadedGenericSounds.Clear();
		}
	}
}
