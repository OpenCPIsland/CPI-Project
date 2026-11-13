namespace Disney.Mix.SDK.Internal
{
	public class MarketingItem : IMarketingItem
	{
		public string Id
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public MarketingType Type
		{
			get;
			private set;
		}

		public bool Checked
		{
			get;
			private set;
		}

		public MarketingItem(string id, string text, MarketingType type, bool isChecked)
		{
			Id = id;
			Text = text;
			Type = type;
			Checked = isChecked;
		}
	}
}
