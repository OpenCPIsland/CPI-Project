using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IWwwCall : IDisposable
	{
		int RequestId
		{
			get;
		}

		byte[] ResponseBody
		{
			get;
		}

		string Error
		{
			get;
		}

		Dictionary<string, string> ResponseHeaders
		{
			get;
		}

		long TimeToStartUpload
		{
			get;
		}

		long TimeToFinishUpload
		{
			get;
		}

		float PercentUploaded
		{
			get;
		}

		long TimeToStartDownload
		{
			get;
		}

		long TimeToFinishDownload
		{
			get;
		}

		float PercentDownloaded
		{
			get;
		}

		string TimeoutReason
		{
			get;
		}

		long TimeoutTime
		{
			get;
		}

		uint StatusCode
		{
			get;
		}

		event EventHandler<WwwDoneEventArgs> OnDone;

		void Execute();
	}
}
