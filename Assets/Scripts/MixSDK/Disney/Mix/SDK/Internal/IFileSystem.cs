namespace Disney.Mix.SDK.Internal
{
	public interface IFileSystem
	{
		void CreateDirectory(string path);

		bool DirectoryExists(string path);

		void DeleteDirectory(string path);

		void WriteFile(string path, byte[] content);

		byte[] ReadFile(string path);

		bool FileExists(string path);
	}
}
