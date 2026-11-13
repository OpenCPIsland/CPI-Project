using System.Collections.Generic;
using System.IO;

namespace DeviceDB
{
	internal class FileSystem : IFileSystem
	{
		private readonly Dictionary<string, Stream> openStreams;

		public FileSystem()
		{
			openStreams = new Dictionary<string, Stream>();
		}

		public Stream OpenFileStream(string path)
		{
			Stream value;
			if (openStreams.TryGetValue(path, out value))
			{
				return value;
			}
			value = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
			openStreams.Add(path, value);
			return value;
		}

		public void DeleteFile(string path)
		{
			File.Delete(path);
			openStreams.Remove(path);
		}

		public bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}
	}
}
