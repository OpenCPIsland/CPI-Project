using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IWww : IDisposable
	{
		bool IsDone
		{
			get;
		}

		float UploadProgress
		{
			get;
		}

		float DownloadProgress
		{
			get;
		}

		string Error
		{
			get;
		}

		byte[] Bytes
		{
			get;
		}

		Dictionary<string, string> ResponseHeaders
		{
			get;
		}

		uint StatusCode
		{
			get;
		}

		void Execute();
	}
}
