namespace Disney.Mix.SDK
{
	public interface IMarketingItem
	{
		string Id
		{
			get;
		}

		string Text
		{
			get;
		}

		MarketingType Type
		{
			get;
		}

		bool Checked
		{
			get;
		}
	}
}
