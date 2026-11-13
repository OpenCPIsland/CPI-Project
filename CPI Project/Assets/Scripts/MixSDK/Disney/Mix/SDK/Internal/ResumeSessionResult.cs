namespace Disney.Mix.SDK.Internal
{
	public class ResumeSessionResult : IResumeSessionResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public ResumeSessionResult(bool success)
		{
			Success = success;
		}
	}
}
