using UnityEngine;

namespace DI.CMS.FileManagement
{
	public class PassthroughFileManifest : IFileManifest
	{
		private FmsOptions options;

		public void Prepare(FmsOptions options, string json)
		{
			this.options = options;
			Debug.Log("Passthrough manifest is ready.");
		}

		public string TranslateFileUrl(string relativePath)
		{
			return options.RootUrl + relativePath;
		}

		public int GetVersionFromFileUrl(string relativePath)
		{
			return 0;
		}

		public string GetManifestVersion()
		{
			return "0";
		}
	}
}
