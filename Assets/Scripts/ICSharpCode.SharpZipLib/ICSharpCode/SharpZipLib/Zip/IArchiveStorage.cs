using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public interface IArchiveStorage
	{
		FileUpdateMode UpdateMode
		{
			get;
		}

		Stream GetTemporaryOutput();

		Stream ConvertTemporaryToFinal();

		Stream MakeTemporaryCopy(Stream stream);

		Stream OpenForDirectUpdate(Stream stream);

		void Dispose();
	}
}
