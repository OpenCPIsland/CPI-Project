using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Protobot : mg_jr_Boss
	{
		private const float HIGH_POSITION_Y = 2.71f;

		private const float LOW_POSITION_Y = -0.2f;

		private const float START_DISTANCE = 10.24f;

		private const float SPEED = 6.24f;

		private bool m_isBattleInProgress = false;

		private bool m_hasAppearanceSoundPlayed = false;

		private bool m_isMovingLeft = true;

		private bool m_isPositionHigh;

		private Animator m_animator;

		private SpriteRenderer m_thrusterRenderer;

		private SpriteRenderer m_clawRenderer;

		private SpriteRenderer m_protobotRenderer;

		private OnBossComplete m_bossCompleteCallback;

		protected override void Awake()
		{
			base.Awake();
			Transform transform = base.transform.Find("boost");
			m_thrusterRenderer = transform.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_thrusterRenderer, "Thruster renderer not found");
			Transform transform2 = base.transform.Find("claw");
			m_clawRenderer = transform2.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_clawRenderer, "Claw renderer not found");
			Transform transform3 = base.transform.Find("mg_jr_protobot_sprite");
			m_protobotRenderer = transform3.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_protobotRenderer, "Protobot renderer not found");
			m_animator = GetComponent<Animator>();
			Assert.NotNull(m_animator, "Protoboss Animator not found");
			m_warning.RendererToWarnAbout = m_protobotRenderer;
		}

		private void Update()
		{
			if (MinigameManager.IsPaused || !m_miniGame.GameLogic.IsGameInProgress || !m_isBattleInProgress)
			{
				return;
			}
			Vector3 position;
			if (m_isMovingLeft)
			{
				position = base.transform.position;
				position.x -= 6.24f * Time.deltaTime;
				base.transform.position = position;
				if (!m_hasAppearanceSoundPlayed && position.x < m_miniGame.VisibleWorldBounds.max.x)
				{
					m_miniGame.PlaySFX(mg_jr_Sound.BOSS_PROTOBOT_RL.ClipName());
					m_hasAppearanceSoundPlayed = true;
				}
				if (position.x < m_miniGame.VisibleWorldBounds.min.x && !m_miniGame.VisibleWorldBounds.Intersects(m_thrusterRenderer.bounds))
				{
					StartPass(false);
				}
				return;
			}
			position = base.transform.position;
			position.x += 6.24f * Time.deltaTime;
			base.transform.position = position;
			if (!m_hasAppearanceSoundPlayed && position.x > m_miniGame.VisibleWorldBounds.min.x)
			{
				m_miniGame.PlaySFX(mg_jr_Sound.BOSS_PROTOBOT_LR.ClipName());
				m_hasAppearanceSoundPlayed = true;
			}
			if (position.x > m_miniGame.VisibleWorldBounds.max.x && !m_miniGame.VisibleWorldBounds.Intersects(m_thrusterRenderer.bounds))
			{
				if (m_bossCompleteCallback != null)
				{
					m_bossCompleteCallback();
				}
				m_isBattleInProgress = false;
				m_miniGame.Resources.ReturnPooledResource(base.gameObject);
			}
		}

		public override void StartBossBattle(OnBossComplete _completionCallback)
		{
			m_isBattleInProgress = true;
			StartPass();
			m_bossCompleteCallback = _completionCallback;
		}

		private void StartPass(bool _fromRight = true)
		{
			m_hasAppearanceSoundPlayed = false;
			m_isMovingLeft = _fromRight;
			m_isPositionHigh = (Random.value > 0.5f);
			Vector3 zero = Vector3.zero;
			if (m_isPositionHigh)
			{
				zero.y = 2.71f;
			}
			else
			{
				zero.y = -0.2f;
			}
			if (m_isMovingLeft)
			{
				zero.x = m_miniGame.VisibleWorldBounds.max.x + 10.24f;
				Vector3 localScale = new Vector3(1f, 1f, 1f);
				base.transform.localScale = localScale;
			}
			else
			{
				zero.x = m_miniGame.VisibleWorldBounds.min.x - 10.24f;
				Vector3 localScale2 = new Vector3(-1f, 1f, 1f);
				base.transform.localScale = localScale2;
			}
			base.transform.position = zero;
			if (!_fromRight)
			{
				m_warning.ActivateWarning(3, false);
			}
			else
			{
				m_warning.ActivateWarning(3);
			}
			m_miniGame.PlaySFX(mg_jr_Sound.BOSS_ALERT.ClipName());
		}
	}
}
