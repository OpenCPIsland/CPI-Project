namespace Disney.Mix.SDK.Internal
{
	public class SetLangaugePreferenceResult : ISetLangaugePreferenceResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SetLangaugePreferenceResult(bool success)
		{
			Success = success;
		}
	}
}
