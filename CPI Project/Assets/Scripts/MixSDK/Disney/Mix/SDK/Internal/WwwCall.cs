using System;
using System.Collections;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class WwwCall : IWwwCall, IDisposable
	{
		private const string WwwCallTimeoutMessage = "timedout";

		private static readonly byte[] wwwDefaultResponseBody = new byte[0];

		private static readonly Dictionary<string, string> wwwDefaultResponseHeaders = new Dictionary<string, string>();

		private readonly AbstractLogger logger;

		private readonly Uri uri;

		private readonly HttpMethod method;

		private readonly byte[] requestBody;

		private readonly Dictionary<string, string> requestHeaders;

		private readonly ICoroutineManager coroutineManager;

		private readonly IStopwatch stopwatch;

		private readonly IWwwFactory wwwFactory;

		private readonly long latencyWwwCallTimeout;

		private readonly long maxWwwCallTimeout;

		private IWww www;

		private bool isDisposed;

		public int RequestId
		{
			get;
			private set;
		}

		public byte[] ResponseBody
		{
			get;
			private set;
		}

		public string Error
		{
			get;
			private set;
		}

		public Dictionary<string, string> ResponseHeaders
		{
			get;
			private set;
		}

		public long TimeToStartUpload
		{
			get;
			private set;
		}

		public long TimeToFinishUpload
		{
			get;
			private set;
		}

		public float PercentUploaded
		{
			get
			{
				return www.UploadProgress;
			}
		}

		public long TimeToStartDownload
		{
			get;
			private set;
		}

		public long TimeToFinishDownload
		{
			get;
			private set;
		}

		public float PercentDownloaded
		{
			get
			{
				return www.DownloadProgress;
			}
		}

		public string TimeoutReason
		{
			get;
			private set;
		}

		public long TimeoutTime
		{
			get;
			private set;
		}

		public uint StatusCode
		{
			get
			{
				return www.StatusCode;
			}
		}

		public event EventHandler<WwwDoneEventArgs> OnDone = delegate
		{
		};

		public WwwCall(AbstractLogger logger, int requestId, Uri uri, HttpMethod method, byte[] body, Dictionary<string, string> headers, ICoroutineManager coroutineManager, IStopwatch stopwatch, IWwwFactory wwwFactory, long latencyWwwCallTimeout, long maxWwwCallTimeout)
		{
			RequestId = requestId;
			this.logger = logger;
			this.uri = uri;
			this.method = method;
			requestBody = body;
			requestHeaders = headers;
			this.coroutineManager = coroutineManager;
			this.stopwatch = stopwatch;
			this.wwwFactory = wwwFactory;
			this.latencyWwwCallTimeout = latencyWwwCallTimeout;
			this.maxWwwCallTimeout = maxWwwCallTimeout;
		}

		public void Execute()
		{
			www = wwwFactory.Create(uri.AbsoluteUri, method, requestBody, requestHeaders);
			coroutineManager.Start(RunWww());
		}

		private IEnumerator RunWww()
		{
			long elapsedTime = 0L;
			long maxElapsedTime = 0L;
			float curUploadProgress = 0f;
			float curDownloadProgress = 0f;
			stopwatch.Start();
			try
			{
				www.Execute();
			}
			catch (Exception ex)
			{
				logger.Critical("Unhandled exception: " + ex);
				Error = ex.Message;
				ResponseBody = wwwDefaultResponseBody;
				ResponseHeaders = wwwDefaultResponseHeaders;
				this.OnDone(this, new WwwDoneEventArgs());
				yield break;
			}
			while (!www.IsDone)
			{
				if (elapsedTime < latencyWwwCallTimeout && maxElapsedTime < maxWwwCallTimeout)
				{
					yield return null;
					if (isDisposed)
					{
						yield break;
					}
					if (curUploadProgress != www.UploadProgress)
					{
						if (curUploadProgress == 0f)
						{
							TimeToStartUpload = stopwatch.ElapsedMilliseconds;
						}
						curUploadProgress = www.UploadProgress;
						if (curUploadProgress == 1f)
						{
							TimeToFinishUpload = stopwatch.ElapsedMilliseconds;
						}
					}
					if (curDownloadProgress != www.DownloadProgress)
					{
						if (curDownloadProgress == 0f)
						{
							TimeToStartDownload = stopwatch.ElapsedMilliseconds;
						}
						curDownloadProgress = www.DownloadProgress;
						if (curDownloadProgress == 1f)
						{
							TimeToFinishDownload = stopwatch.ElapsedMilliseconds;
						}
					}
					if (curUploadProgress == 1f && www.DownloadProgress == 0f)
					{
						elapsedTime = stopwatch.ElapsedMilliseconds - TimeToFinishUpload;
					}
					maxElapsedTime = stopwatch.ElapsedMilliseconds;
					continue;
				}
				stopwatch.Stop();
				TimeoutTime = stopwatch.ElapsedMilliseconds;
				Error = "timedout";
				ResponseBody = wwwDefaultResponseBody;
				ResponseHeaders = wwwDefaultResponseHeaders;
				TimeoutReason = ((elapsedTime >= latencyWwwCallTimeout) ? "Response didn't start in time" : "Whole process took too long");
				this.OnDone(this, new WwwDoneEventArgs());
				yield break;
			}
			stopwatch.Stop();
			Error = www.Error;
			ResponseBody = www.Bytes;
			ResponseHeaders = www.ResponseHeaders;
			this.OnDone(this, new WwwDoneEventArgs());
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				this.OnDone = null;
				coroutineManager.Start(DisposeWww());
			}
		}

		private IEnumerator DisposeWww()
		{
			while (!www.IsDone)
			{
				yield return null;
			}
			try
			{
				www.Dispose();
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
			}
		}
	}
}
