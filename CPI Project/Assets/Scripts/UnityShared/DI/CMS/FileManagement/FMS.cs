using UnityEngine;

namespace DI.CMS.FileManagement
{
	public class FMS
	{
		private IManifestLoader manifestLoader;

		public void Init(FmsOptions options)
		{
			string manifestUrl = "";
			if (options.JSONParser == null)
			{
				throw new CMSException("JSON parser implementation is required.");
			}
			Debug.Log("JSON parser: " + options.JSONParser.GetDescription());
			switch (options.Mode)
			{
			case FmsMode.Versioned:
				manifestUrl = string.Format("{0}manifest/{1}/{2}/{3}.json", options.BaseURL, options.CodeName, options.Env.ToString().ToLower(), options.ManifestVersion);
				manifestLoader = new VersionedFileManifestLoader(options);
				break;
			case FmsMode.Passthrough:
				manifestLoader = new PassthroughFileManifestLoader();
				break;
			}
			manifestLoader.Load(options, manifestUrl);
		}

		public string GetFileUrl(string relativePath)
		{
			if (manifestLoader == null)
			{
				Debug.LogWarning(string.Format("A file URL was requested for {0} but the manifest has not been instantiated.", relativePath));
				return "";
			}
			if (manifestLoader.IsLoaded())
			{
				return manifestLoader.GetManifest().TranslateFileUrl(relativePath);
			}
			Debug.LogWarning(string.Format("A file URL was requested for {0}, but the manifest is not ready.", relativePath));
			return "";
		}

		public int GetFileVersion(string relativePath)
		{
			return manifestLoader.GetManifest().GetVersionFromFileUrl(relativePath);
		}

		public string GetManifestVersion()
		{
			return manifestLoader.GetManifest().GetManifestVersion();
		}
	}
}
