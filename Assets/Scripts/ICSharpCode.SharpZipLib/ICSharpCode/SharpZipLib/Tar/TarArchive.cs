using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarArchive : IDisposable
	{
		private bool keepOldFiles;

		private bool asciiTranslate;

		private int userId;

		private string userName = string.Empty;

		private int groupId;

		private string groupName = string.Empty;

		private string rootPath;

		private string pathPrefix;

		private bool applyUserInfoOverrides;

		private TarInputStream tarIn;

		private TarOutputStream tarOut;

		private bool isDisposed;

		public bool AsciiTranslate
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return asciiTranslate;
			}
			set
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				asciiTranslate = value;
			}
		}

		public string PathPrefix
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return pathPrefix;
			}
			set
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				pathPrefix = value;
			}
		}

		public string RootPath
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return rootPath;
			}
			set
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				rootPath = value;
			}
		}

		public bool ApplyUserInfoOverrides
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return applyUserInfoOverrides;
			}
			set
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				applyUserInfoOverrides = value;
			}
		}

		public int UserId
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return userId;
			}
		}

		public string UserName
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return userName;
			}
		}

		public int GroupId
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return groupId;
			}
		}

		public string GroupName
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return groupName;
			}
		}

		public int RecordSize
		{
			get
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				if (tarIn != null)
				{
					return tarIn.RecordSize;
				}
				if (tarOut != null)
				{
					return tarOut.RecordSize;
				}
				return 10240;
			}
		}

		public bool IsStreamOwner
		{
			set
			{
				if (tarIn != null)
				{
					tarIn.IsStreamOwner = value;
				}
				else
				{
					tarOut.IsStreamOwner = value;
				}
			}
		}

		public event ProgressMessageHandler ProgressMessageEvent;

		protected virtual void OnProgressMessageEvent(TarEntry entry, string message)
		{
			ProgressMessageHandler progressMessageEvent = this.ProgressMessageEvent;
			if (progressMessageEvent != null)
			{
				progressMessageEvent(this, entry, message);
			}
		}

		protected TarArchive()
		{
		}

		protected TarArchive(TarInputStream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			tarIn = stream;
		}

		protected TarArchive(TarOutputStream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			tarOut = stream;
		}

		public static TarArchive CreateInputTarArchive(Stream inputStream)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			TarInputStream tarInputStream = inputStream as TarInputStream;
			if (tarInputStream != null)
			{
				return new TarArchive(tarInputStream);
			}
			return CreateInputTarArchive(inputStream, 20);
		}

		public static TarArchive CreateInputTarArchive(Stream inputStream, int blockFactor)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (inputStream is TarInputStream)
			{
				throw new ArgumentException("TarInputStream not valid");
			}
			return new TarArchive(new TarInputStream(inputStream, blockFactor));
		}

		public static TarArchive CreateOutputTarArchive(Stream outputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			TarOutputStream tarOutputStream = outputStream as TarOutputStream;
			if (tarOutputStream != null)
			{
				return new TarArchive(tarOutputStream);
			}
			return CreateOutputTarArchive(outputStream, 20);
		}

		public static TarArchive CreateOutputTarArchive(Stream outputStream, int blockFactor)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (outputStream is TarOutputStream)
			{
				throw new ArgumentException("TarOutputStream is not valid");
			}
			return new TarArchive(new TarOutputStream(outputStream, blockFactor));
		}

		public void SetKeepOldFiles(bool keepExistingFiles)
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			keepOldFiles = keepExistingFiles;
		}

		[Obsolete("Use the AsciiTranslate property")]
		public void SetAsciiTranslation(bool translateAsciiFiles)
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			asciiTranslate = translateAsciiFiles;
		}

		public void SetUserInfo(int userId, string userName, int groupId, string groupName)
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			this.userId = userId;
			this.userName = userName;
			this.groupId = groupId;
			this.groupName = groupName;
			applyUserInfoOverrides = true;
		}

		[Obsolete("Use Close instead")]
		public void CloseArchive()
		{
			Close();
		}

		public void ListContents()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			while (true)
			{
				TarEntry nextEntry = tarIn.GetNextEntry();
				if (nextEntry == null)
				{
					break;
				}
				OnProgressMessageEvent(nextEntry, null);
			}
		}

		public void ExtractContents(string destinationDirectory)
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			while (true)
			{
				TarEntry nextEntry = tarIn.GetNextEntry();
				if (nextEntry == null)
				{
					break;
				}
				ExtractEntry(destinationDirectory, nextEntry);
			}
		}

		private void ExtractEntry(string destDir, TarEntry entry)
		{
			OnProgressMessageEvent(entry, null);
			string text = entry.Name;
			if (Path.IsPathRooted(text))
			{
				text = text.Substring(Path.GetPathRoot(text).Length);
			}
			text = text.Replace('/', Path.DirectorySeparatorChar);
			string text2 = Path.Combine(destDir, text);
			if (entry.IsDirectory)
			{
				EnsureDirectoryExists(text2);
				return;
			}
			string directoryName = Path.GetDirectoryName(text2);
			EnsureDirectoryExists(directoryName);
			bool flag = true;
			FileInfo fileInfo = new FileInfo(text2);
			if (fileInfo.Exists)
			{
				if (keepOldFiles)
				{
					OnProgressMessageEvent(entry, "Destination file already exists");
					flag = false;
				}
				else if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
				{
					OnProgressMessageEvent(entry, "Destination file already exists, and is read-only");
					flag = false;
				}
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = false;
			Stream stream = File.Create(text2);
			if (asciiTranslate)
			{
				flag2 = !IsBinary(text2);
			}
			StreamWriter streamWriter = null;
			if (flag2)
			{
				streamWriter = new StreamWriter(stream);
			}
			byte[] array = new byte[32768];
			while (true)
			{
				int num = tarIn.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				if (flag2)
				{
					int num2 = 0;
					for (int i = 0; i < num; i++)
					{
						if (array[i] == 10)
						{
							string @string = Encoding.ASCII.GetString(array, num2, i - num2);
							streamWriter.WriteLine(@string);
							num2 = i + 1;
						}
					}
				}
				else
				{
					stream.Write(array, 0, num);
				}
			}
			if (flag2)
			{
				streamWriter.Close();
			}
			else
			{
				stream.Close();
			}
		}

		public void WriteEntry(TarEntry sourceEntry, bool recurse)
		{
			if (sourceEntry == null)
			{
				throw new ArgumentNullException("sourceEntry");
			}
			if (isDisposed)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			try
			{
				if (recurse)
				{
					TarHeader.SetValueDefaults(sourceEntry.UserId, sourceEntry.UserName, sourceEntry.GroupId, sourceEntry.GroupName);
				}
				WriteEntryCore(sourceEntry, recurse);
			}
			finally
			{
				if (recurse)
				{
					TarHeader.RestoreSetValues();
				}
			}
		}

		private void WriteEntryCore(TarEntry sourceEntry, bool recurse)
		{
			string text = null;
			string text2 = sourceEntry.File;
			TarEntry tarEntry = (TarEntry)sourceEntry.Clone();
			if (applyUserInfoOverrides)
			{
				tarEntry.GroupId = groupId;
				tarEntry.GroupName = groupName;
				tarEntry.UserId = userId;
				tarEntry.UserName = userName;
			}
			OnProgressMessageEvent(tarEntry, null);
			if (asciiTranslate && !tarEntry.IsDirectory && !IsBinary(text2))
			{
				text = Path.GetTempFileName();
				using (StreamReader streamReader = File.OpenText(text2))
				{
					using (Stream stream = File.Create(text))
					{
						while (true)
						{
							string text3 = streamReader.ReadLine();
							if (text3 == null)
							{
								break;
							}
							byte[] bytes = Encoding.ASCII.GetBytes(text3);
							stream.Write(bytes, 0, bytes.Length);
							stream.WriteByte(10);
						}
						stream.Flush();
					}
				}
				tarEntry.Size = new FileInfo(text).Length;
				text2 = text;
			}
			string text4 = null;
			if (rootPath != null && tarEntry.Name.StartsWith(rootPath))
			{
				text4 = tarEntry.Name.Substring(rootPath.Length + 1);
			}
			if (pathPrefix != null)
			{
				text4 = ((text4 == null) ? (pathPrefix + "/" + tarEntry.Name) : (pathPrefix + "/" + text4));
			}
			if (text4 != null)
			{
				tarEntry.Name = text4;
			}
			tarOut.PutNextEntry(tarEntry);
			if (tarEntry.IsDirectory)
			{
				if (recurse)
				{
					TarEntry[] directoryEntries = tarEntry.GetDirectoryEntries();
					for (int i = 0; i < directoryEntries.Length; i++)
					{
						WriteEntryCore(directoryEntries[i], recurse);
					}
				}
			}
			else
			{
				using (Stream stream2 = File.OpenRead(text2))
				{
					byte[] array = new byte[32768];
					while (true)
					{
						int num = stream2.Read(array, 0, array.Length);
						if (num <= 0)
						{
							break;
						}
						tarOut.Write(array, 0, num);
					}
				}
				if (text != null && text.Length > 0)
				{
					File.Delete(text);
				}
				tarOut.CloseEntry();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (isDisposed)
			{
				return;
			}
			isDisposed = true;
			if (disposing)
			{
				if (tarOut != null)
				{
					tarOut.Flush();
					tarOut.Close();
				}
				if (tarIn != null)
				{
					tarIn.Close();
				}
			}
		}

		public virtual void Close()
		{
			Dispose(true);
		}

		~TarArchive()
		{
			Dispose(false);
		}

		private static void EnsureDirectoryExists(string directoryName)
		{
			if (!Directory.Exists(directoryName))
			{
				try
				{
					Directory.CreateDirectory(directoryName);
				}
				catch (Exception ex)
				{
					throw new TarException("Exception creating directory '" + directoryName + "', " + ex.Message, ex);
				}
			}
		}

		private static bool IsBinary(string filename)
		{
			using (FileStream fileStream = File.OpenRead(filename))
			{
				int num = Math.Min(4096, (int)fileStream.Length);
				byte[] array = new byte[num];
				int num2 = fileStream.Read(array, 0, num);
				for (int i = 0; i < num2; i++)
				{
					byte b = array[i];
					if (b < 8 || (b > 13 && b < 32) || b == byte.MaxValue)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
