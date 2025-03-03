namespace BeanCounter
{
	public class mg_bc_NormalScoreController : mg_bc_ScoreController
	{
		public override void OnBagCaught()
		{
			AddScore(m_truck.TruckNumber.Value * 2);
		}

		public override void OnBagUnloaded(bool _isCorrect = true)
		{
			AddScore(m_truck.TruckNumber.Value * 3);
		}

		public override void OnBagThrownBack(bool _isTargetBag)
		{
		}
	}
}
