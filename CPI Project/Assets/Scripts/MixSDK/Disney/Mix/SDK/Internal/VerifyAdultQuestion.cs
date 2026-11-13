using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class VerifyAdultQuestion : IVerifyAdultQuestion
	{
		public int QuestionId
		{
			get;
			private set;
		}

		public string QuestionText
		{
			get;
			private set;
		}

		public IEnumerable<string> Choices
		{
			get;
			private set;
		}

		public VerifyAdultQuestion(int questionId, string questionText, IEnumerable<string> choices)
		{
			QuestionId = questionId;
			QuestionText = questionText;
			Choices = choices;
		}
	}
}
