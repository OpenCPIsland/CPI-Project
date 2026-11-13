using Disney.LaunchPadFramework;
using System;
using System.IO;

namespace Disney.Kelowna.Common
{
	public class CdnGetFileHelper
	{
		public readonly string LogTitle;

		public readonly string ExpectedContentHash;

		private bool isDownloadCompleted = false;

		private bool isDownloadSuccessful = false;

		public bool IsDownloadCompleted
		{
			get
			{
				return isDownloadCompleted;
			}
		}

		public bool IsDownloadSuccessful
		{
			get
			{
				return isDownloadSuccessful;
			}
		}

		public CdnGetFileHelper(string logTitle, string expectedContentHash)
		{
			LogTitle = logTitle;
			ExpectedContentHash = expectedContentHash;
		}

		protected virtual void downloadCompleted(bool success, string filename, string errorMessage)
		{
		}

		protected virtual void downloadSucceeded(string filename)
		{
		}

		protected virtual void downloadFailed(string filename, string errorMessage)
		{
			Log.LogErrorFormatted(this, "{0}: Download failed - {1}:\n {2}", LogTitle, filename, errorMessage);
		}

		protected virtual void hashTestFailed(string filename, string calculatedHash)
		{
			Log.LogErrorFormatted(this, "Error downloading Launcher: File content hash does not match.\n Manifest entry hash: {0}, '{1}' hash: {2}, ", ExpectedContentHash, filename, calculatedHash);
		}

		protected virtual void downloadedFileNotFound(string filename)
		{
			Log.LogErrorFormatted(this, "Error downloading Launcher: Downloaded file not found at path: {0}", filename);
		}

		public void CdnGetFileCompleted(bool success, string filename, string errorMessage)
		{
			try
			{
				if (!success)
				{
					downloadFailed(filename, errorMessage);
				}
				else if (File.Exists(filename))
				{
					if (string.IsNullOrEmpty(ExpectedContentHash))
					{
						isDownloadSuccessful = true;
						downloadSucceeded(filename);
					}
					else
					{
						string text = ContentHash.CalculateHashForFile(false, filename);
						if (string.Equals(text, ExpectedContentHash, StringComparison.Ordinal))
						{
							isDownloadSuccessful = true;
							downloadSucceeded(filename);
						}
						else
						{
							hashTestFailed(filename, text);
						}
					}
				}
				else
				{
					downloadedFileNotFound(filename);
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				isDownloadCompleted = true;
				downloadCompleted(success, filename, errorMessage);
			}
		}
	}
}
