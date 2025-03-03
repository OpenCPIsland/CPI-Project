namespace Disney.Kelowna.Common
{
	public class CompositeCoroutineReturn : CoroutineReturn
	{
		protected CoroutineReturn[] coroutineReturns;

		public override bool Finished
		{
			get
			{
				for (int i = 0; i < coroutineReturns.Length; i++)
				{
					if (!coroutineReturns[i].Finished)
					{
						return false;
					}
				}
				return true;
			}
		}

		public override bool Cancelled
		{
			get
			{
				if (base.Cancelled)
				{
					return true;
				}
				for (int i = 0; i < coroutineReturns.Length; i++)
				{
					if (coroutineReturns[i].Cancelled)
					{
						return true;
					}
				}
				return false;
			}
		}

		public CompositeCoroutineReturn(params CoroutineReturn[] coroutineReturns)
		{
			this.coroutineReturns = coroutineReturns;
		}
	}
}
