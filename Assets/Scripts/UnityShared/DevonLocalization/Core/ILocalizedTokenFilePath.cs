namespace DevonLocalization.Core
{
	public interface ILocalizedTokenFilePath
	{
		string GetResourceFilePathForLanguage(string language);

		string GetLocalFilePathForLanguage(string language);

		string GetRemoteFileName(string language, string version);
	}
}
