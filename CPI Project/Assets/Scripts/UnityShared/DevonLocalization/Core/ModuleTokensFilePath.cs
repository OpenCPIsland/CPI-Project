namespace DevonLocalization.Core
{
	public class ModuleTokensFilePath : ILocalizedTokenFilePath
	{
		private const string RESOURCES_PATH_ROOT = "resources";

		private const string MODULE_SUFFIX = "Module";

		private string directoryPath;

		private string moduleId;

		private Platform platform;

		private string localFileExtension;

		private string resourceExtension;

		private string remoteFileExtension;

		private string resourcePath;

		public ModuleTokensFilePath(string directoryPath, string moduleId, Platform platform, string localFileExtension = ".json.txt", string resourceExtension = ".json", string remoteFileExtension = ".zip")
		{
			this.directoryPath = directoryPath;
			this.moduleId = moduleId.ToLower();
			this.platform = platform;
			this.localFileExtension = localFileExtension;
			this.resourceExtension = resourceExtension;
			this.remoteFileExtension = remoteFileExtension;
			setupResourcePath();
		}

		private void setupResourcePath()
		{
			resourcePath = directoryPath;
			int num = resourcePath.ToLower().IndexOf("resources");
			if (num >= 0)
			{
				num += "resources".Length;
				resourcePath = directoryPath.Substring(num);
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
			return resourcePath + "/" + language + "/" + LocalizationPlatform.GetPlatformString(platform) + "/" + moduleId + resourceExtension;
		}

		public string GetLocalFilePathForLanguage(string language)
		{
			return directoryPath + "/" + language + "/" + LocalizationPlatform.GetPlatformString(platform) + "/" + moduleId + localFileExtension;
		}

		public string GetRemoteFileName(string language, string version)
		{
			return moduleId + "Module" + remoteFileExtension;
		}
	}
}
