using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipEntry : ICloneable
	{
		[Flags]
		private enum Known : byte
		{
			None = 0x0,
			Size = 0x1,
			CompressedSize = 0x2,
			Crc = 0x4,
			Time = 0x8,
			ExternalAttributes = 0x10
		}

		internal int AESKeySize;

		private Known known;

		private int externalFileAttributes = -1;

		private ushort versionMadeBy;

		private string name;

		private ulong size;

		private ulong compressedSize;

		private ushort versionToExtract;

		private uint crc;

		private uint dosTime;

		private CompressionMethod method = CompressionMethod.Deflated;

		private byte[] extra;

		private string comment;

		private int flags;

		private long zipFileIndex = -1L;

		private long offset;

		private bool forceZip64_;

		private byte cryptoCheckValue_;

		public bool HasCrc
		{
			get
			{
				return (known & Known.Crc) != 0;
			}
		}

		public bool IsCrypted
		{
			get
			{
				return (flags & 1) != 0;
			}
			set
			{
				if (value)
				{
					flags |= 1;
				}
				else
				{
					flags &= -2;
				}
			}
		}

		public bool IsUnicodeText
		{
			get
			{
				return (flags & 0x800) != 0;
			}
			set
			{
				if (value)
				{
					flags |= 2048;
				}
				else
				{
					flags &= -2049;
				}
			}
		}

		internal byte CryptoCheckValue
		{
			get
			{
				return cryptoCheckValue_;
			}
			set
			{
				cryptoCheckValue_ = value;
			}
		}

		public int Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}

		public long ZipFileIndex
		{
			get
			{
				return zipFileIndex;
			}
			set
			{
				zipFileIndex = value;
			}
		}

		public long Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		public int ExternalFileAttributes
		{
			get
			{
				if ((known & Known.ExternalAttributes) == 0)
				{
					return -1;
				}
				return externalFileAttributes;
			}
			set
			{
				externalFileAttributes = value;
				known |= Known.ExternalAttributes;
			}
		}

		public int VersionMadeBy
		{
			get
			{
				return versionMadeBy & 0xFF;
			}
		}

		public bool IsDOSEntry
		{
			get
			{
				if (HostSystem != 0)
				{
					return HostSystem == 10;
				}
				return true;
			}
		}

		public int HostSystem
		{
			get
			{
				return (versionMadeBy >> 8) & 0xFF;
			}
			set
			{
				versionMadeBy &= 255;
				versionMadeBy |= (ushort)((value & 0xFF) << 8);
			}
		}

		public int Version
		{
			get
			{
				if (versionToExtract != 0)
				{
					return versionToExtract;
				}
				int result = 10;
				if (AESKeySize > 0)
				{
					result = 51;
				}
				else if (CentralHeaderRequiresZip64)
				{
					result = 45;
				}
				else if (CompressionMethod.Deflated == method)
				{
					result = 20;
				}
				else if (IsDirectory)
				{
					result = 20;
				}
				else if (IsCrypted)
				{
					result = 20;
				}
				else if (HasDosAttributes(8))
				{
					result = 11;
				}
				return result;
			}
		}

		public bool CanDecompress
		{
			get
			{
				if (Version <= 51 && (Version == 10 || Version == 11 || Version == 20 || Version == 45 || Version == 51))
				{
					return IsCompressionMethodSupported();
				}
				return false;
			}
		}

		public bool LocalHeaderRequiresZip64
		{
			get
			{
				bool flag = forceZip64_;
				if (!flag)
				{
					ulong num = compressedSize;
					if (versionToExtract == 0 && IsCrypted)
					{
						num += 12;
					}
					flag = ((size >= uint.MaxValue || num >= uint.MaxValue) && (versionToExtract == 0 || versionToExtract >= 45));
				}
				return flag;
			}
		}

		public bool CentralHeaderRequiresZip64
		{
			get
			{
				if (!LocalHeaderRequiresZip64)
				{
					return offset >= uint.MaxValue;
				}
				return true;
			}
		}

		public long DosTime
		{
			get
			{
				if ((known & Known.Time) == 0)
				{
					return 0L;
				}
				return dosTime;
			}
			set
			{
				dosTime = (uint)value;
				known |= Known.Time;
			}
		}

		public DateTime DateTime
		{
			get
			{
				uint second = Math.Min(59u, 2 * (dosTime & 0x1F));
				uint minute = Math.Min(59u, (dosTime >> 5) & 0x3F);
				uint hour = Math.Min(23u, (dosTime >> 11) & 0x1F);
				uint month = Math.Max(1u, Math.Min(12u, (dosTime >> 21) & 0xF));
				uint year = ((dosTime >> 25) & 0x7F) + 1980;
				int day = Math.Max(1, Math.Min(DateTime.DaysInMonth((int)year, (int)month), (int)((dosTime >> 16) & 0x1F)));
				return new DateTime((int)year, (int)month, day, (int)hour, (int)minute, (int)second);
			}
			set
			{
				uint num = (uint)value.Year;
				uint num2 = (uint)value.Month;
				uint num3 = (uint)value.Day;
				uint num4 = (uint)value.Hour;
				uint num5 = (uint)value.Minute;
				uint num6 = (uint)value.Second;
				if (num < 1980)
				{
					num = 1980u;
					num2 = 1u;
					num3 = 1u;
					num4 = 0u;
					num5 = 0u;
					num6 = 0u;
				}
				else if (num > 2107)
				{
					num = 2107u;
					num2 = 12u;
					num3 = 31u;
					num4 = 23u;
					num5 = 59u;
					num6 = 59u;
				}
				DosTime = ((((num - 1980) & 0x7F) << 25) | (num2 << 21) | (num3 << 16) | (num4 << 11) | (num5 << 5) | (num6 >> 1));
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public long Size
		{
			get
			{
				if ((known & Known.Size) == 0)
				{
					return -1L;
				}
				return (long)size;
			}
			set
			{
				size = (ulong)value;
				known |= Known.Size;
			}
		}

		public long CompressedSize
		{
			get
			{
				if ((known & Known.CompressedSize) == 0)
				{
					return -1L;
				}
				return (long)compressedSize;
			}
			set
			{
				compressedSize = (ulong)value;
				known |= Known.CompressedSize;
			}
		}

		public long Crc
		{
			get
			{
				if ((known & Known.Crc) == 0)
				{
					return -1L;
				}
				return (long)crc & 4294967295L;
			}
			set
			{
				if ((crc & -4294967296L) != 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				crc = (uint)value;
				known |= Known.Crc;
			}
		}

		public CompressionMethod CompressionMethod
		{
			get
			{
				return method;
			}
			set
			{
				if (!IsCompressionMethodSupported(value))
				{
					throw new NotSupportedException("Compression method not supported");
				}
				method = value;
			}
		}

		internal CompressionMethod CompressionMethodForHeader
		{
			get
			{
				if (AESKeySize <= 0)
				{
					return method;
				}
				return CompressionMethod.WinZipAES;
			}
		}

		public byte[] ExtraData
		{
			get
			{
				return extra;
			}
			set
			{
				if (value == null)
				{
					extra = null;
					return;
				}
				if (value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				extra = new byte[value.Length];
				Array.Copy(value, 0, extra, 0, value.Length);
			}
		}

		internal int AESSaltLen
		{
			get
			{
				return AESKeySize / 16;
			}
		}

		internal int AESOverheadSize
		{
			get
			{
				return 12 + AESSaltLen;
			}
		}

		public string Comment
		{
			get
			{
				return comment;
			}
			set
			{
				if (value != null && value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("value", "cannot exceed 65535");
				}
				comment = value;
			}
		}

		public bool IsDirectory
		{
			get
			{
				int length = name.Length;
				return (length > 0 && (name[length - 1] == '/' || name[length - 1] == '\\')) || HasDosAttributes(16);
			}
		}

		public bool IsFile
		{
			get
			{
				if (!IsDirectory)
				{
					return !HasDosAttributes(8);
				}
				return false;
			}
		}

		public ZipEntry(string name)
			: this(name, 0, 51, CompressionMethod.Deflated)
		{
		}

		internal ZipEntry(string name, int versionRequiredToExtract)
			: this(name, versionRequiredToExtract, 51, CompressionMethod.Deflated)
		{
		}

		internal ZipEntry(string name, int versionRequiredToExtract, int madeByInfo, CompressionMethod method)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length > 65535)
			{
				throw new ArgumentException("Name is too long", "name");
			}
			if (versionRequiredToExtract != 0 && versionRequiredToExtract < 10)
			{
				throw new ArgumentOutOfRangeException("versionRequiredToExtract");
			}
			DateTime = DateTime.Now;
			this.name = name;
			versionMadeBy = (ushort)madeByInfo;
			versionToExtract = (ushort)versionRequiredToExtract;
			this.method = method;
		}

		[Obsolete("Use Clone instead")]
		public ZipEntry(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			known = entry.known;
			name = entry.name;
			size = entry.size;
			compressedSize = entry.compressedSize;
			crc = entry.crc;
			dosTime = entry.dosTime;
			method = entry.method;
			comment = entry.comment;
			versionToExtract = entry.versionToExtract;
			versionMadeBy = entry.versionMadeBy;
			externalFileAttributes = entry.externalFileAttributes;
			flags = entry.flags;
			zipFileIndex = entry.zipFileIndex;
			offset = entry.offset;
			forceZip64_ = entry.forceZip64_;
			if (entry.extra != null)
			{
				extra = new byte[entry.extra.Length];
				Array.Copy(entry.extra, 0, extra, 0, entry.extra.Length);
			}
		}

		private bool HasDosAttributes(int attributes)
		{
			bool result = false;
			if ((known & Known.ExternalAttributes) != 0 && (HostSystem == 0 || HostSystem == 10) && (ExternalFileAttributes & attributes) == attributes)
			{
				result = true;
			}
			return result;
		}

		public void ForceZip64()
		{
			forceZip64_ = true;
		}

		public bool IsZip64Forced()
		{
			return forceZip64_;
		}

		internal void ProcessExtraData(bool localHeader)
		{
			ZipExtraData zipExtraData = new ZipExtraData(extra);
			if (zipExtraData.Find(1))
			{
				forceZip64_ = true;
				if (zipExtraData.ValueLength < 4)
				{
					throw new ZipException("Extra data extended Zip64 information length is invalid");
				}
				if (localHeader || size == uint.MaxValue)
				{
					size = (ulong)zipExtraData.ReadLong();
				}
				if (localHeader || compressedSize == uint.MaxValue)
				{
					compressedSize = (ulong)zipExtraData.ReadLong();
				}
				if (!localHeader && offset == uint.MaxValue)
				{
					offset = zipExtraData.ReadLong();
				}
			}
			else if ((versionToExtract & 0xFF) >= 45 && (size == uint.MaxValue || compressedSize == uint.MaxValue))
			{
				throw new ZipException("Zip64 Extended information required but is missing.");
			}
			if (zipExtraData.Find(10))
			{
				if (zipExtraData.ValueLength < 4)
				{
					throw new ZipException("NTFS Extra data invalid");
				}
				zipExtraData.ReadInt();
				while (zipExtraData.UnreadCount >= 4)
				{
					int num = zipExtraData.ReadShort();
					int num2 = zipExtraData.ReadShort();
					if (num == 1)
					{
						if (num2 >= 24)
						{
							long fileTime = zipExtraData.ReadLong();
							zipExtraData.ReadLong();
							zipExtraData.ReadLong();
							DateTime = DateTime.FromFileTime(fileTime);
						}
						break;
					}
					zipExtraData.Skip(num2);
				}
			}
			else if (zipExtraData.Find(21589))
			{
				int valueLength = zipExtraData.ValueLength;
				int num3 = zipExtraData.ReadByte();
				if ((num3 & 1) != 0 && valueLength >= 5)
				{
					int seconds = zipExtraData.ReadInt();
					DateTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
				}
			}
			if (method == CompressionMethod.WinZipAES)
			{
				ProcessAESExtraData(zipExtraData);
			}
		}

		private void ProcessAESExtraData(ZipExtraData extraData)
		{
			throw new ZipException("AES unsupported");
		}

		public bool IsCompressionMethodSupported()
		{
			return IsCompressionMethodSupported(CompressionMethod);
		}

		public object Clone()
		{
			ZipEntry zipEntry = (ZipEntry)MemberwiseClone();
			if (extra != null)
			{
				zipEntry.extra = new byte[extra.Length];
				Array.Copy(extra, 0, zipEntry.extra, 0, extra.Length);
			}
			return zipEntry;
		}

		public override string ToString()
		{
			return name;
		}

		public static bool IsCompressionMethodSupported(CompressionMethod method)
		{
			if (method != CompressionMethod.Deflated)
			{
				return method == CompressionMethod.Stored;
			}
			return true;
		}

		public static string CleanName(string name)
		{
			if (name == null)
			{
				return string.Empty;
			}
			if (Path.IsPathRooted(name))
			{
				name = name.Substring(Path.GetPathRoot(name).Length);
			}
			name = name.Replace("\\", "/");
			while (name.Length > 0 && name[0] == '/')
			{
				name = name.Remove(0, 1);
			}
			return name;
		}
	}
}
