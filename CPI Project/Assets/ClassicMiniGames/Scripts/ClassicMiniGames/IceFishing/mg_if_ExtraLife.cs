using UnityEngine;

namespace IceFishing
{
	public class mg_if_ExtraLife : mg_if_ObstacleHook
	{
		private Animator m_animator;

		private float m_fadeTimeRemaining;

		private bool m_lifeGiven;

		private mg_if_XMovement m_XMovement;

		private mg_if_YMovement m_YMovement;

		private mg_if_BobMovement m_BobMovement;

		protected override void Awake()
		{
			base.Awake();
			m_XMovement = GetComponent<mg_if_XMovement>();
			m_YMovement = GetComponent<mg_if_YMovement>();
			m_BobMovement = new mg_if_BobMovement(m_variables.ExtraLifeBobTimeRate, m_variables.ExtraLifeBobAmplitude, m_variables.ExtraLifeBobTimeOffset);
			m_animator = GetComponentInChildren<Animator>();
		}

		public override void Spawn()
		{
			base.Spawn();
			m_XMovement.Initialize(mg_if_EObjectsMovement.MOVEMENT_AUTO, m_variables.ExtraLifeSpeed, m_variables.ExtraLifeSpeedRange);
			m_XMovement.SetInitialPos();
			m_YMovement.Initialize(0f, 0f);
			m_YMovement.SetInitialPos();
			m_BobMovement.Base = base.transform.position.y;
			m_lifeGiven = false;
			m_animator.enabled = true;
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			m_XMovement.MinigameUpdate(p_deltaTime);
			Vector2 v = base.transform.position;
			v.y = m_BobMovement.GetValue(p_deltaTime);
			base.transform.position = v;
			if (m_lifeGiven)
			{
				m_fadeTimeRemaining -= p_deltaTime;
				float p_alpha = m_fadeTimeRemaining / m_variables.ExtraLifeFadeTime;
				base.UpdateAlpha(p_alpha);
			}
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
			}
		}

		public override void OnObstacleHitHook(mg_if_FishingRod p_rod)
		{
			if (!m_lifeGiven)
			{
				p_rod.GainLife();
				m_fadeTimeRemaining = m_variables.ExtraLifeFadeTime;
				m_lifeGiven = true;
				m_animator.enabled = false;
			}
		}

		public override void UpdateAlpha(float p_alpha)
		{
			if (!m_lifeGiven)
			{
				base.UpdateAlpha(p_alpha);
			}
		}
	}
}
