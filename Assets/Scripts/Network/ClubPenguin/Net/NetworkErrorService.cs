using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.http;
using System;

namespace ClubPenguin.Net
{
	internal class NetworkErrorService
	{
		public delegate bool ErrorMapper<T>(NetworkErrorType errorType, T handler) where T : IBaseNetworkErrorHandler;

		public static void OnError<T>(HttpResponse response, T handler, ErrorMapper<T> errorMapper = null) where T : IBaseNetworkErrorHandler
		{
			if (response.Request.CompletionState == HttpRequestState.TIMEOUT)
			{
				handler.onRequestTimeOut();
			}
			else if (response.StatusCode == HttpStatusCode.Unknown || string.IsNullOrEmpty(response.Text))
			{
				handler.onGeneralNetworkError();
			}
			else
			{
				try
				{
					ErrorResponse errorResponse = Service.Get<JsonService>().Deserialize<ErrorResponse>(response.Text);
					NetworkErrorType code = (NetworkErrorType)errorResponse.code;
					bool flag = false;
					NetworkErrorType networkErrorType = code;
					if (networkErrorType == NetworkErrorType.INVALID_SUBSCRIPTION)
					{
						Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.InvalidSubscriptionError));
						flag = true;
					}
					if ((errorMapper == null || !errorMapper(code, handler)) && !flag)
					{
						handler.onGeneralNetworkError();
					}
				}
				catch (Exception)
				{
					handler.onGeneralNetworkError();
				}
			}
		}

		public static void dispatchErrorEvent(HttpResponse response)
		{
			if (!response.Is2XX)
			{
				ErrorResponse errorResponse = new ErrorResponse();
				if (response.Request.CompletionState == HttpRequestState.TIMEOUT)
				{
					errorResponse.code = 10001;
				}
				else if (string.IsNullOrEmpty(response.Text))
				{
					errorResponse.code = 10000;
				}
				else
				{
					try
					{
						errorResponse = Service.Get<JsonService>().Deserialize<ErrorResponse>(response.Text);
					}
					catch (Exception)
					{
						errorResponse.code = 10002;
					}
				}
				dispatchErrorEventFromError(errorResponse);
			}
		}

		private static void dispatchErrorEventFromError(ErrorResponse error)
		{
			switch (error.code)
			{
			case 5:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.SecurityAccessError));
				break;
			case 714:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.PlayerNotFoundError));
				break;
			case 952:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.InvalidSubscriptionError));
				break;
			case 707:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.NotEnoughResourcesError));
				break;
			case 1:
				Service.Get<EventDispatcher>().DispatchEvent(new NetworkErrors.InputBadRequestError(error));
				break;
			case 6:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.GeneralResourceNotFoundError));
				break;
			case 13:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.GeneralResourceNoLongerAvailableError));
				break;
			case 999:
				Service.Get<EventDispatcher>().DispatchEvent(default(NetworkErrors.SystemInternalErrorError));
				break;
			default:
				Service.Get<EventDispatcher>().DispatchEvent(new NetworkErrors.GeneralError(error));
				break;
			}
		}
	}
}
