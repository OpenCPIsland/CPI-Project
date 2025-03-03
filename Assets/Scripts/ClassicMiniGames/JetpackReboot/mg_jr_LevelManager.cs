using CameraExtensionMethods;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_LevelManager : MonoBehaviour
	{
		protected struct DifficultyRange
		{
			public int Minimum
			{
				get;
				set;
			}

			public int Maximum
			{
				get;
				set;
			}

			public DifficultyRange(int _singleValueRange)
			{
				this = default(DifficultyRange);
				Minimum = _singleValueRange;
				Maximum = _singleValueRange;
			}

			public DifficultyRange(int _min, int _max)
			{
				this = default(DifficultyRange);
				Minimum = _min;
				Maximum = _max;
			}

			public int RandomDifficulty()
			{
				return Random.Range(Minimum, Maximum);
			}

			public bool IsDifficultyInRange(int _difficulty)
			{
				return _difficulty >= Minimum && _difficulty <= Maximum;
			}

			public int ClampToRange(int _difficulty)
			{
				return Mathf.Clamp(_difficulty, Minimum, Maximum);
			}
		}

		protected struct EnvironmentTypeAndDifficultyID
		{
			private EnvironmentType m_environmentType;

			private int m_difficulty;

			public EnvironmentTypeAndDifficultyID(EnvironmentType _type, int _difficulty)
			{
				Assert.AreNotEqual(_type, EnvironmentType.MAX);
				Assert.IsTrue(_difficulty > 0);
				m_environmentType = _type;
				m_difficulty = _difficulty;
			}

			public static bool operator ==(EnvironmentTypeAndDifficultyID _left, EnvironmentTypeAndDifficultyID _right)
			{
				return _left.m_environmentType == _right.m_environmentType && _left.m_difficulty == _right.m_difficulty;
			}

			public static bool operator !=(EnvironmentTypeAndDifficultyID _left, EnvironmentTypeAndDifficultyID _right)
			{
				return !(_left == _right);
			}

			public override bool Equals(object obj)
			{
				return obj is EnvironmentTypeAndDifficultyID && this == (EnvironmentTypeAndDifficultyID)obj;
			}

			public override int GetHashCode()
			{
				return (int)(m_environmentType + 5 + m_difficulty);
			}

			public override string ToString()
			{
				return m_environmentType.ToString() + "_" + m_difficulty;
			}
		}

		public const string LEVEL_BASE_FOLDER = "JetpackReboot/Levels/";

		private const float LEVEL_HEIGHT = 10.24f;

		private LinkedList<mg_jr_Level> m_activeLevels = new LinkedList<mg_jr_Level>();

		private Dictionary<EnvironmentType, DifficultyRange> m_availableDifficulties;

		private Dictionary<EnvironmentTypeAndDifficultyID, List<mg_jr_LevelDefinition>> m_levelDefinitions;

		private mg_jr_ScrollingSpeed m_scrolling = null;

		private mg_jr_Resources m_resources;

		private mg_jr_GameLogic m_gameLogic;

		private float m_fillLevelsTillX = 2f;

		private mg_jr_EnvironmentManager m_environmentManager;

		private bool m_isEnvironmentOverrideEnabled = false;

		private EnvironmentType m_environmentOverride;

		public int NumberOfLevelsActive
		{
			get
			{
				return m_activeLevels.Count;
			}
		}

		public int MaximumDifficulty
		{
			get
			{
				return m_availableDifficulties[EnvironmentType.FOREST].Maximum;
			}
		}

		public bool ContinuousLevels
		{
			get;
			set;
		}

		public mg_jr_Level LastLevelInQueue
		{
			get
			{
				mg_jr_Level result = null;
				if (m_activeLevels.Count > 0)
				{
					result = m_activeLevels.Last.Value;
				}
				return result;
			}
		}

		public float EndPositionXofQueue
		{
			get
			{
				if (m_activeLevels.Last != null)
				{
					mg_jr_Level value = m_activeLevels.Last.Value;
					float x = value.transform.position.x;
					return x + value.LevelDefinition.Size.x;
				}
				return MinigameManager.GetActive<mg_JetpackReboot>().VisibleWorldBounds.min.x;
			}
		}

		public bool IsLastLevelEmpty()
		{
			return m_activeLevels.Last.Value.LevelDefinition.IsEmpty;
		}

		private void Awake()
		{
			m_levelDefinitions = new Dictionary<EnvironmentTypeAndDifficultyID, List<mg_jr_LevelDefinition>>();
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			m_resources = active.Resources;
			m_fillLevelsTillX = Camera.main.RightEdgeInWorld() * 2f;
			m_availableDifficulties = new Dictionary<EnvironmentType, DifficultyRange>();
			LoadLevelDeffinitions(Resources.LoadAll<TextAsset>("JetpackReboot/Levels/forest/"));
			LoadLevelDeffinitions(Resources.LoadAll<TextAsset>("JetpackReboot/Levels/cave/"));
			LoadLevelDeffinitions(Resources.LoadAll<TextAsset>("JetpackReboot/Levels/town/"));
			LoadLevelDeffinitions(Resources.LoadAll<TextAsset>("JetpackReboot/Levels/water/"));
			LoadLevelDeffinitions(Resources.LoadAll<TextAsset>("JetpackReboot/Levels/common/"));
			Resources.UnloadUnusedAssets();
		}

		public void Init(mg_jr_ScrollingSpeed _scrolling, mg_jr_GameLogic _gameLogic)
		{
			m_scrolling = _scrolling;
			m_gameLogic = _gameLogic;
		}

		private void Start()
		{
			Assert.NotNull(m_scrolling, "Level manager needs a reference to a scrolling component. Make sure Init is called after component creation");
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			m_environmentManager = active.GameLogic.EnvironmentManager;
		}

		private void LoadLevelDeffinitions(TextAsset[] levelXmls)
		{
			foreach (TextAsset xmlLevelDef in levelXmls)
			{
				mg_jr_LevelDefinition mg_jr_LevelDefinition = new mg_jr_LevelDefinition();
				mg_jr_LevelDefinition.LoadFromXML(xmlLevelDef);
				EnvironmentTypeAndDifficultyID key = new EnvironmentTypeAndDifficultyID(mg_jr_LevelDefinition.EnvironmentType, mg_jr_LevelDefinition.Difficulty);
				if (!m_levelDefinitions.ContainsKey(key))
				{
					m_levelDefinitions.Add(key, new List<mg_jr_LevelDefinition>());
				}
				m_levelDefinitions[key].Add(mg_jr_LevelDefinition);
				DifficultyRange value;
				if (!m_availableDifficulties.ContainsKey(mg_jr_LevelDefinition.EnvironmentType))
				{
					value = new DifficultyRange(mg_jr_LevelDefinition.Difficulty);
					m_availableDifficulties.Add(mg_jr_LevelDefinition.EnvironmentType, value);
					continue;
				}
				value = m_availableDifficulties[mg_jr_LevelDefinition.EnvironmentType];
				value.Minimum = Mathf.Min(mg_jr_LevelDefinition.Difficulty, value.Minimum);
				value.Maximum = Mathf.Max(mg_jr_LevelDefinition.Difficulty, value.Maximum);
				m_availableDifficulties[mg_jr_LevelDefinition.EnvironmentType] = value;
			}
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			if (!m_scrolling.ScrollingEnabled)
			{
				return;
			}
			foreach (mg_jr_Level activeLevel in m_activeLevels)
			{
				Vector3 vector = new Vector3((0f - _deltaTime) * m_scrolling.CurrentSpeedForObstacles(), 0f, 0f);
				activeLevel.transform.position += vector;
			}
			if (m_activeLevels.Count > 0)
			{
				RemoveExpiredLevels();
			}
			if (ContinuousLevels && (m_activeLevels.Last == null || m_activeLevels.Last.Value.TopRightCornerInWorld().x < m_fillLevelsTillX))
			{
				if (m_isEnvironmentOverrideEnabled)
				{
					AddRandomLevelToQueue(_difficulty: m_availableDifficulties[m_environmentOverride].RandomDifficulty(), _environmentType: m_environmentOverride);
				}
				else
				{
					AddRandomLevelToQueue(m_environmentManager.CurrentEnvironment.Type, m_gameLogic.RandomisedCurrentDifficulty());
				}
			}
		}

		private mg_jr_Level LoadLevelAfterExisting(mg_jr_LevelDefinition _pattern)
		{
			_pattern.UseDeffinition();
			GameObject gameObject = new GameObject(string.Concat("mg_jr_Level_", _pattern.EnvironmentType, "_", _pattern.Difficulty));
			mg_jr_Level mg_jr_Level = gameObject.AddComponent<mg_jr_Level>();
			mg_jr_Level.LevelDefinition = _pattern;
			EnvironmentVariant variant = m_environmentManager.CurrentEnvironment.Variant;
			foreach (mg_jr_LevelDefinition.ObjectSpawnDefinition objectSpawnDefinition in _pattern.ObjectSpawnDefinitions)
			{
				GameObject pooledResource = m_resources.GetPooledResource(objectSpawnDefinition.XmlResourceName, variant);
				pooledResource.transform.parent = gameObject.transform;
				pooledResource.transform.position = objectSpawnDefinition.PositionInLevel;
				pooledResource.name = pooledResource.name + "_" + objectSpawnDefinition.Name;
			}
			Vector3 position;
			if (m_activeLevels.Count == 0)
			{
				float num = _pattern.Size.y * 0.5f - Camera.main.orthographicSize;
				position = new Vector3(Camera.main.RightEdgeInWorld(), Camera.main.TopEdgeInWorld() + num, 0f);
			}
			else
			{
				mg_jr_Level value = m_activeLevels.Last.Value;
				position = value.TopRightCornerInWorld();
			}
			gameObject.transform.position = position;
			gameObject.transform.parent = base.transform;
			m_activeLevels.AddLast(mg_jr_Level);
			return mg_jr_Level;
		}

		private bool AddLevelToQueueWith(mg_jr_ResourceList _resource, EnvironmentType _environmentType, int _difficulty)
		{
			bool result = false;
			EnvironmentTypeAndDifficultyID key = new EnvironmentTypeAndDifficultyID(_environmentType, _difficulty);
			List<mg_jr_LevelDefinition> list = m_levelDefinitions[key];
			mg_jr_Resources resources = MinigameManager.GetActive<mg_JetpackReboot>().Resources;
			foreach (mg_jr_LevelDefinition item in list)
			{
				if (item.ContainsAtLeastOne(resources.FindXmlNameOfPrefab(_resource)))
				{
					LoadLevelAfterExisting(item);
					result = true;
					break;
				}
			}
			return result;
		}

		private mg_jr_Level AddRandomLevelToQueue(EnvironmentType _environmentType, int _difficulty)
		{
			Assert.AreNotEqual(EnvironmentType.MAX, _environmentType, "Invalid value: " + _environmentType);
			_difficulty = m_availableDifficulties[_environmentType].ClampToRange(_difficulty);
			EnvironmentTypeAndDifficultyID key = new EnvironmentTypeAndDifficultyID(_environmentType, _difficulty);
			List<mg_jr_LevelDefinition> list = m_levelDefinitions[key];
			mg_jr_LevelDefinition mg_jr_LevelDefinition = null;
			int num = -1;
			foreach (mg_jr_LevelDefinition item in list)
			{
				if (!item.IsUseLimitReached)
				{
					int num2 = Random.Range(0, 1000) * item.CurrentRarity;
					if (num2 > num)
					{
						mg_jr_LevelDefinition = item;
						num = num2;
					}
				}
			}
			if (mg_jr_LevelDefinition == null)
			{
				ResetLevelRarity(_environmentType);
				mg_jr_LevelDefinition = list[0];
			}
			Assert.NotNull(mg_jr_LevelDefinition, "No useablelevel def found");
			return LoadLevelAfterExisting(mg_jr_LevelDefinition);
		}

		public void AddEmptyLevelToQueue(float _widthInWorldUnits)
		{
			Vector2 size = new Vector2(_widthInWorldUnits, 10.24f);
			mg_jr_LevelDefinition pattern = new mg_jr_LevelDefinition(size);
			LoadLevelAfterExisting(pattern);
		}

		public void AddLevelsToQueue(EnvironmentType _type, float _maxTotalWidth)
		{
			Assert.AreNotEqual(EnvironmentType.MAX, _type, "Not a valid environemnt");
			DifficultyRange difficultyRange = m_availableDifficulties[_type];
			float num = 0f;
			foreach (mg_jr_Level activeLevel in m_activeLevels)
			{
				num += activeLevel.LevelDefinition.Size.x;
			}
			int num2 = 0;
			while (num <= _maxTotalWidth)
			{
				mg_jr_Level current = AddRandomLevelToQueue(_type, difficultyRange.RandomDifficulty());
				num += current.LevelDefinition.Size.x;
				num2++;
			}
			mg_jr_Level value = m_activeLevels.Last.Value;
			m_activeLevels.RemoveLast();
			if (value != null)
			{
				value.DestroyLevel();
			}
			num2--;
			Debug.Log("_maxTotalWidth" + _maxTotalWidth);
			Debug.Log("loaded turbo levels =" + num2);
		}

		public void AddXtoQueue(EnvironmentType _type, int _numberOfLevelsToAdd)
		{
			Assert.AreNotEqual(EnvironmentType.MAX, _type, "Not a valid environemnt");
			DifficultyRange difficultyRange = m_availableDifficulties[_type];
			for (int i = 0; i <= _numberOfLevelsToAdd; i++)
			{
				AddRandomLevelToQueue(_type, difficultyRange.RandomDifficulty());
			}
		}

		public void FillQueueWith(EnvironmentType _type, int _numberOfLevelsToFillQueueTo)
		{
			Assert.AreNotEqual(EnvironmentType.MAX, _type, "Not a valid environemnt");
			if (m_activeLevels.Count >= _numberOfLevelsToFillQueueTo)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Can't fill queue with level type because queue is full");
				return;
			}
			DifficultyRange difficultyRange = m_availableDifficulties[_type];
			while (m_activeLevels.Count < _numberOfLevelsToFillQueueTo)
			{
				AddRandomLevelToQueue(_type, difficultyRange.RandomDifficulty());
			}
		}

		public void ResetLevelRarity()
		{
			ResetLevelRarity(m_environmentManager.CurrentEnvironment.Type);
		}

		public void ResetLevelRarity(EnvironmentType _forEnvironment)
		{
			Assert.AreNotEqual(EnvironmentType.MAX, _forEnvironment, "Invalid value: " + _forEnvironment);
			DifficultyRange difficultyRange = m_availableDifficulties[_forEnvironment];
			for (int i = difficultyRange.Minimum; i <= difficultyRange.Maximum; i++)
			{
				EnvironmentTypeAndDifficultyID key = new EnvironmentTypeAndDifficultyID(_forEnvironment, i);
				List<mg_jr_LevelDefinition> list = m_levelDefinitions[key];
				foreach (mg_jr_LevelDefinition item in list)
				{
					item.ResetUsage();
				}
			}
		}

		public void ResetLevelRarityAllEnvironments()
		{
			foreach (List<mg_jr_LevelDefinition> value in m_levelDefinitions.Values)
			{
				foreach (mg_jr_LevelDefinition item in value)
				{
					item.ResetUsage();
				}
			}
		}

		public void RemoveNonVisibleLevels()
		{
			while (m_activeLevels.Last != null && !m_activeLevels.Last.Value.IsOnScreen())
			{
				mg_jr_Level value = m_activeLevels.Last.Value;
				m_activeLevels.RemoveLast();
				if (value != null)
				{
					value.DestroyLevel();
				}
			}
		}

		private void RemoveExpiredLevels()
		{
			while (m_activeLevels.First != null && m_activeLevels.First.Value.IsSafelyOffLeftOfScreen())
			{
				mg_jr_Level value = m_activeLevels.First.Value;
				m_activeLevels.RemoveFirst();
				if (value != null)
				{
					value.DestroyLevel();
				}
			}
		}

		public void RemoveAllLevels()
		{
			while (m_activeLevels.First != null)
			{
				mg_jr_Level value = m_activeLevels.First.Value;
				m_activeLevels.RemoveFirst();
				if (value != null)
				{
					value.DestroyLevel();
				}
			}
		}

		public void EnableEnvironmentOverride(EnvironmentType _type)
		{
			Assert.IsFalse(_type == EnvironmentType.MAX, "Not a valid value: " + _type);
			RemoveNonVisibleLevels();
			m_environmentOverride = _type;
			m_isEnvironmentOverrideEnabled = true;
		}

		public void DisableEnvironmentOverride()
		{
			RemoveNonVisibleLevels();
			m_isEnvironmentOverrideEnabled = false;
		}
	}
}
