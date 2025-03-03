using MinigameFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_Resources : MinigameResources<mg_if_EResourceList>
	{
		private bool m_hasLoadedGame = false;

		private List<GameObject> m_loadedSounds = new List<GameObject>();

		private mg_if_Variables m_variables;

		public bool HasLoadedGameSounds
		{
			get;
			private set;
		}

		public mg_if_Variables Variables
		{
			get
			{
				return m_variables;
			}
		}

		public override void LoadResources()
		{
			LoadResource("IceFishing/mg_if_pf_Title", mg_if_EResourceList.TITLE_LOGIC);
		}

		public void LoadGameResources()
		{
			if (!m_hasLoadedGame)
			{
				LoadGameVariables();
				LoadResource("IceFishing/mg_if_pf_IceFishing", mg_if_EResourceList.GAME_LOGIC);
				LoadResource("IceFishing/mg_if_pf_Puffle", mg_if_EResourceList.GAME_PUFFLE);
				LoadResource("IceFishing/mg_if_pf_YellowFish", mg_if_EResourceList.GAME_YELLOWFISH);
				LoadResource("IceFishing/mg_if_pf_Barrel", mg_if_EResourceList.GAME_BARREL);
				LoadResource("IceFishing/mg_if_pf_Boot", mg_if_EResourceList.GAME_BOOT);
				LoadResource("IceFishing/mg_if_pf_JellyFish", mg_if_EResourceList.GAME_JELLYFISH);
				LoadResource("IceFishing/mg_if_pf_SharkNear", mg_if_EResourceList.GAME_SHARK_NEAR);
				LoadResource("IceFishing/mg_if_pf_SharkFar", mg_if_EResourceList.GAME_SHARK_FAR);
				LoadResource("IceFishing/mg_if_pf_Crab", mg_if_EResourceList.GAME_CRAB);
				LoadResource("IceFishing/mg_if_pf_FreeLife", mg_if_EResourceList.GAME_FREE_LIFE);
				LoadResource("IceFishing/mg_if_pf_game_sounds", mg_if_EResourceList.GAME_SOUNDS);
				LoadResource("IceFishing/mg_if_pf_game_sounds_single", mg_if_EResourceList.GAME_SOUNDS_SINGLE);
				LoadGameSounds();
				m_hasLoadedGame = true;
			}
		}

		public void UnloadGameResources()
		{
			m_variables = null;
			UnloadResource(mg_if_EResourceList.GAME_LOGIC);
			UnloadResource(mg_if_EResourceList.GAME_PUFFLE);
			UnloadResource(mg_if_EResourceList.GAME_YELLOWFISH);
			UnloadResource(mg_if_EResourceList.GAME_BARREL);
			UnloadResource(mg_if_EResourceList.GAME_BOOT);
			UnloadResource(mg_if_EResourceList.GAME_JELLYFISH);
			UnloadResource(mg_if_EResourceList.GAME_SHARK_NEAR);
			UnloadResource(mg_if_EResourceList.GAME_SHARK_FAR);
			UnloadResource(mg_if_EResourceList.GAME_CRAB);
			UnloadResource(mg_if_EResourceList.GAME_FREE_LIFE);
			UnloadResource(mg_if_EResourceList.GAME_SOUNDS);
			UnloadResource(mg_if_EResourceList.GAME_SOUNDS_SINGLE);
			UnloadGameSounds();
			m_hasLoadedGame = false;
		}

		private void LoadGameSounds()
		{
			GameObject gameObject = null;
			for (int i = 0; i < 3; i++)
			{
				gameObject = GetInstancedResource(mg_if_EResourceList.GAME_SOUNDS);
				m_loadedSounds.Add(gameObject);
			}
			gameObject = GetInstancedResource(mg_if_EResourceList.GAME_SOUNDS_SINGLE);
			m_loadedSounds.Add(gameObject);
			HasLoadedGameSounds = true;
		}

		private void UnloadGameSounds()
		{
			foreach (GameObject loadedSound in m_loadedSounds)
			{
				UnityEngine.Object.Destroy(loadedSound);
			}
			m_loadedSounds.Clear();
			HasLoadedGameSounds = false;
		}

		private void LoadGameVariables()
		{
			TextAsset textAsset = Resources.Load("IceFishing/mg_if_Variables") as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			m_variables = new mg_if_Variables();
			Type type = m_variables.GetType();
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/variables");
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				PropertyInfo property = type.GetProperty(item.Name);
				if (property != null)
				{
					property.SetValue(m_variables, Convert.ChangeType(item.InnerText, property.PropertyType, CultureInfo.InvariantCulture), null);
				}
			}
		}
	}
}
