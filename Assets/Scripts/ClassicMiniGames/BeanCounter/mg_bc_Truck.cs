using MinigameFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_Truck : MonoBehaviour
	{
		public const float EXIT_DURATION = 1.5f;

		private const float GRAVITY_SCALE = 2f;

		private Vector2[] VELOCITIES = new Vector2[4]
		{
			new Vector2(-140f, 550f),
			new Vector2(-220f, 550f),
			new Vector2(-280f, 600f),
			new Vector2(-370f, 550f)
		};

		private Action m_onRoundTransitionHalf;

		private Action m_onRoundTransitionEnded;

		private List<mg_bc_Level> m_levels;

		protected mg_bc_Level m_activeLevel;

		private List<mg_bc_FlyingObject> m_groundedObjects = new List<mg_bc_FlyingObject>();

		private List<mg_bc_FlyingObject> m_flyingObjects = new List<mg_bc_FlyingObject>();

		private List<mg_bc_FlyingObject> m_heldObjects = new List<mg_bc_FlyingObject>();

		private Vector3 m_restingLocation;

		private Vector3 m_spawnLocation;

		private Vector3 m_exitLocation;

		private float m_exitSpeed;

		private float m_exitElapsed;

		private float m_timeSinceLastSpawn;

		private bool m_isExiting;

		private bool m_isFlyingPowerup;

		private Queue<int> m_availableSpriteLayers;

		public mg_bc_UIValue<int> TruckNumber
		{
			get;
			private set;
		}

		public bool IsSpawning
		{
			get;
			set;
		}

		public bool CanSpawnLives
		{
			get;
			set;
		}

		public bool CanSpawnShield
		{
			get;
			set;
		}

		public mg_bc_UIValue<int> CurrentDropAllowance
		{
			get;
			private set;
		}

		public bool IsExiting
		{
			get
			{
				return m_isExiting;
			}
		}

		public mg_bc_Truck()
		{
			TruckNumber = new mg_bc_UIValue<int>();
		}

		public virtual void Awake()
		{
			CurrentDropAllowance = new mg_bc_UIValue<int>();
			ParseXML();
			m_restingLocation = base.gameObject.transform.localPosition;
			m_spawnLocation = base.transform.Find("mg_bc_object_spawn").localPosition;
			m_spawnLocation.x += m_restingLocation.x;
			m_spawnLocation.y += m_restingLocation.y;
			m_availableSpriteLayers = new Queue<int>();
			for (int i = 21; i < 79; i++)
			{
				m_availableSpriteLayers.Enqueue(i);
			}
			IsSpawning = true;
			m_isFlyingPowerup = false;
			CanSpawnLives = true;
			CanSpawnShield = true;
			TruckNumber.SetValue(0);
			NextLevel();
		}

		protected virtual void ParseXML()
		{
		}

		protected void ParseXMLDoc(XmlDocument _document)
		{
			m_levels = new List<mg_bc_Level>();
			XmlNodeList elementsByTagName = _document.GetElementsByTagName("Truck");
			foreach (XmlNode item in elementsByTagName)
			{
				mg_bc_Level mg_bc_Level = new mg_bc_Level();
				m_levels.Add(mg_bc_Level);
				ParseLevelData(mg_bc_Level, item);
			}
		}

		private void ParseLevelData(mg_bc_Level _level, XmlNode _xmlLevel)
		{
			_level.LevelObjects = new List<mg_bc_LevelObject>();
			_level.PowerUps = new List<mg_bc_LevelObject>();
			_level.Goal = Convert.ToInt32(_xmlLevel.Attributes["Goal"].Value, CultureInfo.InvariantCulture);
			_level.MaxFlyingObjects = Convert.ToInt32(_xmlLevel.Attributes["MaxFlyingObjects"].Value, CultureInfo.InvariantCulture);
			_level.PowerupChance = (float)Convert.ToDouble(_xmlLevel.Attributes["PowerupChance"].Value, CultureInfo.InvariantCulture);
			if (_xmlLevel.Attributes.GetNamedItem("DropAllowance") != null)
			{
				_level.DropAllowance = Convert.ToInt32(_xmlLevel.Attributes["DropAllowance"].Value, CultureInfo.InvariantCulture);
			}
			if (_xmlLevel.Attributes.GetNamedItem("ThrownBagTypes") != null)
			{
				_level.ThrownBagTypes = Convert.ToInt32(_xmlLevel.Attributes["ThrownBagTypes"].Value, CultureInfo.InvariantCulture);
			}
			if (_xmlLevel.Attributes.GetNamedItem("WrongBagTypes") != null)
			{
				_level.WrongBagTypes = Convert.ToInt32(_xmlLevel.Attributes["WrongBagTypes"].Value, CultureInfo.InvariantCulture);
			}
			if (_xmlLevel.Attributes.GetNamedItem("SpawnDelay") != null)
			{
				_level.SpawnDelay = (float)Convert.ToDouble(_xmlLevel.Attributes["SpawnDelay"].Value, CultureInfo.InvariantCulture);
			}
			foreach (XmlNode childNode in _xmlLevel.ChildNodes)
			{
				ParseXMLChild(childNode, _level);
			}
		}

		private void ParseXMLChild(XmlNode _child, mg_bc_Level _level)
		{
			if (_child.Name == "Objects")
			{
				foreach (XmlNode childNode in _child.ChildNodes)
				{
					ParseLevelXMLObject(childNode, _level);
				}
			}
			else if (_child.Name == "PowerUps")
			{
				foreach (XmlNode childNode2 in _child.ChildNodes)
				{
					ParseLevelXMLPowerup(childNode2, _level);
				}
			}
		}

		private void ParseLevelXMLPowerup(XmlNode _xml, mg_bc_Level _level)
		{
			mg_bc_LevelObject mg_bc_LevelObject = new mg_bc_LevelObject();
			_level.PowerUps.Add(mg_bc_LevelObject);
			mg_bc_LevelObject.Type = _xml.Attributes["type"].Value;
			mg_bc_LevelObject.Odds = Convert.ToInt32(_xml.Attributes["odds"].Value);
			_level.TotalPowerUpOdds += mg_bc_LevelObject.Odds;
		}

		private void ParseLevelXMLObject(XmlNode _xml, mg_bc_Level _level)
		{
			mg_bc_LevelObject mg_bc_LevelObject = new mg_bc_LevelObject();
			_level.LevelObjects.Add(mg_bc_LevelObject);
			mg_bc_LevelObject.Type = _xml.Attributes["type"].Value;
			mg_bc_LevelObject.Odds = Convert.ToInt32(_xml.Attributes["odds"].Value);
			_level.TotalObjectOdds += mg_bc_LevelObject.Odds;
		}

		public int Goal()
		{
			return m_activeLevel.Goal;
		}

		public virtual bool NextLevel()
		{
			TruckNumber.SetValue(TruckNumber.Value + 1);
			if (TruckNumber.Value > m_levels.Count)
			{
				return false;
			}
			m_activeLevel = m_levels[TruckNumber.Value - 1];
			CurrentDropAllowance.SetValue(m_activeLevel.DropAllowance);
			return true;
		}

		public void TruckUpdate(float _delta)
		{
			if (m_isExiting)
			{
				ExitUpdate(_delta);
			}
			ObjectUpdate(_delta);
			m_timeSinceLastSpawn += _delta;
			TrySpawn();
		}

		protected virtual void ObjectUpdate(float _delta)
		{
			UpdateList(m_groundedObjects, _delta);
			UpdateList(m_heldObjects, _delta);
			UpdateList(m_flyingObjects, _delta);
		}

		protected void UpdateList(List<mg_bc_FlyingObject> _list, float _delta)
		{
			for (int num = _list.Count - 1; num >= 0; num--)
			{
				mg_bc_FlyingObject mg_bc_FlyingObject = _list[num];
				mg_bc_FlyingObject.FlyingUpdate(_delta);
				if (mg_bc_FlyingObject.State == mg_bc_EObjectState.STATE_TO_DESTROY)
				{
					if (m_isFlyingPowerup && mg_bc_FlyingObject is mg_bc_Powerup)
					{
						m_isFlyingPowerup = false;
					}
					_list.RemoveAt(num);
					DestroyFlyer(mg_bc_FlyingObject);
				}
				else if (mg_bc_FlyingObject.State == mg_bc_EObjectState.STATE_FADING_ON_GROUND)
				{
					if (_list != m_groundedObjects)
					{
						_list.RemoveAt(num);
						m_groundedObjects.Add(mg_bc_FlyingObject);
						OnObjectHitGround(mg_bc_FlyingObject);
					}
				}
				else if (mg_bc_FlyingObject.State == mg_bc_EObjectState.STATE_HELD && _list != m_heldObjects)
				{
					_list.RemoveAt(num);
					m_heldObjects.Add(mg_bc_FlyingObject);
				}
			}
		}

		private void OnObjectHitGround(mg_bc_FlyingObject _groundedObject)
		{
			MinigameManager.GetActive<mg_BeanCounter>().GameLogic.OnObjectDropped(_groundedObject);
		}

		private void TrySpawn()
		{
			bool flag = IsSpawning && !m_isExiting;
			if (flag && flag && m_flyingObjects.Count < m_activeLevel.MaxFlyingObjects && m_timeSinceLastSpawn >= m_activeLevel.SpawnDelay)
			{
				Spawn();
			}
		}

		private void Spawn()
		{
			GameObject gameObject = GenerateObjectToSpawn();
			if (gameObject != null)
			{
				mg_bc_FlyingObject component = gameObject.GetComponent<mg_bc_FlyingObject>();
				MinigameSpriteHelper.AssignParentTransform(gameObject, base.transform.parent);
				m_flyingObjects.Add(component);
				gameObject.transform.localPosition = m_spawnLocation;
				component.SetLayers(m_availableSpriteLayers.Dequeue(), m_availableSpriteLayers.Dequeue());
				string name = "mg_bc_sfx_BagThrowWhoosh0" + UnityEngine.Random.Range(1, 4);
				MinigameManager.GetActive().PlaySFX(name);
				m_timeSinceLastSpawn = 0f;
				Rigidbody2D component2 = gameObject.GetComponent<Rigidbody2D>();
				component2.gravityScale = 2f;
				int num = UnityEngine.Random.Range(0, VELOCITIES.Length);
				component2.AddForce(VELOCITIES[num]);
			}
		}

		private GameObject GenerateObjectToSpawn()
		{
			mg_bc_LevelObject mg_bc_LevelObject = null;
			float value = UnityEngine.Random.value;
			if (value <= m_activeLevel.PowerupChance)
			{
				int num = UnityEngine.Random.Range(0, m_activeLevel.TotalPowerUpOdds);
				foreach (mg_bc_LevelObject powerUp in m_activeLevel.PowerUps)
				{
					if (num < powerUp.Odds && CanSpawn(powerUp.Type))
					{
						mg_bc_LevelObject = powerUp;
						break;
					}
					num -= powerUp.Odds;
				}
			}
			if (mg_bc_LevelObject == null)
			{
				int num = UnityEngine.Random.Range(0, m_activeLevel.TotalObjectOdds);
				foreach (mg_bc_LevelObject levelObject in m_activeLevel.LevelObjects)
				{
					if (num < levelObject.Odds && CanSpawn(levelObject.Type))
					{
						mg_bc_LevelObject = levelObject;
						break;
					}
					num -= levelObject.Odds;
				}
			}
			GameObject gameObject = null;
			if (mg_bc_LevelObject == null)
			{
				return GetBag();
			}
			return CreateLevelObject(mg_bc_LevelObject);
		}

		private GameObject CreateLevelObject(mg_bc_LevelObject _type)
		{
			GameObject gameObject = null;
			if (_type.Type == "anvil")
			{
				gameObject = GetHazard(mg_bc_EHazardType.HAZARD_ANVIL);
			}
			else if (_type.Type == "flowers")
			{
				gameObject = GetHazard(mg_bc_EHazardType.HAZARD_FLOWERS);
			}
			else if (_type.Type == "fish")
			{
				gameObject = GetHazard(mg_bc_EHazardType.HAZARD_FISH);
			}
			else if (_type.Type == "extra_life")
			{
				gameObject = GetLifePowerup();
				m_isFlyingPowerup = true;
			}
			else if (_type.Type == "shield")
			{
				gameObject = GetShieldPowerup();
				m_isFlyingPowerup = true;
			}
			else
			{
				gameObject = GetBag();
			}
			return gameObject;
		}

		private bool CanSpawn(string _object)
		{
			if (_object == "extra_life")
			{
				return CanSpawnLives && !m_isFlyingPowerup;
			}
			if (_object == "shield")
			{
				return CanSpawnShield && !m_isFlyingPowerup;
			}
			return true;
		}

		private GameObject GetShieldPowerup()
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			return active.Resources.GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_SHIELD);
		}

		private GameObject GetLifePowerup()
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			return active.Resources.GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_ONE_UP);
		}

		protected virtual GameObject GetBag()
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			return active.Resources.GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_BAG);
		}

		private GameObject GetHazard(mg_bc_EHazardType _type)
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			GameObject instancedResource = active.Resources.GetInstancedResource(mg_bc_EResourceList.GAME_ASSET_HAZARD);
			mg_bc_Hazard component = instancedResource.GetComponent<mg_bc_Hazard>();
			component.SetType(_type);
			return instancedResource;
		}

		private void DestroyFlyer(mg_bc_FlyingObject _flyer)
		{
			int _primary;
			int _secondary;
			_flyer.GetLayers(out _primary, out _secondary);
			m_availableSpriteLayers.Enqueue(_primary);
			m_availableSpriteLayers.Enqueue(_secondary);
			UnityEngine.Object.Destroy(_flyer.gameObject);
		}

		public void DestroyAll()
		{
			DestroyObjectsInList(m_groundedObjects);
			DestroyObjectsInList(m_heldObjects);
			DestroyObjectsInList(m_flyingObjects);
		}

		private void DestroyObjectsInList(List<mg_bc_FlyingObject> _list)
		{
			for (int num = _list.Count - 1; num >= 0; num--)
			{
				mg_bc_FlyingObject flyer = _list[num];
				_list.RemoveAt(num);
				DestroyFlyer(flyer);
			}
		}

		internal void StartRoundTransition(Action _halfCallback, Action _completeCallback)
		{
			m_onRoundTransitionHalf = _halfCallback;
			m_onRoundTransitionEnded = _completeCallback;
			Vector2 screenEdge = MinigameSpriteHelper.GetScreenEdge(EScreenEdge.RIGHT, MinigameManager.GetActive<mg_BeanCounter>().MainCamera);
			m_exitLocation = new Vector3(screenEdge.x, screenEdge.y);
			float num = m_exitLocation.x - base.transform.localPosition.x;
			m_exitSpeed = num / 1.5f;
			m_exitElapsed = 0f;
			m_isExiting = true;
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UITruckUnloaded");
		}

		private void ExitUpdate(float _delta)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x + m_exitSpeed * _delta, base.transform.localPosition.y);
			m_exitElapsed += _delta;
			if (m_exitElapsed >= 1.5f)
			{
				if (m_exitSpeed > 0f)
				{
					OnRoundTransitionHalf();
				}
				else
				{
					OnRoundTransitionEnded();
				}
			}
		}

		protected virtual void OnRoundTransitionHalf()
		{
			m_onRoundTransitionHalf();
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UITruckNext");
			m_exitSpeed *= -1f;
			m_exitElapsed = 0f;
		}

		private void OnRoundTransitionEnded()
		{
			m_onRoundTransitionEnded();
			Vector2 screenEdge = MinigameSpriteHelper.GetScreenEdge(EScreenEdge.RIGHT, MinigameManager.GetActive<mg_BeanCounter>().MainCamera);
			m_exitLocation = new Vector3(screenEdge.x, screenEdge.y);
			m_isExiting = false;
		}

		internal int DeductDropAllowance()
		{
			int value = CurrentDropAllowance.Value;
			int num = CurrentDropAllowance.Value;
			if (!m_isExiting && num >= 0)
			{
				num--;
				if (num < 0)
				{
					num = m_activeLevel.DropAllowance;
				}
				CurrentDropAllowance.SetValue(num);
			}
			return num - value;
		}

		internal virtual void TakeBag()
		{
		}
	}
}
