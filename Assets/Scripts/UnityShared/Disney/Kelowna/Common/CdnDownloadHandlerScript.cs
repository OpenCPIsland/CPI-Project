using Disney.LaunchPadFramework;
using System;
using System.IO;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common
{
	internal class CdnDownloadHandlerScript : DownloadHandlerScript
	{
		private int contentLength = -1;

		private long bytesReceived = 0L;

		private readonly string downloadFilename;

		private FileStream downloadFileStream;

		private bool cancelled = false;

		private bool disposed = false;

		public float Progress
		{
			get;
			private set;
		}

		public CdnDownloadHandlerScript(string downloadFilename)
		{
			this.downloadFilename = downloadFilename;
			openDownloadFile();
		}

		public CdnDownloadHandlerScript(string downloadFilename, byte[] buffer)
			: base(buffer)
		{
			this.downloadFilename = downloadFilename;
			openDownloadFile();
		}

		public void Cancel()
		{
			cancelled = true;
			closeDownloadFile();
		}

		public new void Dispose()
		{
			Dispose(true);
			base.Dispose();
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					closeDownloadFile();
				}
				disposed = true;
			}
		}

		~CdnDownloadHandlerScript()
		{
			Dispose(false);
		}

		protected override byte[] GetData()
		{
			return null;
		}

		protected override void ReceiveContentLength(int contentLength)
		{
			this.contentLength = contentLength;
		}

		protected override bool ReceiveData(byte[] data, int dataLength)
		{
			if (cancelled)
			{
				return false;
			}
			if (data == null || data.Length < 1)
			{
				Log.LogError(this, "CdnDownloadHandlerScript - ReceiveData() received a null/empty buffer!");
				return false;
			}
			try
			{
				if (downloadFileStream != null)
				{
					downloadFileStream.Write(data, 0, dataLength);
				}
				else
				{
					Log.LogErrorFormatted(this, "ReceiveData(...) - unable to write to download file (bytes received={0})\n '{1}'", bytesReceived + dataLength, downloadFilename);
				}
				bytesReceived += dataLength;
				if (contentLength > 0)
				{
					float num = (float)bytesReceived / (float)contentLength;
					if (num <= 0f)
					{
						Progress = 0f;
					}
					else if (num >= 1f)
					{
						Progress = 1f;
					}
					else
					{
						Progress = num;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		protected override float GetProgress()
		{
			return Progress;
		}

		protected override void CompleteContent()
		{
			closeDownloadFile();
		}

		private void openDownloadFile()
		{
			closeDownloadFile();
			downloadFileStream = new FileStream(downloadFilename, FileMode.Create, FileAccess.Write, FileShare.None, 4096);//, FileOptions.SequentialScan | FileOptions.WriteThrough);
		}

		private void closeDownloadFile()
		{
			if (downloadFileStream != null)
			{
				downloadFileStream.Dispose();
				downloadFileStream = null;
			}
		}
	}
}
