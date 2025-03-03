using System;
using System.Collections.Generic;
using System.Text;

namespace Disney.Mix.SDK.Internal
{
	public class AssetLoader : IAssetLoader
	{
		private enum WwwCallResult
		{
			Success,
			TooManyTimeoutsError,
			TooManyServerErrors,
			ClientError,
			OfflineError,
			RetryTimeout,
			RetryServerError
		}

		private const long DefaultLatencyWwwCallTimeout = 10000L;

		private const long DefaultMaxWwwCallTimeout = 30000L;

		private const int MaxTimeoutAttempts = 3;

		private const int MaxServerErrorAttempts = 1;

		private static readonly Dictionary<string, string> emptyHeaders = new Dictionary<string, string>();

		private readonly AbstractLogger logger;

		private readonly IWwwCallFactory wwwCallFactory;

		public AssetLoader(AbstractLogger logger, IWwwCallFactory wwwCallFactory)
		{
			this.logger = logger;
			this.wwwCallFactory = wwwCallFactory;
		}

		public void Load(string url, Action<LoadAssetResult> callback)
		{
			Uri uri = new Uri(url);
			StringBuilder timeoutLogs = new StringBuilder();
			Load(uri, callback, timeoutLogs, 1);
		}

		private void Load(Uri uri, Action<LoadAssetResult> callback, StringBuilder timeoutLogs, int numAttempts)
		{
			IWwwCall wwwCall = wwwCallFactory.Create(uri, HttpMethod.GET, null, emptyHeaders, 10000L, 30000L);
			logger.Debug(BuildRequestLog(uri, wwwCall.RequestId));
			EventHandler<WwwDoneEventArgs> handleOnDone = null;
			handleOnDone = delegate
			{
				wwwCall.OnDone -= handleOnDone;
				Dictionary<string, string> responseHeaders = wwwCall.ResponseHeaders;
				uint statusCode = wwwCall.StatusCode;
				WwwCallResult result = GetResult(wwwCall.Error, statusCode, numAttempts);
				string log = BuildResponseLog(uri, result, responseHeaders, statusCode, wwwCall);
				byte[] responseBody = wwwCall.ResponseBody;
				HandleResult(result, log, responseBody, uri, timeoutLogs, numAttempts, callback, Load, logger);
				wwwCall.Dispose();
			};
			wwwCall.OnDone += handleOnDone;
			wwwCall.Execute();
		}

		private static string BuildResponseLog(Uri uri, WwwCallResult result, Dictionary<string, string> headers, uint statusCode, IWwwCall wwwCall)
		{
			return (result == WwwCallResult.RetryTimeout) ? HttpLogBuilder.BuildTimeoutLog(wwwCall.RequestId, uri, HttpMethod.GET, headers, string.Empty, wwwCall.TimeToStartUpload, wwwCall.TimeToFinishUpload, wwwCall.PercentUploaded, wwwCall.TimeToStartDownload, wwwCall.TimeToFinishDownload, wwwCall.PercentDownloaded, wwwCall.TimeoutReason, wwwCall.TimeoutTime) : HttpLogBuilder.BuildResponseLog(wwwCall.RequestId, uri, HttpMethod.GET, headers, string.Empty, statusCode);
		}

		private static string BuildRequestLog(Uri uri, int requestId)
		{
			return HttpLogBuilder.BuildRequestLog(requestId, uri, HttpMethod.GET, emptyHeaders, string.Empty);
		}

		private static WwwCallResult GetResult(string error, uint statusCode, int numAttempts)
		{
			return (!IsTimeout(error)) ? ((!IsServerError(statusCode)) ? (IsClientError(statusCode) ? WwwCallResult.ClientError : ((!IsSuccess(statusCode)) ? (IsOfflineError(error) ? WwwCallResult.OfflineError : WwwCallResult.ClientError) : WwwCallResult.Success)) : ((numAttempts > 1) ? WwwCallResult.TooManyServerErrors : WwwCallResult.RetryServerError)) : ((numAttempts > 3) ? WwwCallResult.TooManyTimeoutsError : WwwCallResult.RetryTimeout);
		}

		private static void HandleResult(WwwCallResult result, string log, byte[] bytes, Uri uri, StringBuilder timeoutLogs, int numAttempts, Action<LoadAssetResult> callback, Action<Uri, Action<LoadAssetResult>, StringBuilder, int> retry, AbstractLogger logger)
		{
			switch (result)
			{
			case WwwCallResult.Success:
				logger.Debug(log);
				callback(new LoadAssetResult(true, bytes));
				break;
			case WwwCallResult.TooManyTimeoutsError:
				logger.Critical("Too many timeouts: " + uri.AbsoluteUri + "\nPrevious logs:\n" + timeoutLogs);
				callback(new LoadAssetResult(false, null));
				break;
			case WwwCallResult.TooManyServerErrors:
				logger.Critical("Too many server errors:\n" + log);
				callback(new LoadAssetResult(false, null));
				break;
			case WwwCallResult.RetryTimeout:
				logger.Error(log);
				timeoutLogs.Append(log);
				timeoutLogs.Append("\n\n");
				retry(uri, callback, timeoutLogs, numAttempts + 1);
				break;
			case WwwCallResult.RetryServerError:
				logger.Error(log);
				retry(uri, callback, timeoutLogs, numAttempts + 1);
				break;
			case WwwCallResult.OfflineError:
				logger.Error(log);
				callback(new LoadAssetResult(false, null));
				break;
			case WwwCallResult.ClientError:
				logger.Critical(log);
				callback(new LoadAssetResult(false, null));
				break;
			}
		}

		private static bool IsTimeout(string error)
		{
			if (!string.IsNullOrEmpty(error))
			{
				string text = error.ToLower();
				return text == "couldn't connect to host" || text.Contains("timedout") || text.Contains("timed out");
			}
			return false;
		}

		private static bool IsServerError(uint statusCode)
		{
			return statusCode >= 500 && statusCode <= 599;
		}

		private static bool IsClientError(uint statusCode)
		{
			return statusCode >= 400 && statusCode <= 499;
		}

		private static bool IsOfflineError(string error)
		{
			if (!string.IsNullOrEmpty(error))
			{
				string text = error.ToLower();
				return text.Contains("connection appears to be offline") || text.Contains("connection was lost") || text.Contains("network is unreachable");
			}
			return false;
		}

		private static bool IsSuccess(uint statusCode)
		{
			return statusCode >= 200 && statusCode <= 299;
		}
	}
}
