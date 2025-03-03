using System;
using System.Collections;
using UnityEngine;

namespace DI.CMS.FileManagement
{
	public class VersionedFileManifestLoader : IManifestLoader
	{
		private const int MAX_LOAD_ATTEMPTS = 3;

		private const float LOAD_ATTEMPT_INTERVAL = 0.5f;

		private FmsOptions options;

		private IFileManifest manifest;

		private string manifestUrl;

		private int loadAttempts;

		public VersionedFileManifestLoader(FmsOptions options)
		{
		}

		public void Load(FmsOptions options, string manifestUrl)
		{
			this.options = options;
			this.manifestUrl = manifestUrl;
			Debug.Log(string.Format("Setting manifestUrl to {0}", manifestUrl));
			AttemptManifestRequest(0u, null);
		}

		public bool IsLoaded()
		{
			return manifest != null;
		}

		public IFileManifest GetManifest()
		{
			if (!IsLoaded())
			{
				throw new Exception("The versioned manifest has not been instantiated yet. Has VersionedFileManifestLoader.Load been called?");
			}
			return manifest;
		}

		private void AttemptManifestRequest(uint id, object cookie)
		{
			if (++loadAttempts > 3)
			{
				if (options.FMSListener != null)
				{
					try
					{
						options.FMSListener.OnManifestLoadFailure(new CMSException("Retry count (" + 3 + ") exceeded."));
					}
					catch (Exception)
					{
					}
					options.FMSListener.OnManifestLoadComplete();
				}
			}
			else
			{
				options.Context.StartCoroutine(RequestManifestFile());
			}
		}

		private void RetryRequest()
		{
			AttemptManifestRequest(0u, null);
		}

		private IEnumerator RequestManifestFile()
		{
			WWW www = new WWW(manifestUrl);
			yield return www;
			if (www.error != null)
			{
				Debug.LogError(string.Format("Unable to request manifest file [{0}] on attempt #{1} with the following error: {2}", manifestUrl, loadAttempts, www.error));
				RetryRequest();
			}
			else if (www.isDone)
			{
				if (www.text != "")
				{
					PrepareManifest(www.text);
				}
				else
				{
					Debug.LogError(string.Format("Manifest request attempt #{0} yielded an empty manifest.", loadAttempts));
					RetryRequest();
				}
			}
			www.Dispose();
		}

		private void PrepareManifest(string json)
		{
			Exception ex = null;
			manifest = new VersionedFileManifest();
			try
			{
				manifest.Prepare(options, json);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			if (options.FMSListener != null)
			{
				try
				{
					if (ex != null)
					{
						options.FMSListener.OnManifestLoadFailure(ex);
					}
					else
					{
						options.FMSListener.OnManifestLoadSuccess();
					}
				}
				catch (Exception exception)
				{
					Debug.LogError("The FMS Listener threw an exception.");
					Debug.LogException(exception);
				}
				options.FMSListener.OnManifestLoadComplete();
			}
			else if (ex != null)
			{
				throw ex;
			}
		}
	}
}
