using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Herbert : mg_jr_Boss
	{
		private enum HerbertState
		{
			MOVING,
			WAITING,
			LEAVING,
			MAX
		}

		private const float TOP_Y_POSITION = 6.2f;

		private const float MIDDLE_Y_POSITION = 4.2f;

		private const float BOTTOM_Y_POSITION = 2.2f;

		private const float X_POSITION = 1f;

		private const float START_DISTANCE = 10.24f;

		private const float WAIT_TIME = 1f;

		private const float SPEED = 4.4f;

		private const int SHOTS = 3;

		private int m_shotsFired = 0;

		private HerbertState m_currentState = HerbertState.MOVING;

		private float m_timeWaiting = 0f;

		private bool m_isBattleInProgress = false;

		private bool m_hasHerbertBeenVisible = false;

		private List<Vector3> m_firingPositions = new List<Vector3>();

		private Vector3 m_exitTargetPosition;

		private Vector3 m_targetPosition;

		private int m_targetPositionIndex = -1;

		private Animator m_animator;

		private SpriteRenderer m_armRenderer;

		private SpriteRenderer m_herbertRenderer;

		private Transform m_snowballSpawnSocket;

		private OnBossComplete m_bossCompleteCallback;

		protected override void Awake()
		{
			base.Awake();
			Transform transform = base.transform.Find("arm");
			m_armRenderer = transform.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_armRenderer, "Arm not found");
			m_herbertRenderer = GetComponent<SpriteRenderer>();
			Assert.NotNull(m_herbertRenderer, "Herbert renderer not found");
			m_snowballSpawnSocket = base.transform.Find("gun/mg_jr_snowball_socket");
			Assert.NotNull(m_snowballSpawnSocket, "Snowball socket not found");
			m_animator = GetComponent<Animator>();
			Assert.NotNull(m_animator, "Herbert Animator not found");
			m_firingPositions.Add(new Vector3(1f, 6.2f, 0f));
			m_firingPositions.Add(new Vector3(1f, 4.2f, 0f));
			m_firingPositions.Add(new Vector3(1f, 2.2f, 0f));
			float y = m_miniGame.VisibleWorldBounds.size.y;
			float y2 = m_miniGame.VisibleWorldBounds.max.y;
			float x = m_miniGame.VisibleWorldBounds.min.x;
			m_exitTargetPosition = new Vector3(x, y2 + 0.75f * y, 0f);
		}

		private void Update()
		{
			if (m_miniGame.IsPaused || !m_miniGame.GameLogic.IsGameInProgress || !m_isBattleInProgress)
			{
				return;
			}
			switch (m_currentState)
			{
			case HerbertState.MOVING:
			{
				float maxDistanceDelta = 4.4f * Time.deltaTime;
				base.transform.position = Vector3.MoveTowards(base.transform.position, m_targetPosition, maxDistanceDelta);
				if (base.transform.position == m_targetPosition)
				{
					Shoot();
					if (Random.value < 0.5f)
					{
						m_miniGame.PlaySFX(mg_jr_Sound.BOSS_HERBERT_LAUGH.ClipName());
					}
					m_currentState = HerbertState.WAITING;
				}
				break;
			}
			case HerbertState.WAITING:
				m_timeWaiting += Time.deltaTime;
				if (m_timeWaiting > 1f)
				{
					m_timeWaiting = 0f;
					if (m_shotsFired >= 3)
					{
						m_currentState = HerbertState.LEAVING;
						break;
					}
					ChangeToDifferentTargetPosition();
					m_currentState = HerbertState.MOVING;
				}
				break;
			case HerbertState.LEAVING:
			{
				float maxDistanceDelta = 4.4f * Time.deltaTime;
				base.transform.position = Vector3.MoveTowards(base.transform.position, m_exitTargetPosition, maxDistanceDelta);
				if (base.transform.position == m_exitTargetPosition)
				{
					if (m_bossCompleteCallback != null)
					{
						m_bossCompleteCallback();
					}
					m_miniGame.StopSFX(mg_jr_Sound.BOSS_HERBERT_FLY_LOOP.ClipName());
					m_isBattleInProgress = false;
					m_miniGame.Resources.ReturnPooledResource(base.gameObject);
				}
				break;
			}
			default:
				Assert.IsTrue(false, "Unhandled or invalid state");
				break;
			}
			if (!m_hasHerbertBeenVisible && base.transform.position.x < m_miniGame.VisibleWorldBounds.max.x)
			{
				m_warning.DeactivateWarning();
				m_miniGame.PlaySFX(mg_jr_Sound.BOSS_HERBERT_FLY_LOOP.ClipName());
				m_hasHerbertBeenVisible = true;
			}
		}

		public override void StartBossBattle(OnBossComplete _completionCallback)
		{
			m_isBattleInProgress = true;
			m_bossCompleteCallback = _completionCallback;
			ChangeToDifferentTargetPosition();
			Vector3 targetPosition = m_targetPosition;
			targetPosition.x = m_miniGame.VisibleWorldBounds.max.x + 10.24f;
			base.transform.position = targetPosition;
			m_warning.RendererToWarnAbout = m_armRenderer;
			m_warning.ActivateWarning(3);
			m_miniGame.PlaySFX(mg_jr_Sound.BOSS_ALERT.ClipName());
			m_currentState = HerbertState.MOVING;
		}

		private void ChangeToDifferentTargetPosition()
		{
			int num = Random.Range(0, m_firingPositions.Count);
			if (m_targetPositionIndex == num)
			{
				if (Random.value > 0.5f)
				{
					num++;
					if (num == m_firingPositions.Count)
					{
						num = 0;
					}
				}
				else
				{
					num--;
					if (num == -1)
					{
						num = m_firingPositions.Count - 1;
					}
				}
			}
			m_targetPositionIndex = num;
			m_targetPosition = m_firingPositions[m_targetPositionIndex];
		}

		private void OnDisable()
		{
			m_miniGame.StopSFX(mg_jr_Sound.BOSS_HERBERT_FLY_LOOP.ClipName());
		}

		private void Shoot()
		{
			m_animator.SetTrigger("Shoot");
			Assert.IsTrue(m_shotsFired < 3, "Trying to shoot more than " + 3 + " shots");
			m_shotsFired++;
			GameObject pooledResource = m_miniGame.Resources.GetPooledResource(mg_jr_ResourceList.BOSS_HERBERT_SNOWBALL);
			pooledResource.transform.position = m_snowballSpawnSocket.transform.position;
			pooledResource.transform.localRotation = m_snowballSpawnSocket.transform.localRotation;
			pooledResource.transform.parent = m_miniGame.GameLogic.transform;
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.BOSS_HERBERT_FIRE.ClipName());
		}
	}
}
