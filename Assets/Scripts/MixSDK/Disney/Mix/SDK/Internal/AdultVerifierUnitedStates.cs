using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class AdultVerifierUnitedStates
	{
		public static void VerifyAdult(AbstractLogger logger, IGuestControllerClient guestControllerClient, IVerifyAdultFormUnitedStates form, Action<IVerifyAdultResult> callback)
		{
			try
			{
				AdultVerificationRequest adultVerificationRequest = new AdultVerificationRequest();
				adultVerificationRequest.address = new AdultVerificationAddress
				{
					addressLine1 = form.AddressLine1,
					postalCode = form.PostalCode
				};
				adultVerificationRequest.dateOfBirth = form.DateOfBirth.ToString("yyyy-MM-dd");
				adultVerificationRequest.firstName = form.FirstName;
				adultVerificationRequest.lastName = form.LastName;
				adultVerificationRequest.ssn = form.Ssn;
				AdultVerificationRequest request = adultVerificationRequest;
				guestControllerClient.VerifyAdultUnitedStates(request, delegate(GuestControllerResult<AdultVerificationResponse> r)
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

		private static IVerifyAdultResult MakeGenericFailure()
		{
			return new VerifyAdultResult(false, false);
		}

		private static IVerifyAdultResult ParseError(AdultVerificationResponse response)
		{
			IVerifyAdultResult verifyAdultResult = GuestControllerErrorParser.GetVerifyAdultResult(response.error);
			if (verifyAdultResult == null)
			{
				return MakeGenericFailure();
			}
			if (verifyAdultResult is IVerifyAdultFailedQuestionsResult && response.data != null && response.data.questions != null)
			{
				VerifyAdultQuestion[] questions = response.data.questions.Select((AdultVerificationQuestion q) => new VerifyAdultQuestion(q.questionId, q.questionText, q.choices)).ToArray();
				VerifyAdultQuiz quiz = new VerifyAdultQuiz(questions, response.data.applicationId);
				verifyAdultResult = new VerifyAdultFailedQuestionsResult(quiz);
			}
			return verifyAdultResult;
		}
	}
}
