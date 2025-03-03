namespace Disney.Mix.SDK.Internal
{
	internal class TextModerationResult : ITextModerationResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public bool IsModerated
		{
			get;
			private set;
		}

		public string ModeratedText
		{
			get;
			private set;
		}

		public TextModerationResult(bool success, bool isModerated, string moderatedText)
		{
			Success = success;
			IsModerated = isModerated;
			ModeratedText = moderatedText;
		}
	}
}
