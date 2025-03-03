namespace DI.CMS.FileManagement
{
	public interface IManifestLoader
	{
		void Load(FmsOptions options, string manifestUrl);

		bool IsLoaded();

		IFileManifest GetManifest();
	}
}
