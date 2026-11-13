namespace Disney.Mix.SDK.Internal
{
	public class VerifyAdultFailedQuestionsResult : IVerifyAdultFailedQuestionsResult, IVerifyAdultResult
	{
		public IVerifyAdultQuiz Quiz
		{
			get;
			private set;
		}

		public bool Success
		{
			get
			{
				return false;
			}
		}

		public bool MaxAttempts
		{
			get
			{
				return false;
			}
		}

		public VerifyAdultFailedQuestionsResult()
		{
		}

		public VerifyAdultFailedQuestionsResult(IVerifyAdultQuiz quiz)
		{
			Quiz = quiz;
		}
	}
}
