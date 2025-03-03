using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common
{
	public class CdnGet : IDisposable
	{
		public delegate void CdnGetStringComplete(bool success, string payload, string errorMessage);

		public delegate void CdnGetFileComplete(bool success, string filename, string errorMessage);

		private enum ModeEnum
		{
			GetString,
			GetFile
		}

		private int timeoutSeconds = 0;

		private readonly string contentPath;

		private readonly string saveToFilename;

		private CdnGetStringComplete onGetStringComplete;

		private CdnGetFileComplete onGetFileComplete;

		private readonly ModeEnum mode;

		private ICoroutine webRequestCoroutine;

		private UnityWebRequest unityWebRequest;

		private volatile float progress = 0f;

		private bool disposed = false;

		public int TimeoutSeconds
		{
			get
			{
				return timeoutSeconds;
			}
			set
			{
				if (webRequestCoroutine == null)
				{
					timeoutSeconds = value;
				}
			}
		}

		public CdnGet(string contentPath, CdnGetStringComplete onGetStringComplete)
		{
			mode = ModeEnum.GetString;
			this.contentPath = contentPath;
			this.onGetStringComplete = onGetStringComplete;
			saveToFilename = null;
			onGetFileComplete = null;
		}

		public CdnGet(string contentPath, string saveToFilename, CdnGetFileComplete onGetFileComplete)
		{
			mode = ModeEnum.GetFile;
			this.contentPath = contentPath;
			onGetStringComplete = null;
			this.saveToFilename = saveToFilename;
			this.onGetFileComplete = onGetFileComplete;
		}

		~CdnGet()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				if (webRequestCoroutine != null && !webRequestCoroutine.Disposed)
				{
					webRequestCoroutine.Stop();
					webRequestCoroutine = null;
				}
				if (unityWebRequest != null)
				{
					unityWebRequest.Dispose();
					unityWebRequest = null;
				}
			}
			onGetStringComplete = null;
			onGetFileComplete = null;
			disposed = true;
		}

		public void Execute()
		{
			if (webRequestCoroutine != null)
			{
				string message = "'CdnGet' objects can only be used once!";
				throw new Exception(message);
			}
			webRequestCoroutine = CoroutineRunner.Start(get(), this, "CdnGet");
			webRequestCoroutine.ECancelled += cleanup;
			webRequestCoroutine.ECompleted += cleanup;
		}

		public float GetProgress()
		{
			if (unityWebRequest != null)
			{
				progress = unityWebRequest.downloadProgress;
			}
			return progress;
		}

		public void Cancel()
		{
			if (webRequestCoroutine != null && !webRequestCoroutine.Completed && !webRequestCoroutine.Cancelled)
			{
				webRequestCoroutine.Cancel();
			}
		}

		private void cleanup()
		{
			if (webRequestCoroutine != null)
			{
				webRequestCoroutine.ECancelled -= cleanup;
				webRequestCoroutine.ECompleted -= cleanup;
			}
		}

		private IEnumerator get()
		{
			ICPipeManifestService cpipeManifestService = Service.Get<ICPipeManifestService>();
			CPipeManifestResponse cpipeManifestResponse = new CPipeManifestResponse();
			yield return cpipeManifestService.LookupAssetUrl(cpipeManifestResponse, contentPath);
			if (string.IsNullOrEmpty(cpipeManifestResponse.FullAssetUrl))
			{
				Log.LogErrorFormatted(this, "ERROR: CdnGet - contentPath '{0}' NOT FOUND!", contentPath);
				yield break;
			}
			switch (mode)
			{
			case ModeEnum.GetString:
				yield return getString(cpipeManifestResponse.FullAssetUrl);
				break;
			case ModeEnum.GetFile:
				yield return getFile(cpipeManifestResponse.FullAssetUrl);
				break;
			}
		}

		private IEnumerator getString(string fullAssetUrl)
		{
			unityWebRequest = UnityWebRequest.Get(fullAssetUrl);
			unityWebRequest.timeout = timeoutSeconds;
			unityWebRequest.disposeDownloadHandlerOnDispose = true;
			yield return unityWebRequest.SendWebRequest();
			if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
			{
				Log.LogErrorFormatted(this, "Error: getString({0}):\n{1}", fullAssetUrl, unityWebRequest.error);
				if (onGetStringComplete != null)
				{
					onGetStringComplete(false, null, unityWebRequest.error);
				}
			}
			else if (onGetStringComplete != null)
			{
				onGetStringComplete(true, unityWebRequest.downloadHandler.text, null);
			}
			onGetStringComplete = null;
			unityWebRequest.Dispose();
			unityWebRequest = null;
		}

		private IEnumerator getFile(string fullAssetUrl)
		{
			unityWebRequest = new UnityWebRequest(fullAssetUrl, "GET");
			unityWebRequest.timeout = timeoutSeconds;
			unityWebRequest.disposeDownloadHandlerOnDispose = true;
			unityWebRequest.downloadHandler = new CdnDownloadHandlerScript(saveToFilename);
			yield return unityWebRequest.SendWebRequest();
			if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
			{
				Log.LogErrorFormatted(this, "Error: getFile({0}):\n{1}", fullAssetUrl, unityWebRequest.error);
				if (onGetFileComplete != null)
				{
					onGetFileComplete(false, saveToFilename, unityWebRequest.error);
				}
			}
			else if (onGetFileComplete != null)
			{
				onGetFileComplete(true, saveToFilename, null);
			}
			onGetFileComplete = null;
			unityWebRequest.Dispose();
			unityWebRequest = null;
		}
	}
}
