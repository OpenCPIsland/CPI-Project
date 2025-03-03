namespace SmoothieSmash
{
	public class mg_ss_GameLogic_Normal : mg_ss_GameLogic
	{
		public float GameTimer
		{
			get;
			private set;
		}

		public override void Initialize(mg_ss_GameScreen p_screen)
		{
			GameTimer = 90f;
			base.Scoring = new mg_ss_ScoreNormal(this);
			base.Initialize(p_screen);
		}

		protected override void UpdateGameTime(float p_deltaTime)
		{
			base.UpdateGameTime(p_deltaTime);
			if (m_gameStarted)
			{
				GameTimer -= p_deltaTime;
			}
		}

		protected override bool CheckForGameOver()
		{
			return GameTimer <= 0f;
		}

		public override void OnClockCollision(mg_ss_Item_ClockObject p_clock)
		{
			GameTimer += 10f;
		}

		public override void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			base.OnFruitCollision(p_fruit);
		}

		protected override void OnChaosItemCollision()
		{
			GameTimer += 0.5f;
		}
	}
}
