namespace Pizzatron
{
	public class mg_pt_OrderComponent
	{
		public int RequiredCount
		{
			get;
			set;
		}

		public int CurrentCount
		{
			get;
			set;
		}

		public int CoinsEarnt
		{
			get;
			set;
		}

		public mg_pt_EToppingType Topping
		{
			get;
			private set;
		}

		public bool Completed
		{
			get
			{
				return CurrentCount >= RequiredCount;
			}
		}

		public bool Failed
		{
			get
			{
				return RequiredCount == 0 && CurrentCount > 0;
			}
		}

		public mg_pt_OrderComponent(mg_pt_EToppingType p_topping)
		{
			Topping = p_topping;
		}

		public void Reset()
		{
			CurrentCount = 0;
			CoinsEarnt = 0;
		}
	}
}
