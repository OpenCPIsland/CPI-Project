namespace Disney.Mix.SDK.Internal
{
	public class LegalDocument : ILegalDocument
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

		public string Type
		{
			get;
			private set;
		}

		public bool DisplayCheckbox
		{
			get;
			private set;
		}

		public LegalDocument(string id, string text, string type, bool displayCheckbox)
		{
			Id = id;
			Text = text;
			Type = type;
			DisplayCheckbox = displayCheckbox;
		}
	}
}
