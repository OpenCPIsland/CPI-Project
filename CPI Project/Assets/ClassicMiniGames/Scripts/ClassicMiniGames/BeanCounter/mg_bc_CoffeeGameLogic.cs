namespace BeanCounter
{
	public class mg_bc_CoffeeGameLogic : mg_bc_GameLogic
	{
		public override void Awake()
		{
			base.ScoreController = new mg_bc_NormalScoreController();
			base.Awake();
		}
	}
}
