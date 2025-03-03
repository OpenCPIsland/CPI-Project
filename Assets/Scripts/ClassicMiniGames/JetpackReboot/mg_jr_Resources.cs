using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Resources : MinigameResources<mg_jr_ResourceList>
	{
		public const string PREFAB_FILE_PREFIX = "mg_jr_pf";

		public const string RESOURCE_FOLDER = "JetpackReboot/";

		protected const string OBSTACLES_FOLDER = "JetpackReboot/Obstacle/";

		protected const string BOSSES_FOLDER = "JetpackReboot/Boss/";

		public const string PREFAB_ENVIRONMENT_FOLDER = "JetpackReboot/Environment/";

		protected const string SFX_ROOT_FOLDER = "JetpackReboot/Sound/";

		protected const string SINGLE_SFX_FOLDER = "JetpackReboot/Sound/SinglePlay/";

		protected const string MULTIPLAY_SFX_FOLDER = "JetpackReboot/Sound/MultiPlay/";

		protected const string SFX_LOOPS_FOLDER = "JetpackReboot/Sound/Loops/";

		protected const string MUSIC_FOLDER = "JetpackReboot/Sound/Music/";

		protected Dictionary<mg_jr_EnvironmentLayerID, GameObject> m_layerResources = new Dictionary<mg_jr_EnvironmentLayerID, GameObject>();

		protected Dictionary<mg_jr_ResourceList, mg_GameObjectPool> m_pooledResources = new Dictionary<mg_jr_ResourceList, mg_GameObjectPool>();

		protected List<AudioSource> m_musicResources = new List<AudioSource>();

		private GameObject m_loadedSoundsContainer = null;

		private GameObject m_loadedSingleSounds = null;

		private GameObject m_loadedMultiSounds = null;

		private GameObject m_loadedSoundLoops = null;

		private GameObject m_loadedMusic = null;

		private mg_jr_StringResources m_textResources = new mg_jr_StringResources();

		private Dictionary<string, mg_jr_ResourceList> m_xmlResourceNameToPrefabMapping = new Dictionary<string, mg_jr_ResourceList>();

		private Dictionary<string, mg_jr_ResourceList> m_xmlResourceNameToPrefabMappingNight = new Dictionary<string, mg_jr_ResourceList>();

		private float m_loadProgress = 0f;

		private float m_loadStepSize = 0.091f;

		public ReadOnlyCollection<AudioSource> MusicClips
		{
			get
			{
				return m_musicResources.AsReadOnly();
			}
		}

		public bool GameSoundsAreInstantiated
		{
			get;
			private set;
		}

		public GameObject ResourceContainer
		{
			get;
			set;
		}

		public bool AreFullyLoaded
		{
			get;
			private set;
		}

		public float LoadProgress
		{
			get
			{
				return Mathf.Clamp01(m_loadProgress);
			}
		}

		public mg_jr_ResourceList JrResourceForXmlResource(string _xmlResourceName, EnvironmentVariant _variation)
		{
			mg_jr_ResourceList result = mg_jr_ResourceList.GAME_PREFAB_CAVE_LANTERN;
			bool flag = false;
			if (_variation == EnvironmentVariant.NIGHT)
			{
				flag = m_xmlResourceNameToPrefabMappingNight.ContainsKey(_xmlResourceName);
				if (flag)
				{
					result = m_xmlResourceNameToPrefabMappingNight[_xmlResourceName];
				}
			}
			int num;
			switch (_variation)
			{
			case EnvironmentVariant.NIGHT:
				num = (flag ? 1 : 0);
				break;
			default:
				num = 1;
				break;
			case EnvironmentVariant.DEFAULT:
				num = 0;
				break;
			}
			if (num == 0)
			{
				if (m_xmlResourceNameToPrefabMapping.ContainsKey(_xmlResourceName))
				{
					result = m_xmlResourceNameToPrefabMapping[_xmlResourceName];
				}
				else
				{
					DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "No mapping for '" + _xmlResourceName + "' using placeholder prefab");
				}
			}
			return result;
		}

		private void CreateXxmlToPrefabLookups()
		{
			m_xmlResourceNameToPrefabMapping.Add("jpr_collectable_coin_small_i.png", mg_jr_ResourceList.GAME_PREFAB_SMALL_COIN);
			m_xmlResourceNameToPrefabMapping.Add("jpr_collectable_coin_big_i.png", mg_jr_ResourceList.GAME_PREFAB_BIG_COIN);
			m_xmlResourceNameToPrefabMapping.Add("jpr_collectable_robotpenguin_idle_i.png", mg_jr_ResourceList.GAME_PREFAB_ROBOT_PENGUIN);
			m_xmlResourceNameToPrefabMapping.Add("jpr_collectable_turbofuelcell.png", mg_jr_ResourceList.GAME_PREFAB_TURBO_PICKUP);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_anvil.png", mg_jr_ResourceList.GAME_PREFAB_BIG_ANVIL);
			m_xmlResourceNameToPrefabMapping.Add("jpr_all_anvil_moving_i.png", mg_jr_ResourceList.GAME_PREFAB_MOVING_ANVIL_TOP);
			m_xmlResourceNameToPrefabMapping.Add("jpr_all_anvil_moving_bottom_i.png", mg_jr_ResourceList.GAME_PREFAB_MOVING_ANVIL_BOTTOM);
			m_xmlResourceNameToPrefabMapping.Add("jpr_all_cannongreen_i.png", mg_jr_ResourceList.GAME_PREFAB_CANNON_GREEN);
			m_xmlResourceNameToPrefabMapping.Add("jpr_all_cannonred_top_i.png", mg_jr_ResourceList.GAME_PREFAB_CANNON_RED_TOP);
			m_xmlResourceNameToPrefabMapping.Add("jpr_all_cannonred_i.png", mg_jr_ResourceList.GAME_PREFAB_CANNON_RED_BOTTOM);
			m_xmlResourceNameToPrefabMapping.Add("jpr_forest_anvil.png", mg_jr_ResourceList.GAME_PREFAB_FOREST_ANVIL);
			m_xmlResourceNameToPrefabMapping.Add("jpr_forest_stormycloud_i.png", mg_jr_ResourceList.GAME_PREFAB_STORMY_CLOUD);
			m_xmlResourceNameToPrefabMapping.Add("jpr_forest_tree01.png", mg_jr_ResourceList.GAME_PREFAB_TREE_01);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_forest_tree02.png", mg_jr_ResourceList.GAME_PREFAB_TREE_02_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_forest_tree02.png", mg_jr_ResourceList.GAME_PREFAB_TREE_02);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_forest_tree01.png", mg_jr_ResourceList.GAME_PREFAB_TREE_01_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_forest_hill.png", mg_jr_ResourceList.GAME_PREFAB_FOREST_HILL);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_forest_hill.png", mg_jr_ResourceList.GAME_PREFAB_FOREST_HILL_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_lantern_i.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_LANTERN);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_wave_bottom01.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_BOTTOM_01);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_wave_bottom02.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_BOTTOM_02);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_splitlevel.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_SPLIT_LEVEL);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_wave_top01.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_TOP_01);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_wave_top02.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_TOP_02);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_bigrocks_top02.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_ROCKS_TOP);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_bigrocks_bottom01.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_ROCKS_BOTTOM);
			m_xmlResourceNameToPrefabMapping.Add("jpr_cave_stalagmite01.png", mg_jr_ResourceList.GAME_PREFAB_CAVE_STALAGMITE);
			m_xmlResourceNameToPrefabMapping.Add("jpr_town_cactus.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_CACTUS);
			m_xmlResourceNameToPrefabMapping.Add("jpr_town_obstacle_building01.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_01);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_town_obstacle_building01.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_01_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_town_obstacle_building02.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_02);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_town_obstacle_building02.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_02_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_town_obstacle_lightpole.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_LAMP);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_town_obstacle_lightpole.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_LAMP_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_town_piano.png", mg_jr_ResourceList.GAME_PREFAB_TOWN_PIANO);
			m_xmlResourceNameToPrefabMapping.Add("jpr_water_whale_i.png", mg_jr_ResourceList.GAME_PREFAB_WATER_WHALE);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_water_whale_i.png", mg_jr_ResourceList.GAME_PREFAB_WATER_WHALE_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_water_iceberg.png", mg_jr_ResourceList.GAME_PREFAB_WATER_ICEBERG);
			m_xmlResourceNameToPrefabMappingNight.Add("jpr_water_iceberg.png", mg_jr_ResourceList.GAME_PREFAB_WATER_ICEBERG_NIGHT);
			m_xmlResourceNameToPrefabMapping.Add("jpr_water_coffeebag.png", mg_jr_ResourceList.GAME_PREFAB_WATER_COFFEBAG);
		}

		public mg_jr_Resources()
		{
			CreateXxmlToPrefabLookups();
		}

		public string FindXmlNameOfPrefab(mg_jr_ResourceList _resourceId)
		{
			return m_xmlResourceNameToPrefabMapping.FirstOrDefault((KeyValuePair<string, mg_jr_ResourceList> x) => x.Value == _resourceId).Key;
		}

		public void LoadTextResources()
		{
			m_textResources.LoadResources();
		}

		public IEnumerator<int> LoadResourcesCoRoutine()
		{
			LoadResource("JetpackReboot/mg_jr_pf_world", mg_jr_ResourceList.GAME_WORLD_LOGIC);
			m_loadProgress += m_loadStepSize;
			yield return 0;
			LoadResource("JetpackReboot/mg_jr_pf_penguin", mg_jr_ResourceList.GAME_PREFAB_PENGUIN);
			LoadResource("JetpackReboot/mg_jr_pf_start_platform", mg_jr_ResourceList.PREFAB_START_PLATFORM);
			LoadResource("JetpackReboot/mg_jr_pf_intro_gary", mg_jr_ResourceList.GARY_INTRO);
			m_loadProgress += m_loadStepSize;
			yield return 0;
			LoadPooledResource("JetpackReboot/mg_jr_pf_small_coin", mg_jr_ResourceList.GAME_PREFAB_SMALL_COIN, new mg_jr_CoinFactory());
			LoadPooledResource("JetpackReboot/mg_jr_pf_big_coin", mg_jr_ResourceList.GAME_PREFAB_BIG_COIN, new mg_jr_BigCoinFactory());
			LoadPooledResource("JetpackReboot/mg_jr_pf_robot_penguin", mg_jr_ResourceList.GAME_PREFAB_ROBOT_PENGUIN, new mg_jr_RobotPenguinFactory());
			LoadPooledResource("JetpackReboot/mg_jr_pf_turbo_pickup", mg_jr_ResourceList.GAME_PREFAB_TURBO_PICKUP, new mg_jr_TurboPickupFactory());
			m_loadProgress += m_loadStepSize;
			yield return 1;
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cave_anvil", mg_jr_ResourceList.GAME_PREFAB_BIG_ANVIL);
			LoadPooledResource("JetpackReboot/Obstacle/Common/mg_jr_pf_moving_anvil", mg_jr_ResourceList.GAME_PREFAB_MOVING_ANVIL_TOP);
			LoadPooledResource("JetpackReboot/Obstacle/Common/mg_jr_pf_moving_anvil", mg_jr_ResourceList.GAME_PREFAB_MOVING_ANVIL_BOTTOM);
			LoadPooledResource("JetpackReboot/Obstacle/Common/mg_jr_pf_snowball", mg_jr_ResourceList.GAME_PREFAB_SNOWBALL);
			mg_jr_ObstacleFactory redTopCannonFactory = new mg_jr_CannonFactory(mg_jr_ResourceList.GAME_PREFAB_CANNON_RED_TOP, mg_jr_Sound.FIRE_RED_CANNON, 0.25f);
			mg_jr_ObstacleFactory redBottomCannonFactory = new mg_jr_CannonFactory(mg_jr_ResourceList.GAME_PREFAB_CANNON_RED_BOTTOM, mg_jr_Sound.FIRE_RED_CANNON, 0.25f);
			mg_jr_ObstacleFactory greenBottomCannonFactory = new mg_jr_CannonFactory(mg_jr_ResourceList.GAME_PREFAB_CANNON_GREEN, mg_jr_Sound.FIRE_GREEN_CANNON, 0f);
			LoadPooledResource("JetpackReboot/Obstacle/Common/mg_jr_pf_cannon_green", mg_jr_ResourceList.GAME_PREFAB_CANNON_GREEN, greenBottomCannonFactory);
			LoadPooledResource("JetpackReboot/Obstacle/Common/mg_jr_pf_cannon_red_top", mg_jr_ResourceList.GAME_PREFAB_CANNON_RED_TOP, redTopCannonFactory);
			LoadPooledResource("JetpackReboot/Obstacle/Common/mg_jr_pf_cannon_red_bottom", mg_jr_ResourceList.GAME_PREFAB_CANNON_RED_BOTTOM, redBottomCannonFactory);
			m_loadProgress += m_loadStepSize;
			yield return 2;
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_forest_anvil", mg_jr_ResourceList.GAME_PREFAB_FOREST_ANVIL);
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_stormcloud", mg_jr_ResourceList.GAME_PREFAB_STORMY_CLOUD, new mg_jr_StormFactory());
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_tree01", mg_jr_ResourceList.GAME_PREFAB_TREE_01);
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_tree01_night", mg_jr_ResourceList.GAME_PREFAB_TREE_01_NIGHT);
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_tree02", mg_jr_ResourceList.GAME_PREFAB_TREE_02);
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_tree02_night", mg_jr_ResourceList.GAME_PREFAB_TREE_02_NIGHT);
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_hill", mg_jr_ResourceList.GAME_PREFAB_FOREST_HILL);
			LoadPooledResource("JetpackReboot/Obstacle/Forest/mg_jr_pf_hill_night", mg_jr_ResourceList.GAME_PREFAB_FOREST_HILL_NIGHT);
			m_loadProgress += m_loadStepSize;
			yield return 3;
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cave_lantern", mg_jr_ResourceList.GAME_PREFAB_CAVE_LANTERN, new mg_jr_LanternFactory());
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cavewave_bottom01", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_BOTTOM_01);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cavewave_bottom02", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_BOTTOM_02);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cave_splitlevel", mg_jr_ResourceList.GAME_PREFAB_CAVE_SPLIT_LEVEL);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cavewave_top01", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_TOP_01);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cavewave_top02", mg_jr_ResourceList.GAME_PREFAB_CAVE_WAVE_TOP_02);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cave_rock_top", mg_jr_ResourceList.GAME_PREFAB_CAVE_ROCKS_TOP);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cave_rock_bottom", mg_jr_ResourceList.GAME_PREFAB_CAVE_ROCKS_BOTTOM);
			LoadPooledResource("JetpackReboot/Obstacle/Cave/mg_jr_pf_cave_stalagmite", mg_jr_ResourceList.GAME_PREFAB_CAVE_STALAGMITE);
			m_loadProgress += m_loadStepSize;
			yield return 4;
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_town_cactus", mg_jr_ResourceList.GAME_PREFAB_TOWN_CACTUS);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_town_house1", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_01);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_town_house1_night", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_01_NIGHT);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_town_house2", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_02);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_town_house2_night", mg_jr_ResourceList.GAME_PREFAB_TOWN_HOUSE_02_NIGHT);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_lamp_post", mg_jr_ResourceList.GAME_PREFAB_TOWN_LAMP);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_lamp_post_night", mg_jr_ResourceList.GAME_PREFAB_TOWN_LAMP_NIGHT);
			LoadPooledResource("JetpackReboot/Obstacle/Town/mg_jr_pf_town_piano", mg_jr_ResourceList.GAME_PREFAB_TOWN_PIANO);
			m_loadProgress += m_loadStepSize;
			yield return 5;
			LoadPooledResource("JetpackReboot/Obstacle/Water/mg_jr_pf_water_whale", mg_jr_ResourceList.GAME_PREFAB_WATER_WHALE, new mg_jr_WhaleFactory(EnvironmentVariant.DEFAULT));
			LoadPooledResource("JetpackReboot/Obstacle/Water/mg_jr_pf_water_whale_night", mg_jr_ResourceList.GAME_PREFAB_WATER_WHALE_NIGHT, new mg_jr_WhaleFactory(EnvironmentVariant.NIGHT));
			LoadPooledResource("JetpackReboot/Obstacle/Water/mg_jr_pf_water_iceberg", mg_jr_ResourceList.GAME_PREFAB_WATER_ICEBERG);
			LoadPooledResource("JetpackReboot/Obstacle/Water/mg_jr_pf_water_iceberg_night", mg_jr_ResourceList.GAME_PREFAB_WATER_ICEBERG_NIGHT);
			LoadPooledResource("JetpackReboot/Obstacle/Water/mg_jr_pf_water_coffeebag", mg_jr_ResourceList.GAME_PREFAB_WATER_COFFEBAG);
			m_loadProgress += m_loadStepSize;
			yield return 6;
			LoadResource("JetpackReboot/Boss/mg_jr_pf_boss_protobot", mg_jr_ResourceList.BOSS_PROTOBOT);
			LoadResource("JetpackReboot/Boss/mg_jr_pf_boss_klutzy", mg_jr_ResourceList.BOSS_KLUTZY);
			LoadResource("JetpackReboot/Boss/mg_jr_pf_boss_herbert", mg_jr_ResourceList.BOSS_HERBERT);
			LoadResource("JetpackReboot/Boss/mg_jr_pf_herbert_snowball", mg_jr_ResourceList.BOSS_HERBERT_SNOWBALL);
			m_loadProgress += m_loadStepSize;
			yield return 7;
			LoadPooledResource("JetpackReboot/mg_jr_pf_effect_coin_pickup", mg_jr_ResourceList.GAME_PREFAB_EFFECT_COIN_PICKUP, new mg_jr_FxFactory(mg_jr_ResourceList.GAME_PREFAB_EFFECT_COIN_PICKUP));
			LoadPooledResource("JetpackReboot/mg_jr_pf_effect_penguin_explode", mg_jr_ResourceList.GAME_PREFAB_EFFECT_PENGUIN_EXPLOSION, new mg_jr_FxFactory(mg_jr_ResourceList.GAME_PREFAB_EFFECT_PENGUIN_EXPLOSION));
			LoadPooledResource("JetpackReboot/mg_jr_pf_obstacle_explosion", mg_jr_ResourceList.GAME_PREFAB_EFFECT_OBSTACLE_EXPLOSION, new mg_jr_FxFactory(mg_jr_ResourceList.GAME_PREFAB_EFFECT_OBSTACLE_EXPLOSION));
			mg_jr_GameObjectFactory speedLineFactory = new mg_jr_FxFactory(mg_jr_ResourceList.GAME_PREFAB_EFFECT_SPEEDLINE, mg_jr_SpriteDrawingLayers.DrawingLayers.FX_OVERLAY_0);
			LoadPooledResource("JetpackReboot/mg_jr_pf_speed_line", mg_jr_ResourceList.GAME_PREFAB_EFFECT_SPEEDLINE, speedLineFactory);
			LoadResource("JetpackReboot/mg_jr_pf_transition", mg_jr_ResourceList.GAME_PREFAB_EFFECT_TRANSITION);
			LoadResource("JetpackReboot/mg_jr_pf_turbo_speedlines", mg_jr_ResourceList.PREFAB_TURBO_SPEEDLINES);
			LoadResource("JetpackReboot/mg_jr_pf_turbo_announce", mg_jr_ResourceList.PREFAB_TURBO_ANNOUNCE);
			m_loadProgress += m_loadStepSize;
			yield return 8;
			LoadPooledResource("JetpackReboot/mg_jr_pf_UICautionSign", mg_jr_ResourceList.WARNING_SIGN, new mg_jr_UIWarningSignFactory());
			m_loadProgress += m_loadStepSize;
			yield return 9;
			LoadSfxResources();
			m_loadProgress += m_loadStepSize;
			yield return 10;
			for (int i = 0; i < 5; i++)
			{
				if (i == 4)
				{
					continue;
				}
				for (int j = 0; j < 2; j++)
				{
					if (i != 3 || j == 0)
					{
						for (int k = 0; k < 5; k++)
						{
							LoadResource(new mg_jr_EnvironmentLayerID((EnvironmentType)i, (EnvironmentVariant)j, (EnvironmentLayer)k));
						}
					}
				}
			}
			m_loadProgress += m_loadStepSize;
			AreFullyLoaded = true;
		}

		public override void LoadResources()
		{
			IEnumerator<int> enumerator = LoadResourcesCoRoutine();
			while (enumerator.MoveNext())
			{
			}
		}

		private void LoadSfxResources()
		{
			if (GameSoundsAreInstantiated)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Sounds have already been loaded");
				return;
			}
			if (m_loadedSoundsContainer == null)
			{
				m_loadedSoundsContainer = new GameObject("mg_jr_sound_resources");
				m_loadedSoundsContainer.transform.parent = ResourceContainer.transform;
				m_loadedMultiSounds = new GameObject("mg_jr_multiplay");
				m_loadedMultiSounds.transform.parent = m_loadedSoundsContainer.transform;
				m_loadedSingleSounds = new GameObject("mg_jr_singleplay");
				m_loadedSingleSounds.transform.parent = m_loadedSoundsContainer.transform;
				m_loadedSoundLoops = new GameObject("mg_jr_soundloops");
				m_loadedSoundLoops.transform.parent = m_loadedSoundsContainer.transform;
				m_loadedMusic = new GameObject("mg_jr_music");
				m_loadedMusic.transform.parent = m_loadedSoundsContainer.transform;
			}
			GameObject[] array = Resources.LoadAll<GameObject>("JetpackReboot/Sound/SinglePlay/");
			GameObject[] array2 = array;
			foreach (GameObject original in array2)
			{
				GameObject gameObject = Object.Instantiate(original);
				MinigameSFX component = gameObject.GetComponent<MinigameSFX>();
				component.AudioTrack = gameObject.GetComponent<AudioSource>();
				gameObject.transform.parent = m_loadedSingleSounds.transform;
			}
			GameObject[] array3 = Resources.LoadAll<GameObject>("JetpackReboot/Sound/MultiPlay/");
			array2 = array3;
			foreach (GameObject original in array2)
			{
				GameObject gameObject = Object.Instantiate(original);
				MinigameSFX component = gameObject.GetComponent<MinigameSFX>();
				component.AudioTrack = gameObject.GetComponent<AudioSource>();
				gameObject.transform.parent = m_loadedSingleSounds.transform;
				for (int j = 0; j < 3; j++)
				{
					GameObject gameObject2 = Object.Instantiate(original);
					gameObject2.transform.parent = m_loadedMultiSounds.transform;
				}
			}
			GameObject[] array4 = Resources.LoadAll<GameObject>("JetpackReboot/Sound/Loops/");
			array2 = array4;
			foreach (GameObject original2 in array2)
			{
				GameObject gameObject3 = Object.Instantiate(original2);
				MinigameSFX component = gameObject3.GetComponent<MinigameSFX>();
				component.AudioTrack = gameObject3.GetComponent<AudioSource>();
				gameObject3.transform.parent = m_loadedSoundLoops.transform;
			}
			GameObject[] array5 = Resources.LoadAll<GameObject>("JetpackReboot/Sound/Music/");
			array2 = array5;
			foreach (GameObject original3 in array2)
			{
				GameObject gameObject4 = Object.Instantiate(original3);
				m_musicResources.Add(gameObject4.GetComponent<AudioSource>());
				gameObject4.transform.parent = m_loadedMusic.transform;
			}
			GameSoundsAreInstantiated = true;
		}

		protected bool LoadResource(mg_jr_EnvironmentLayerID _id)
		{
			bool result = false;
			if (!m_layerResources.ContainsKey(_id))
			{
				GameObject gameObject = Resources.Load(_id.ResourceFileName()) as GameObject;
				if (gameObject != null)
				{
					m_layerResources.Add(_id, gameObject);
					result = true;
				}
				else
				{
					DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Couldn't load resource with id: " + _id.ToString() + ", and prefab name: " + _id.ResourceFileName());
				}
			}
			else
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Attempting to register same EnvironmentLayer resource twice");
			}
			return result;
		}

		protected void LoadPooledResource(string _resourcePath, mg_jr_ResourceList _assetTag, mg_ICreator<GameObject> _creator)
		{
			if (!m_pooledResources.ContainsKey(_assetTag))
			{
				LoadResource(_resourcePath, _assetTag);
				GameObject gameObject = new GameObject("mg_jr_Pool_" + _assetTag);
				mg_GameObjectPool mg_GameObjectPool = gameObject.AddComponent<mg_GameObjectPool>();
				mg_GameObjectPool.Init(_creator);
				gameObject.transform.parent = ResourceContainer.transform;
				m_pooledResources.Add(_assetTag, mg_GameObjectPool);
			}
			else
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "LoadPooledResource: Attempting to register same resource twice");
			}
		}

		protected void LoadPooledResource(string _resourcePath, mg_jr_ResourceList _assetTag)
		{
			LoadPooledResource(_resourcePath, _assetTag, new mg_jr_ObstacleFactory(_assetTag));
		}

		public void UnloadGameSounds()
		{
			Object.Destroy(m_loadedSoundsContainer);
			GameSoundsAreInstantiated = false;
		}

		public GameObject GetPooledResource(mg_jr_ResourceList _assetTag)
		{
			GameObject gameObject = null;
			if (m_pooledResources.ContainsKey(_assetTag))
			{
				mg_GameObjectPool mg_GameObjectPool = m_pooledResources[_assetTag];
				gameObject = mg_GameObjectPool.Fetch();
			}
			else
			{
				gameObject = GetInstancedResource(_assetTag);
			}
			if (gameObject == null)
			{
				gameObject = new GameObject("ErrorObject-MissingResource-" + _assetTag);
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "ErrorObject-MissingResource-" + _assetTag);
			}
			return gameObject;
		}

		public GameObject GetPooledResource(string _xmlResourceName, EnvironmentVariant _variation)
		{
			return GetPooledResource(JrResourceForXmlResource(_xmlResourceName, _variation));
		}

		public void ReturnPooledResource(GameObject _instancedResource)
		{
			mg_jr_Pooled component = _instancedResource.GetComponent<mg_jr_Pooled>();
			if (component != null)
			{
				mg_GameObjectPool mg_GameObjectPool = null;
				foreach (mg_GameObjectPool value in m_pooledResources.Values)
				{
					if (value == component.ManagingPool)
					{
						mg_GameObjectPool = value;
						break;
					}
				}
				Assert.NotNull(mg_GameObjectPool, "Returning resource to a pool that doesn't exist.");
				mg_GameObjectPool.Return(_instancedResource);
			}
			else
			{
				_instancedResource.transform.parent = null;
				Object.Destroy(_instancedResource);
			}
		}

		public T GetPooledResourceByComponent<T>(mg_jr_ResourceList _assetTag) where T : MonoBehaviour
		{
			T val = null;
			GameObject pooledResource = GetPooledResource(_assetTag);
			if (pooledResource == null)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(_assetTag, string.Concat("Attempt to load '", _assetTag, "' failed."));
			}
			else
			{
				val = pooledResource.GetComponent<T>();
				if ((Object)val == (Object)null)
				{
					ReturnPooledResource(pooledResource);
					DisneyMobile.CoreUnitySystems.Logger.LogWarning(pooledResource, string.Concat("'", _assetTag, "' does not contain a component of specified type."));
				}
			}
			return val;
		}

		public mg_jr_ParallaxLayer GetInstancedParallaxLayer(EnvironmentType _environment, EnvironmentVariant _variant, EnvironmentLayer _layer)
		{
			mg_jr_EnvironmentLayerID mg_jr_EnvironmentLayerID = new mg_jr_EnvironmentLayerID(_environment, _variant, _layer);
			if (!m_layerResources.ContainsKey(mg_jr_EnvironmentLayerID))
			{
				Assert.Fail("No layer " + mg_jr_EnvironmentLayerID.ToString() + " found");
			}
			GameObject gameObject = Object.Instantiate(m_layerResources[mg_jr_EnvironmentLayerID]);
			return gameObject.GetComponent<mg_jr_ParallaxLayer>();
		}
	}
}
