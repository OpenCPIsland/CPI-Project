using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class VerifyAdultQuizAnswersSender
	{
		public static void AnswerQuiz(AbstractLogger logger, IGuestControllerClient guestControllerClient, IVerifyAdultQuizAnswers answers, Action<IVerifyAdultResult> callback)
		{
			if (string.IsNullOrEmpty(answers.Id) || answers.Answers == null || answers.Answers.Count == 0)
			{
				callback(new VerifyAdultFailedMissingInfoResult());
			}
			else if (!ValidateAnswers(answers))
			{
				callback(new VerifyAdultFailedInvalidDataResult());
			}
			else
			{
				try
				{
					AdultVerificationQuizRequest adultVerificationQuizRequest = new AdultVerificationQuizRequest();
					adultVerificationQuizRequest.applicationId = answers.Id;
					adultVerificationQuizRequest.answers = answers.Answers.Select((KeyValuePair<IVerifyAdultQuestion, string> keyValue) => new AdultVerificationQuizAnswer
					{
						choice = keyValue.Value,
						questionId = keyValue.Key.QuestionId
					}).ToList();
					AdultVerificationQuizRequest request = adultVerificationQuizRequest;
					guestControllerClient.SendAdultVerificationQuiz(request, delegate(GuestControllerResult<AdultVerificationQuizResponse> r)
					{
						if (!r.Success)
						{
							callback(MakeGenericFailure());
						}
						else if (r.Response.error != null)
						{
							callback(ParseError(r.Response));
						}
						else if (r.Response.data == null)
						{
							callback(MakeGenericFailure());
						}
						else
						{
							callback(new VerifyAdultResult(r.Response.data.verified, r.Response.data.maxAttempts));
						}
					});
				}
				catch (Exception arg)
				{
					logger.Critical("Unhandled exception: " + arg);
					callback(MakeGenericFailure());
				}
			}
		}

		private static bool ValidateAnswers(IVerifyAdultQuizAnswers answers)
		{
			int num = answers.Answers.Count((KeyValuePair<IVerifyAdultQuestion, string> pair) => pair.Key.Choices.Any((string choice) => choice == pair.Value));
			return num == answers.Answers.Count;
		}

		private static IVerifyAdultResult MakeGenericFailure()
		{
			return new VerifyAdultResult(false, false);
		}

		private static IVerifyAdultResult ParseError(GuestControllerWebCallResponse response)
		{
			IVerifyAdultResult verifyAdultResult = GuestControllerErrorParser.GetVerifyAdultResult(response.error);
			return verifyAdultResult ?? MakeGenericFailure();
		}
	}
}
