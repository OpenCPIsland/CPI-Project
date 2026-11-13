using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class DisplayNameSearcher
	{
		public static void Search(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, string displayName, IUserDatabase userDatabase, Action<IInternalUnidentifiedUser> successCallback, Action failureCallback)
		{
			try
			{
				DisplayNameSearchRequest displayNameSearchRequest = new DisplayNameSearchRequest();
				displayNameSearchRequest.DisplayName = displayName;
				DisplayNameSearchRequest request = displayNameSearchRequest;
				IWebCall<DisplayNameSearchRequest, DisplayNameSearchResponse> webCall = mixWebCallFactory.SearchDisplaynamePost(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<DisplayNameSearchResponse> e)
				{
					DisplayNameSearchResponse response = e.Response;
					if (ValidateResponse(response))
					{
						userDatabase.InsertUserDocument(new UserDocument
						{
							DisplayName = response.DisplayName,
							FirstName = response.FirstName,
							Swid = null,
							HashedSwid = null
						});
						IInternalUnidentifiedUser obj = RemoteUserFactory.CreateUnidentifiedUser(response.DisplayName, response.FirstName, userDatabase);
						successCallback(obj);
					}
					else
					{
						logger.Critical("Failed to validate display name search response: " + JsonParser.ToJson(response));
						failureCallback();
					}
				};
				webCall.OnError += delegate(object sender, WebCallErrorEventArgs e)
				{
					logger.Debug("Failed to find user: " + e.Message);
					failureCallback();
				};
				webCall.Execute();
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				failureCallback();
			}
		}

		private static bool ValidateResponse(DisplayNameSearchResponse response)
		{
			return response.DisplayName != null;
		}
	}
}
