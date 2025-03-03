using System;
using UnityEngine;

namespace DI.CMS.FileManagement
{
	public class PassthroughFileManifestLoader : IManifestLoader
	{
		private IFileManifest manifest;

		public void Load(FmsOptions options, string manifestUrl)
		{
			manifest = new PassthroughFileManifest();
			manifest.Prepare(options, "");
			Caching.ClearCache();
			if (options.FMSListener != null)
			{
				try
				{
					options.FMSListener.OnManifestLoadSuccess();
				}
				catch (Exception exception)
				{
					Debug.LogError("The FMS Listener threw an exception.");
					Debug.LogException(exception);
				}
				options.FMSListener.OnManifestLoadComplete();
			}
		}

		public bool IsLoaded()
		{
			return manifest != null;
		}

		public IFileManifest GetManifest()
		{
			if (!IsLoaded())
			{
				throw new Exception("The passthrough manifest has not been instantiated. Has PassthroughFileManifestLoader.Load been called?");
			}
			return manifest;
		}
	}
}
