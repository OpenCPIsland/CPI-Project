namespace IceFishing
{
	public class mg_if_JellyFish : mg_if_ObstacleLine
	{
		private mg_if_XMovement m_XMovement;

		private mg_if_YMovement m_YMovement;

		public void Initialize(mg_if_EObjectsMovement p_movement)
		{
			m_XMovement = base.gameObject.GetComponent<mg_if_XMovement>();
			m_XMovement.Initialize(p_movement, m_variables.JellyfishSpeed, m_variables.JellyfishSpeedRange);
			m_YMovement = base.gameObject.GetComponent<mg_if_YMovement>();
			m_YMovement.Initialize(0f, 0f);
		}

		public override void Spawn()
		{
			base.Spawn();
			m_XMovement.SetInitialPos();
			m_YMovement.SetInitialPos();
		}

		public override void MinigameUpdate(float p_deltaTime)
		{
			base.MinigameUpdate(p_deltaTime);
			m_XMovement.MinigameUpdate(p_deltaTime);
			m_YMovement.MinigameUpdate(p_deltaTime);
			if (m_XMovement.CheckOffEdge())
			{
				Despawn();
			}
		}

		public override void OnObstacleHitHook(mg_if_FishingRod p_rod)
		{
			ShockRod(p_rod);
		}

		public override void OnObstacleHitLine(mg_if_FishingRod p_rod)
		{
			ShockRod(p_rod);
		}

		private void ShockRod(mg_if_FishingRod p_rod)
		{
			p_rod.ShockRod();
		}
	}
}
