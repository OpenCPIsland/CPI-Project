using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IVerifyAdultQuiz
	{
		IEnumerable<IVerifyAdultQuestion> Questions
		{
			get;
		}

		string Id
		{
			get;
		}
	}
}
