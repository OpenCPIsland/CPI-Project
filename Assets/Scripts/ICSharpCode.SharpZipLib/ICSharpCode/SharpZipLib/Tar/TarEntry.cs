using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarEntry : ICloneable
	{
		private string file;

		private TarHeader header;

		public TarHeader TarHeader
		{
			get
			{
				return header;
			}
		}

		public string Name
		{
			get
			{
				return header.Name;
			}
			set
			{
				header.Name = value;
			}
		}

		public int UserId
		{
			get
			{
				return header.UserId;
			}
			set
			{
				header.UserId = value;
			}
		}

		public int GroupId
		{
			get
			{
				return header.GroupId;
			}
			set
			{
				header.GroupId = value;
			}
		}

		public string UserName
		{
			get
			{
				return header.UserName;
			}
			set
			{
				header.UserName = value;
			}
		}

		public string GroupName
		{
			get
			{
				return header.GroupName;
			}
			set
			{
				header.GroupName = value;
			}
		}

		public DateTime ModTime
		{
			get
			{
				return header.ModTime;
			}
			set
			{
				header.ModTime = value;
			}
		}

		public string File
		{
			get
			{
				return file;
			}
		}

		public long Size
		{
			get
			{
				return header.Size;
			}
			set
			{
				header.Size = value;
			}
		}

		public bool IsDirectory
		{
			get
			{
				if (file != null)
				{
					return Directory.Exists(file);
				}
				if (header != null && (header.TypeFlag == 53 || Name.EndsWith("/")))
				{
					return true;
				}
				return false;
			}
		}

		private TarEntry()
		{
			header = new TarHeader();
		}

		public TarEntry(byte[] headerBuffer)
		{
			header = new TarHeader();
			header.ParseBuffer(headerBuffer);
		}

		public TarEntry(TarHeader header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			this.header = (TarHeader)header.Clone();
		}

		public object Clone()
		{
			TarEntry tarEntry = new TarEntry();
			tarEntry.file = file;
			tarEntry.header = (TarHeader)header.Clone();
			tarEntry.Name = Name;
			return tarEntry;
		}

		public static TarEntry CreateTarEntry(string name)
		{
			TarEntry tarEntry = new TarEntry();
			NameTarHeader(tarEntry.header, name);
			return tarEntry;
		}

		public static TarEntry CreateEntryFromFile(string fileName)
		{
			TarEntry tarEntry = new TarEntry();
			tarEntry.GetFileTarHeader(tarEntry.header, fileName);
			return tarEntry;
		}

		public override bool Equals(object obj)
		{
			TarEntry tarEntry = obj as TarEntry;
			if (tarEntry != null)
			{
				return Name.Equals(tarEntry.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public bool IsDescendent(TarEntry toTest)
		{
			if (toTest == null)
			{
				throw new ArgumentNullException("toTest");
			}
			return toTest.Name.StartsWith(Name);
		}

		public void SetIds(int userId, int groupId)
		{
			UserId = userId;
			GroupId = groupId;
		}

		public void SetNames(string userName, string groupName)
		{
			UserName = userName;
			GroupName = groupName;
		}

		public void GetFileTarHeader(TarHeader header, string file)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			this.file = file;
			string text = file;
			text = text.Replace(Path.DirectorySeparatorChar, '/');
			while (text.StartsWith("/"))
			{
				text = text.Substring(1);
			}
			header.LinkName = string.Empty;
			header.Name = text;
			if (Directory.Exists(file))
			{
				header.Mode = 1003;
				header.TypeFlag = 53;
				if (header.Name.Length == 0 || header.Name[header.Name.Length - 1] != '/')
				{
					header.Name += "/";
				}
				header.Size = 0L;
			}
			else
			{
				header.Mode = 33216;
				header.TypeFlag = 48;
				header.Size = new FileInfo(file.Replace('/', Path.DirectorySeparatorChar)).Length;
			}
			header.ModTime = System.IO.File.GetLastWriteTime(file.Replace('/', Path.DirectorySeparatorChar)).ToUniversalTime();
			header.DevMajor = 0;
			header.DevMinor = 0;
		}

		public TarEntry[] GetDirectoryEntries()
		{
			if (file == null || !Directory.Exists(file))
			{
				return new TarEntry[0];
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries(file);
			TarEntry[] array = new TarEntry[fileSystemEntries.Length];
			for (int i = 0; i < fileSystemEntries.Length; i++)
			{
				array[i] = CreateEntryFromFile(fileSystemEntries[i]);
			}
			return array;
		}

		public void WriteEntryHeader(byte[] outBuffer)
		{
			header.WriteHeader(outBuffer);
		}

		public static void AdjustEntryName(byte[] buffer, string newName)
		{
			TarHeader.GetNameBytes(newName, buffer, 0, 100);
		}

		public static void NameTarHeader(TarHeader header, string name)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			bool flag = name.EndsWith("/");
			header.Name = name;
			header.Mode = (flag ? 1003 : 33216);
			header.UserId = 0;
			header.GroupId = 0;
			header.Size = 0L;
			header.ModTime = DateTime.UtcNow;
			header.TypeFlag = (byte)(flag ? 53 : 48);
			header.LinkName = string.Empty;
			header.UserName = string.Empty;
			header.GroupName = string.Empty;
			header.DevMajor = 0;
			header.DevMinor = 0;
		}
	}
}
