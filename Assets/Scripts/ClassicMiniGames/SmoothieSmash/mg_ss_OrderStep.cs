namespace SmoothieSmash
{
	public class mg_ss_OrderStep
	{
		public mg_ss_EItemTypes FruitType
		{
			get;
			set;
		}

		public mg_ss_EOrderStepState State
		{
			get;
			set;
		}

		public mg_ss_OrderStep()
		{
			State = mg_ss_EOrderStepState.INVALID;
		}
	}
}
