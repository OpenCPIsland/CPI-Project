namespace Disney.Mix.SDK
{
	public interface ITextModerationResult
	{
		bool Success
		{
			get;
		}

		bool IsModerated
		{
			get;
		}

		string ModeratedText
		{
			get;
		}
	}
}
