using System.IO;

namespace DeviceDB
{
	internal interface IFileSystem
	{
		Stream OpenFileStream(string path);

		void DeleteFile(string path);

		bool DirectoryExists(string path);

		void CreateDirectory(string path);
	}
}
