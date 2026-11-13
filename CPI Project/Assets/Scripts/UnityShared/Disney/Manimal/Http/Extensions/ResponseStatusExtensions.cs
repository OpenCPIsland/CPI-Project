using System;
using System.Net;

namespace Disney.Manimal.Http.Extensions
{
	public static class ResponseStatusExtensions
	{
		public static WebException ToWebException(this ResponseStatus responseStatus)
		{
			switch (responseStatus)
			{
			case ResponseStatus.None:
				return new WebException("The request could not be processed.", WebExceptionStatus.UnknownError);
			case ResponseStatus.Error:
				return new WebException("An error occurred while processing the request.", WebExceptionStatus.ServerProtocolViolation);
			case ResponseStatus.TimedOut:
				return new WebException("The request timed-out.", WebExceptionStatus.Timeout);
			case ResponseStatus.Aborted:
				return new WebException("The request was aborted.", WebExceptionStatus.RequestCanceled);
			default:
				throw new ArgumentOutOfRangeException("responseStatus");
			}
		}
	}
}
