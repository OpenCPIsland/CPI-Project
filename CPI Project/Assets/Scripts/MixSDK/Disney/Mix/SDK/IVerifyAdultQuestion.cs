using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IVerifyAdultQuestion
	{
		int QuestionId
		{
			get;
		}

		string QuestionText
		{
			get;
		}

		IEnumerable<string> Choices
		{
			get;
		}
	}
}
