namespace Disney.Mix.SDK
{
	public interface ILegalDocument
	{
		string Id
		{
			get;
		}

		string Text
		{
			get;
		}

		string Type
		{
			get;
		}

		bool DisplayCheckbox
		{
			get;
		}
	}
}
