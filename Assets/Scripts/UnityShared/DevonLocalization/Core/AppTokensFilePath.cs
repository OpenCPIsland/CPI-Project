namespace DevonLocalization.Core
{
	public class AppTokensFilePath : ILocalizedTokenFilePath
	{
		private const string RESOURCES_PATH_ROOT = "resources";

		private string directoryPath;

		private Platform platform;

		private string resourceExtension;

		private string remoteFileExtension;

		private string resourcePath;

		public AppTokensFilePath(string directoryPath, Platform platform = Platform.none, string resourceExtension = ".json", string remoteFileExtension = ".zip")
		{
			this.directoryPath = directoryPath;
			this.platform = platform;
			this.remoteFileExtension = remoteFileExtension;
			this.resourceExtension = resourceExtension;
			setupResourcePath();
		}

		private void setupResourcePath()
		{
			resourcePath = directoryPath;
			int num = resourcePath.ToLower().IndexOf("resources");
			if (num >= 0)
			{
				num += "resources".Length;
				resourcePath = resourcePath.Substring(num);
			}
			if (resourcePath.Length > 1 && resourcePath[0] == '/')
			{
				resourcePath = resourcePath.Substring(1, resourcePath.Length - 1);
			}
			if (resourcePath.Length > 0 && resourcePath[resourcePath.Length - 1] == '/')
			{
				resourcePath = resourcePath.Substring(0, resourcePath.Length - 1);
			}
		}

		public string GetResourceFilePathForLanguage(string language)
		{
			return resourcePath + "/" + ((platform == Platform.none) ? "" : (LocalizationPlatform.GetPlatformString(platform) + "/")) + language + resourceExtension;
		}

		public string GetLocalFilePathForLanguage(string language)
		{
			return directoryPath;
		}

		public string GetRemoteFileName(string language, string version)
		{
			return version + "\\" + language + remoteFileExtension;
		}
	}
}
