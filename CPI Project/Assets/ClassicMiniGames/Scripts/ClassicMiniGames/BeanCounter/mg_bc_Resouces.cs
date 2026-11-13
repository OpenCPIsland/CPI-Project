using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_Resouces : MinigameResources<mg_bc_EResourceList>
	{
		private mg_bc_EGameMode m_lastLoadedGameMode;

		private bool m_hasLoadedGame = false;

		private bool m_hasLoadedShared = false;

		private List<GameObject> m_loadedSounds = new List<GameObject>();

		public bool HasLoadedGameSounds
		{
			get;
			private set;
		}

		public override void LoadResources()
		{
			LoadResource("BeanCounter/mg_bc_pf_title_sounds", mg_bc_EResourceList.TITLE_ASSET_SOUNDS);
		}

		public void LoadGameResources(mg_bc_EGameMode _mode)
		{
			if (m_hasLoadedGame && m_lastLoadedGameMode != _mode)
			{
				UnloadGameMode();
			}
			if (!m_hasLoadedGame)
			{
				if (_mode == mg_bc_EGameMode.COFFEE_NORMAL)
				{
					LoadResource("BeanCounter/mg_bc_pf_coffee_hazard", mg_bc_EResourceList.GAME_ASSET_HAZARD);
					LoadResource("BeanCounter/mg_bc_pf_coffee_bag", mg_bc_EResourceList.GAME_ASSET_BAG);
					LoadResource("BeanCounter/mg_bc_pf_CoffeeGameRoot", mg_bc_EResourceList.GAME_ASSET_GAME_LOGIC);
				}
				else
				{
					LoadResource("BeanCounter/mg_bc_pf_jelly_hazard", mg_bc_EResourceList.GAME_ASSET_HAZARD);
					LoadResource("BeanCounter/mg_bc_pf_jelly_bag", mg_bc_EResourceList.GAME_ASSET_BAG);
					LoadResource("BeanCounter/mg_bc_pf_JellyGameRoot", mg_bc_EResourceList.GAME_ASSET_GAME_LOGIC);
				}
				m_lastLoadedGameMode = _mode;
				m_hasLoadedGame = true;
			}
			if (!m_hasLoadedShared)
			{
				LoadResource("BeanCounter/mg_bc_pf_life_powerup", mg_bc_EResourceList.GAME_ASSET_ONE_UP);
				LoadResource("BeanCounter/mg_bc_pf_shield_powerup", mg_bc_EResourceList.GAME_ASSET_SHIELD);
				LoadResource("BeanCounter/mg_bc_pf_game_background", mg_bc_EResourceList.GAME_ASSET_BACKGROUND);
				LoadResource("BeanCounter/mg_bc_pf_object_sounds", mg_bc_EResourceList.GAME_ASSET_OBJECT_SOUNDS);
				LoadResource("BeanCounter/mg_bc_pf_single_sounds", mg_bc_EResourceList.GAME_ASSET_SINGLE_SOUNDS);
				LoadResource("BeanCounter/mg_bc_pf_game_sounds", mg_bc_EResourceList.GAME_ASSET_GAME_SOUNDS);
				m_hasLoadedShared = true;
			}
		}

		private void UnloadGameMode()
		{
			UnloadResource(mg_bc_EResourceList.GAME_ASSET_HAZARD);
			UnloadResource(mg_bc_EResourceList.GAME_ASSET_BAG);
			UnloadResource(mg_bc_EResourceList.GAME_ASSET_GAME_LOGIC);
			m_hasLoadedGame = false;
		}

		public void LoadGameSounds()
		{
			GameObject gameObject = null;
			for (int i = 0; i < 3; i++)
			{
				gameObject = GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_OBJECT_SOUNDS);
				m_loadedSounds.Add(gameObject);
				gameObject = GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_GAME_SOUNDS);
				m_loadedSounds.Add(gameObject);
			}
			gameObject = GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_SINGLE_SOUNDS);
			m_loadedSounds.Add(gameObject);
			HasLoadedGameSounds = true;
		}

		public void UnloadGameSounds()
		{
			foreach (GameObject loadedSound in m_loadedSounds)
			{
				Object.Destroy(loadedSound);
			}
			m_loadedSounds.Clear();
			HasLoadedGameSounds = false;
		}
	}
}
