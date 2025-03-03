using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class DiskArchiveStorage : BaseArchiveStorage
	{
		private Stream temporaryStream_;

		private string fileName_;

		private string temporaryName_;

		public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode)
			: base(updateMode)
		{
			if (file.Name == null)
			{
				throw new ZipException("Cant handle non file archives");
			}
			fileName_ = file.Name;
		}

		public DiskArchiveStorage(ZipFile file)
			: this(file, FileUpdateMode.Safe)
		{
		}

		public override Stream GetTemporaryOutput()
		{
			if (temporaryName_ != null)
			{
				temporaryName_ = GetTempFileName(temporaryName_, true);
				temporaryStream_ = File.Open(temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			}
			else
			{
				temporaryName_ = Path.GetTempFileName();
				temporaryStream_ = File.Open(temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			}
			return temporaryStream_;
		}

		public override Stream ConvertTemporaryToFinal()
		{
			if (temporaryStream_ == null)
			{
				throw new ZipException("No temporary stream has been created");
			}
			Stream stream = null;
			string tempFileName = GetTempFileName(fileName_, false);
			bool flag = false;
			try
			{
				temporaryStream_.Close();
				File.Move(fileName_, tempFileName);
				File.Move(temporaryName_, fileName_);
				flag = true;
				File.Delete(tempFileName);
				return File.Open(fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception)
			{
				stream = null;
				if (!flag)
				{
					File.Move(tempFileName, fileName_);
					File.Delete(temporaryName_);
				}
				throw;
			}
		}

		public override Stream MakeTemporaryCopy(Stream stream)
		{
			stream.Close();
			temporaryName_ = GetTempFileName(fileName_, true);
			File.Copy(fileName_, temporaryName_, true);
			temporaryStream_ = new FileStream(temporaryName_, FileMode.Open, FileAccess.ReadWrite);
			return temporaryStream_;
		}

		public override Stream OpenForDirectUpdate(Stream stream)
		{
			if (stream == null || !stream.CanWrite)
			{
				if (stream != null)
				{
					stream.Close();
				}
				return new FileStream(fileName_, FileMode.Open, FileAccess.ReadWrite);
			}
			return stream;
		}

		public override void Dispose()
		{
			if (temporaryStream_ != null)
			{
				temporaryStream_.Close();
			}
		}

		private static string GetTempFileName(string original, bool makeTempFile)
		{
			string text = null;
			if (original == null)
			{
				text = Path.GetTempFileName();
			}
			else
			{
				int num = 0;
				int second = DateTime.Now.Second;
				while (text == null)
				{
					num++;
					string text2 = string.Format("{0}.{1}{2}.tmp", original, second, num);
					if (!File.Exists(text2))
					{
						if (makeTempFile)
						{
							try
							{
								using (File.Create(text2))
								{
								}
								text = text2;
							}
							catch
							{
								second = DateTime.Now.Second;
							}
						}
						else
						{
							text = text2;
						}
					}
				}
			}
			return text;
		}
	}
}
