namespace Disney.Mix.SDK.Internal
{
	public class DisplayName : IDisplayName
	{
		public string Text
		{
			get;
			private set;
		}

		public DisplayName(string text)
		{
			Text = text;
		}
	}
}
