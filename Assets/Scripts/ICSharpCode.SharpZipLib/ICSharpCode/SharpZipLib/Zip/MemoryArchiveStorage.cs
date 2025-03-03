using ICSharpCode.SharpZipLib.Core;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class MemoryArchiveStorage : BaseArchiveStorage
	{
		private MemoryStream temporaryStream_;

		private MemoryStream finalStream_;

		public MemoryStream FinalStream
		{
			get
			{
				return finalStream_;
			}
		}

		public MemoryArchiveStorage()
			: base(FileUpdateMode.Direct)
		{
		}

		public MemoryArchiveStorage(FileUpdateMode updateMode)
			: base(updateMode)
		{
		}

		public override Stream GetTemporaryOutput()
		{
			temporaryStream_ = new MemoryStream();
			return temporaryStream_;
		}

		public override Stream ConvertTemporaryToFinal()
		{
			if (temporaryStream_ == null)
			{
				throw new ZipException("No temporary stream has been created");
			}
			finalStream_ = new MemoryStream(temporaryStream_.ToArray());
			return finalStream_;
		}

		public override Stream MakeTemporaryCopy(Stream stream)
		{
			temporaryStream_ = new MemoryStream();
			stream.Position = 0L;
			StreamUtils.Copy(stream, temporaryStream_, new byte[4096]);
			return temporaryStream_;
		}

		public override Stream OpenForDirectUpdate(Stream stream)
		{
			Stream stream2;
			if (stream == null || !stream.CanWrite)
			{
				stream2 = new MemoryStream();
				if (stream != null)
				{
					stream.Position = 0L;
					StreamUtils.Copy(stream, stream2, new byte[4096]);
					stream.Close();
				}
			}
			else
			{
				stream2 = stream;
			}
			return stream2;
		}

		public override void Dispose()
		{
			if (temporaryStream_ != null)
			{
				temporaryStream_.Close();
			}
		}
	}
}
