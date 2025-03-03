using UnityEngine;

namespace IceFishing
{
	public class mg_if_Kicker : mg_if_ObstacleHook
	{
		private mg_if_XMovement m_XMovement;

		private mg_if_YMovement m_YMovement;

		private mg_if_BobMovement m_BobMovement;

		protected override void Awake()
		{
			base.Awake();
			m_BobMovement = new mg_if_BobMovement(m_variables.KickerBobTimeRate, m_variables.KickerBobAmplitude, m_variables.KickerBobTimeOffset);
		}

		public void Initialize(mg_if_EObjectsMovement p_movement)
		{
			m_XMovement = base.gameObject.GetComponent<mg_if_XMovement>();
			m_XMovement.Initialize(p_movement, m_variables.KickerSpeed, m_variables.KickerSpeedRange);
			m_YMovement = base.gameObject.GetComponent<mg_if_YMovement>();
			m_YMovement.Initialize(0f, 0f);
		}

		public override void Spawn()
		{
			base.Spawn();
			m_XMovement.SetInitialPos();
			m_YMovement.SetInitialPos();
			m_BobMovement.Base = base.transform.position.y;
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			m_XMovement.MinigameUpdate(p_deltaTime);
			Vector2 v = base.transform.position;
			v.y = m_BobMovement.GetValue(p_deltaTime);
			base.transform.position = v;
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
			}
		}

		public override void OnObstacleHitHook(mg_if_FishingRod p_rod)
		{
			p_rod.ReleaseFish();
		}
	}
}
