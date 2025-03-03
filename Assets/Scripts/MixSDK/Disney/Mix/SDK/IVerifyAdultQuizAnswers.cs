using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IVerifyAdultQuizAnswers
	{
		string Id
		{
			get;
		}

		IDictionary<IVerifyAdultQuestion, string> Answers
		{
			get;
		}
	}
}
