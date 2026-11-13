using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_SpawnManager : MonoBehaviour
	{
		public TextAsset SpawnData;

		private mg_IceFishing m_minigame;

		private Transform m_gameTransform;

		private List<mg_if_SpawnGroup> m_spawnTable = new List<mg_if_SpawnGroup>();

		private mg_if_SpawnGroup m_spawnPuffle;

		private float m_spawnSpeed;

		private int m_spawnLevel;

		private mg_if_SpawnGroup m_spawnGroup;

		private int m_spawnGroupIndex;

		private float m_spawnGroupStartTime;

		private float m_runningTime;

		private List<mg_if_GameObject> m_puffleList = new List<mg_if_GameObject>();

		private List<mg_if_GameObject> m_fishList = new List<mg_if_GameObject>();

		private List<mg_if_GameObject> m_kickerList = new List<mg_if_GameObject>();

		private List<mg_if_GameObject> m_jellyfishList = new List<mg_if_GameObject>();

		private List<mg_if_GameObject> m_crabList = new List<mg_if_GameObject>();

		private List<mg_if_GameObject> m_extraLifeList = new List<mg_if_GameObject>();

		private List<mg_if_GameObject> m_sharkNearList = new List<mg_if_GameObject>();

		private mg_if_SharkFar m_sharkFar;

		private Localizer localizer;

		private void Awake()
		{
			m_minigame = MinigameManager.GetActive<mg_IceFishing>();
			if (Service.IsSet<Localizer>())
			{
				localizer = Service.Get<Localizer>();
			}
			LoadXML();
		}

		private void LoadXML()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(SpawnData.text);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("puffle");
			m_spawnPuffle = new mg_if_SpawnGroup();
			LoadSpawnObject(m_spawnPuffle, elementsByTagName[0]);
			XmlNodeList elementsByTagName2 = xmlDocument.GetElementsByTagName("group");
			foreach (XmlNode item in elementsByTagName2)
			{
				mg_if_SpawnGroup mg_if_SpawnGroup = new mg_if_SpawnGroup();
				m_spawnTable.Add(mg_if_SpawnGroup);
				LoadSpawnObject(mg_if_SpawnGroup, item);
			}
		}

		private void LoadSpawnObject(mg_if_SpawnGroup p_spawnGroup, XmlNode p_xmlGroupNode)
		{
			foreach (XmlNode childNode in p_xmlGroupNode.ChildNodes)
			{
				p_spawnGroup.objects.Add(new mg_if_SpawnObject(childNode.Attributes["time"].Value, childNode.Attributes["type"].Value));
			}
		}

		private void Start()
		{
			m_gameTransform = m_minigame.GetComponentInChildren<mg_if_GameLogic>().transform;
			m_spawnSpeed = m_minigame.Resources.Variables.StartSpawnSpeed;
			m_spawnLevel = 10;
			m_spawnGroup = null;
			m_spawnGroupIndex = 0;
			m_spawnGroupStartTime = 0f;
			m_runningTime = 0f;
			CreateGameObjects();
			if (localizer != null)
			{
				SpawnPuffle(localizer.GetTokenTranslation("Activity.MiniGames.FishGoal"));
			}
			else
			{
				SpawnPuffle("Collect 10 fish to win!");
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_runningTime += p_deltaTime;
			UpdateList(m_fishList, p_deltaTime);
			UpdateList(m_kickerList, p_deltaTime);
			UpdateList(m_jellyfishList, p_deltaTime);
			UpdateList(m_sharkNearList, p_deltaTime);
			UpdateList(m_crabList, p_deltaTime);
			UpdateList(m_extraLifeList, p_deltaTime);
			if (m_sharkFar.State == mg_if_EObjectState.STATE_ACTIVE)
			{
				m_sharkFar.MinigameUpdate(p_deltaTime);
			}
			if (m_spawnGroup == null || m_runningTime > m_spawnGroupStartTime + m_spawnSpeed)
			{
				int num = Random.Range(0, m_spawnLevel);
				DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Changing Spawn Group to " + num, DisneyMobile.CoreUnitySystems.Logger.TagFlags.GAME);
				m_spawnGroup = m_spawnTable[num];
				m_spawnGroupIndex = 0;
				m_spawnGroupStartTime = m_runningTime;
			}
			while (m_spawnGroupIndex < m_spawnGroup.objects.Count)
			{
				mg_if_SpawnObject mg_if_SpawnObject = m_spawnGroup.objects[m_spawnGroupIndex];
				if ((float)mg_if_SpawnObject.Time / 1000f > m_runningTime - m_spawnGroupStartTime)
				{
					break;
				}
				if (mg_if_SpawnObject.Type != 0)
				{
					SpawnObject(mg_if_SpawnObject.Type);
				}
				m_spawnGroupIndex++;
			}
		}

		private void UpdateList(List<mg_if_GameObject> p_list, float p_deltaTime)
		{
			foreach (mg_if_GameObject item in p_list)
			{
				if (item.State == mg_if_EObjectState.STATE_ACTIVE)
				{
					item.MinigameUpdate(p_deltaTime);
				}
			}
		}

		private mg_if_GameObject GetRandomGameObject(List<mg_if_GameObject> p_list)
		{
			int num = Random.Range(0, p_list.Count);
			int count = p_list.Count;
			for (int i = 0; i < count; i++)
			{
				if (p_list[num].State == mg_if_EObjectState.STATE_INACTIVE)
				{
					return p_list[num];
				}
				num++;
				if (num == count)
				{
					num = 0;
				}
			}
			return null;
		}

		private void CreateGameObjects()
		{
			GameObject gameObject = null;
			gameObject = CreateObjectForList(m_puffleList, mg_if_EResourceList.GAME_PUFFLE);
			for (int i = 0; i < m_minigame.Resources.Variables.MaxFish; i++)
			{
				gameObject = CreateObjectForList(m_fishList, mg_if_EResourceList.GAME_YELLOWFISH);
			}
			gameObject = CreateObjectForList(m_kickerList, mg_if_EResourceList.GAME_BARREL);
			gameObject.GetComponent<mg_if_Kicker>().Initialize(mg_if_EObjectsMovement.MOVEMENT_RIGHT);
			gameObject = CreateObjectForList(m_kickerList, mg_if_EResourceList.GAME_BARREL);
			gameObject.GetComponent<mg_if_Kicker>().Initialize(mg_if_EObjectsMovement.MOVEMENT_LEFT);
			gameObject = CreateObjectForList(m_kickerList, mg_if_EResourceList.GAME_BOOT);
			gameObject.GetComponent<mg_if_Kicker>().Initialize(mg_if_EObjectsMovement.MOVEMENT_RIGHT);
			gameObject = CreateObjectForList(m_kickerList, mg_if_EResourceList.GAME_BOOT);
			gameObject.GetComponent<mg_if_Kicker>().Initialize(mg_if_EObjectsMovement.MOVEMENT_LEFT);
			gameObject = CreateObjectForList(m_jellyfishList, mg_if_EResourceList.GAME_JELLYFISH);
			gameObject.GetComponent<mg_if_JellyFish>().Initialize(mg_if_EObjectsMovement.MOVEMENT_RIGHT);
			gameObject = CreateObjectForList(m_jellyfishList, mg_if_EResourceList.GAME_JELLYFISH);
			gameObject.GetComponent<mg_if_JellyFish>().Initialize(mg_if_EObjectsMovement.MOVEMENT_LEFT);
			gameObject = CreateObjectForList(m_sharkNearList, mg_if_EResourceList.GAME_SHARK_NEAR);
			gameObject.GetComponent<mg_if_SharkNear>().Initialize(this);
			m_sharkFar = CreateObject(mg_if_EResourceList.GAME_SHARK_FAR).GetComponent<mg_if_SharkFar>();
			gameObject = CreateObjectForList(m_crabList, mg_if_EResourceList.GAME_CRAB);
			gameObject.GetComponent<mg_if_Crab>().Initialize(mg_if_EObjectsMovement.MOVEMENT_RIGHT);
			gameObject = CreateObjectForList(m_crabList, mg_if_EResourceList.GAME_CRAB);
			gameObject.GetComponent<mg_if_Crab>().Initialize(mg_if_EObjectsMovement.MOVEMENT_LEFT);
			gameObject = CreateObjectForList(m_extraLifeList, mg_if_EResourceList.GAME_FREE_LIFE);
		}

		private GameObject CreateObjectForList(List<mg_if_GameObject> p_list, mg_if_EResourceList p_tag)
		{
			GameObject gameObject = CreateObject(p_tag);
			p_list.Add(gameObject.GetComponent<mg_if_GameObject>());
			return gameObject;
		}

		private GameObject CreateObject(mg_if_EResourceList p_tag)
		{
			GameObject instancedResource = m_minigame.Resources.GetInstancedResource(p_tag);
			MinigameSpriteHelper.AssignParentTransform(instancedResource, m_gameTransform);
			return instancedResource;
		}

		private void SpawnPuffle(string p_text)
		{
			m_spawnGroup = m_spawnPuffle;
			m_spawnGroupIndex = 0;
			m_spawnGroupStartTime = m_runningTime;
			(m_puffleList[0] as mg_if_Puffle).SetText(p_text);
		}

		private void SpawnObject(mg_if_EObjectType p_type)
		{
			DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Spawning " + p_type, DisneyMobile.CoreUnitySystems.Logger.TagFlags.GAME);
			mg_if_GameObject mg_if_GameObject = null;
			switch (p_type)
			{
			case mg_if_EObjectType.OBJ_PUFFLE:
				mg_if_GameObject = GetRandomGameObject(m_puffleList);
				break;
			case mg_if_EObjectType.OBJ_YELLOWFISH:
			case mg_if_EObjectType.OBJ_GREYFISH:
				mg_if_GameObject = GetRandomGameObject(m_fishList);
				break;
			case mg_if_EObjectType.OBJ_KICKER:
				mg_if_GameObject = GetRandomGameObject(m_kickerList);
				break;
			case mg_if_EObjectType.OBJ_JELLYFISH:
				mg_if_GameObject = GetRandomGameObject(m_jellyfishList);
				break;
			case mg_if_EObjectType.OBJ_SHARK:
				mg_if_GameObject = GetSharkNear();
				break;
			case mg_if_EObjectType.OBJ_CRAB:
				mg_if_GameObject = GetRandomGameObject(m_crabList);
				break;
			case mg_if_EObjectType.OBJ_WORMCAN:
				if (m_minigame.Logic.Lives < m_minigame.Resources.Variables.LivesLimit)
				{
					mg_if_GameObject = GetRandomGameObject(m_extraLifeList);
				}
				break;
			}
			if (mg_if_GameObject != null)
			{
				mg_if_GameObject.Spawn();
			}
		}

		private mg_if_GameObject GetSharkNear()
		{
			mg_if_GameObject result = null;
			if (m_sharkFar.State == mg_if_EObjectState.STATE_INACTIVE)
			{
				result = GetRandomGameObject(m_sharkNearList);
			}
			return result;
		}

		public void SpawnShark(mg_if_EObjectsMovement p_movement)
		{
			m_sharkFar.Spawn();
			m_sharkFar.Initialize(p_movement);
		}

		public void CheckGameRound(int p_fishCaught)
		{
			int num = 0;
			string p_text = "";
			switch (p_fishCaught)
			{
			case 10:
				num = 20;
				p_text = "Careful, jellyfish can shock your line";
				if (localizer != null)
				{
					p_text = localizer.GetTokenTranslation("Activity.MiniGames.FishTip4");
				}
				break;
			case 20:
				num = 30;
				p_text = "Watch out for sharks";
				if (localizer != null)
				{
					p_text = localizer.GetTokenTranslation("Activity.MiniGames.FishTip5");
				}
				break;
			case 40:
				num = 40;
				p_text = "Danger! Crabs have sharp claws";
				if (localizer != null)
				{
					p_text = localizer.GetTokenTranslation("Activity.MiniGames.FishTip6");
				}
				break;
			case 60:
				m_minigame.Logic.EndGame();
				break;
			}
			if (num > 0)
			{
				m_spawnLevel = num;
				m_spawnSpeed -= m_minigame.Resources.Variables.SpawnSpeedIncrease;
				SpawnPuffle(p_text);
			}
		}

		public void DebugSpawn()
		{
			m_spawnLevel = m_spawnTable.Count - 1;
			m_spawnSpeed = 2.5f;
			SpawnObject(mg_if_EObjectType.OBJ_CRAB);
			SpawnObject(mg_if_EObjectType.OBJ_GREYFISH);
			SpawnObject(mg_if_EObjectType.OBJ_JELLYFISH);
			SpawnObject(mg_if_EObjectType.OBJ_KICKER);
			SpawnObject(mg_if_EObjectType.OBJ_PUFFLE);
			SpawnObject(mg_if_EObjectType.OBJ_SHARK);
			SpawnObject(mg_if_EObjectType.OBJ_WORMCAN);
			SpawnObject(mg_if_EObjectType.OBJ_YELLOWFISH);
		}
	}
}
