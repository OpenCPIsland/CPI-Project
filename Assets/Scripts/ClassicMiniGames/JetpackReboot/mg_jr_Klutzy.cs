using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Klutzy : mg_jr_Boss
	{
		private enum FlightPath
		{
			TOP,
			MIDDLE,
			BOTTOM,
			MAX
		}

		private enum MovementPhase
		{
			APPROACH,
			WAITING,
			NORMAL,
			MAX
		}

		private const float TOP_Y_POSITION = 3f;

		private const float BOTTOM_Y_POSITION = -1f;

		private const float MIDDLE_Y_POSITION = 1f;

		private const float WAIT_TIME = 0.5f;

		private const float SPEED_X = 3.8f;

		private const float SPEED_Y = 1.8f;

		private const float START_DISTANCE = 10.24f;

		private FlightPath m_targetFlightPath = FlightPath.MIDDLE;

		private MovementPhase m_currentMovementPhase = MovementPhase.APPROACH;

		private float m_timeWaiting = 0f;

		private bool m_isBattleInProgress = false;

		private bool m_hasAppearanceSoundPlayed = false;

		private SpriteRenderer m_klutzySprite;

		private SpriteRenderer m_reactorRingsRight;

		private float m_currentVerticalMovementDirection = 0f;

		private OnBossComplete m_bossCompleteCallback;

		protected override void Awake()
		{
			base.Awake();
			m_klutzySprite = GetComponent<SpriteRenderer>();
			Assert.NotNull(m_klutzySprite, "Klutzy renderer not found");
			Transform transform = base.transform.Find("reactorring_R");
			m_reactorRingsRight = transform.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_reactorRingsRight, "Right reactor rings not found");
		}

		private void Update()
		{
			if (MinigameManager.IsPaused || !m_miniGame.GameLogic.IsGameInProgress || !m_isBattleInProgress)
			{
				return;
			}
			Vector3 position;
			switch (m_currentMovementPhase)
			{
			case MovementPhase.APPROACH:
				position = base.transform.position;
				position.x -= 3.8f * Time.deltaTime;
				base.transform.position = position;
				if (!m_hasAppearanceSoundPlayed && position.x < m_miniGame.VisibleWorldBounds.max.x)
				{
					m_miniGame.PlaySFX(mg_jr_Sound.BOSS_KLUTZY_LOOP.ClipName());
					m_hasAppearanceSoundPlayed = true;
				}
				if (m_klutzySprite.bounds.max.x < m_miniGame.VisibleWorldBounds.max.x)
				{
					int num = (int)(m_targetFlightPath = (FlightPath)Random.Range(0, 3));
					switch (m_targetFlightPath)
					{
					case FlightPath.TOP:
						m_currentVerticalMovementDirection = 1f;
						break;
					case FlightPath.MIDDLE:
						m_currentVerticalMovementDirection = 0f;
						break;
					case FlightPath.BOTTOM:
						m_currentVerticalMovementDirection = -1f;
						break;
					default:
						Assert.IsTrue(false, "Invalid flight path");
						break;
					}
					m_currentMovementPhase = MovementPhase.WAITING;
				}
				break;
			case MovementPhase.WAITING:
				m_timeWaiting += Time.deltaTime;
				if (m_timeWaiting > 0.5f && Random.value > 0.5f)
				{
					m_miniGame.PlaySFX(mg_jr_Sound.BOSS_KLUTZY_LAUGH.ClipName());
					m_currentMovementPhase = MovementPhase.NORMAL;
				}
				break;
			case MovementPhase.NORMAL:
				position = base.transform.position;
				position.x -= 3.8f * Time.deltaTime;
				position.y += 1.8f * Time.deltaTime * m_currentVerticalMovementDirection;
				if (position.y > 3f)
				{
					position.y = 3f;
					m_currentVerticalMovementDirection = -1f;
					m_currentMovementPhase = MovementPhase.WAITING;
				}
				if (position.y < -1f)
				{
					position.y = -1f;
					m_currentVerticalMovementDirection = 1f;
					m_currentMovementPhase = MovementPhase.WAITING;
				}
				base.transform.position = position;
				if (position.x < m_miniGame.VisibleWorldBounds.min.x && !m_miniGame.VisibleWorldBounds.Intersects(m_reactorRingsRight.bounds))
				{
					if (m_bossCompleteCallback != null)
					{
						m_bossCompleteCallback();
					}
					m_miniGame.StopSFX(mg_jr_Sound.BOSS_KLUTZY_LOOP.ClipName());
					m_isBattleInProgress = false;
					m_miniGame.Resources.ReturnPooledResource(base.gameObject);
				}
				break;
			default:
				Assert.IsTrue(false, "Unknown movement phase");
				break;
			}
		}

		public override void StartBossBattle(OnBossComplete _completionCallback)
		{
			m_isBattleInProgress = true;
			m_bossCompleteCallback = _completionCallback;
			Vector3 position = base.transform.position;
			position.x = m_miniGame.VisibleWorldBounds.max.x + 10.24f;
			position.y = 1f;
			base.transform.position = position;
			m_warning.RendererToWarnAbout = m_klutzySprite;
			m_warning.ActivateWarning(3);
			m_miniGame.PlaySFX(mg_jr_Sound.BOSS_ALERT.ClipName());
		}

		private void OnDisable()
		{
			m_miniGame.StopSFX(mg_jr_Sound.BOSS_KLUTZY_LOOP.ClipName());
		}
	}
}
