using System.IO;

namespace Disney.Mix.SDK.Internal
{
	public class FileSystem : IFileSystem
	{
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public void DeleteDirectory(string path)
		{
			Directory.Delete(path, true);
		}

		public void WriteFile(string path, byte[] content)
		{
			File.WriteAllBytes(path, content);
		}

		public byte[] ReadFile(string path)
		{
			return File.ReadAllBytes(path);
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}
	}
}
