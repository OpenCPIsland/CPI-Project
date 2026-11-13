namespace Disney.Mix.SDK
{
	public interface IVerifyAdultFailedQuestionsResult : IVerifyAdultResult
	{
		IVerifyAdultQuiz Quiz
		{
			get;
		}
	}
}
