using System.IO;

namespace Disney.Mix.SDK.Internal
{
	public class DatabaseDirectoryCreator : IDatabaseDirectoryCreator
	{
		private const string SdkDirectoryName = "MixSDK";

		private const string DatabaseDirectoryName = "LocalUserDatabases";

		private readonly IFileSystem fileSystem;

		private readonly string localStorageDirPath;

		public DatabaseDirectoryCreator(IFileSystem fileSystem, string localStorageDirPath)
		{
			this.fileSystem = fileSystem;
			this.localStorageDirPath = localStorageDirPath;
		}

		public string CreateSdkDirectory(string documentCollectionName)
		{
			string dir = CreateSdkDatabasesDirectory();
			string path = HashedPathGenerator.GetPath(dir, documentCollectionName);
			EnsureDirectoryExists(path);
			return path;
		}

		public string CreateUserDirectory()
		{
			string sdkDatabasesDirPath = CreateSdkDatabasesDirectory();
			string text = CreateUserDatabasesDirectory(sdkDatabasesDirPath);
			EnsureDirectoryExists(text);
			return text;
		}

		public string CreateUserDirectory(string documentCollectionName)
		{
			string dir = CreateUserDirectory();
			string path = HashedPathGenerator.GetPath(dir, documentCollectionName);
			EnsureDirectoryExists(path);
			return path;
		}

		private string CreateSdkDatabasesDirectory()
		{
			string sdkDatabasesDirectory = GetSdkDatabasesDirectory();
			EnsureDirectoryExists(sdkDatabasesDirectory);
			return sdkDatabasesDirectory;
		}

		public string GetSdkDatabasesDirectory()
		{
			return Path.Combine(localStorageDirPath, "MixSDK");
		}

		private string CreateUserDatabasesDirectory(string sdkDatabasesDirPath)
		{
			string text = Path.Combine(sdkDatabasesDirPath, "LocalUserDatabases");
			EnsureDirectoryExists(text);
			return text;
		}

		private void EnsureDirectoryExists(string path)
		{
			if (!fileSystem.DirectoryExists(path))
			{
				fileSystem.CreateDirectory(path);
			}
		}
	}
}
