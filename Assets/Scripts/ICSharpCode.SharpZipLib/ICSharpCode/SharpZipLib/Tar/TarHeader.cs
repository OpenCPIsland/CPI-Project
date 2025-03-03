using System;
using System.Text;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarHeader : ICloneable
	{
		public const int NAMELEN = 100;

		public const int MODELEN = 8;

		public const int UIDLEN = 8;

		public const int GIDLEN = 8;

		public const int CHKSUMLEN = 8;

		public const int CHKSUMOFS = 148;

		public const int SIZELEN = 12;

		public const int MAGICLEN = 6;

		public const int VERSIONLEN = 2;

		public const int MODTIMELEN = 12;

		public const int UNAMELEN = 32;

		public const int GNAMELEN = 32;

		public const int DEVLEN = 8;

		public const byte LF_OLDNORM = 0;

		public const byte LF_NORMAL = 48;

		public const byte LF_LINK = 49;

		public const byte LF_SYMLINK = 50;

		public const byte LF_CHR = 51;

		public const byte LF_BLK = 52;

		public const byte LF_DIR = 53;

		public const byte LF_FIFO = 54;

		public const byte LF_CONTIG = 55;

		public const byte LF_GHDR = 103;

		public const byte LF_XHDR = 120;

		public const byte LF_ACL = 65;

		public const byte LF_GNU_DUMPDIR = 68;

		public const byte LF_EXTATTR = 69;

		public const byte LF_META = 73;

		public const byte LF_GNU_LONGLINK = 75;

		public const byte LF_GNU_LONGNAME = 76;

		public const byte LF_GNU_MULTIVOL = 77;

		public const byte LF_GNU_NAMES = 78;

		public const byte LF_GNU_SPARSE = 83;

		public const byte LF_GNU_VOLHDR = 86;

		public const string TMAGIC = "ustar ";

		public const string GNU_TMAGIC = "ustar  ";

		private const long timeConversionFactor = 10000000L;

		private static readonly DateTime dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		private string name;

		private int mode;

		private int userId;

		private int groupId;

		private long size;

		private DateTime modTime;

		private int checksum;

		private bool isChecksumValid;

		private byte typeFlag;

		private string linkName;

		private string magic;

		private string version;

		private string userName;

		private string groupName;

		private int devMajor;

		private int devMinor;

		internal static int userIdAsSet;

		internal static int groupIdAsSet;

		internal static string userNameAsSet;

		internal static string groupNameAsSet = "None";

		internal static int defaultUserId;

		internal static int defaultGroupId;

		internal static string defaultGroupName = "None";

		internal static string defaultUser;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				name = value;
			}
		}

		public int Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}

		public int UserId
		{
			get
			{
				return userId;
			}
			set
			{
				userId = value;
			}
		}

		public int GroupId
		{
			get
			{
				return groupId;
			}
			set
			{
				groupId = value;
			}
		}

		public long Size
		{
			get
			{
				return size;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Cannot be less than zero");
				}
				size = value;
			}
		}

		public DateTime ModTime
		{
			get
			{
				return modTime;
			}
			set
			{
				if (value < dateTime1970)
				{
					throw new ArgumentOutOfRangeException("value", "ModTime cannot be before Jan 1st 1970");
				}
				modTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
			}
		}

		public int Checksum
		{
			get
			{
				return checksum;
			}
		}

		public bool IsChecksumValid
		{
			get
			{
				return isChecksumValid;
			}
		}

		public byte TypeFlag
		{
			get
			{
				return typeFlag;
			}
			set
			{
				typeFlag = value;
			}
		}

		public string LinkName
		{
			get
			{
				return linkName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				linkName = value;
			}
		}

		public string Magic
		{
			get
			{
				return magic;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				magic = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				version = value;
			}
		}

		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				if (value != null)
				{
					userName = value.Substring(0, Math.Min(32, value.Length));
					return;
				}
				string text = "PocketPC";
				if (text.Length > 32)
				{
					text = text.Substring(0, 32);
				}
				userName = text;
			}
		}

		public string GroupName
		{
			get
			{
				return groupName;
			}
			set
			{
				if (value == null)
				{
					groupName = "None";
				}
				else
				{
					groupName = value;
				}
			}
		}

		public int DevMajor
		{
			get
			{
				return devMajor;
			}
			set
			{
				devMajor = value;
			}
		}

		public int DevMinor
		{
			get
			{
				return devMinor;
			}
			set
			{
				devMinor = value;
			}
		}

		public TarHeader()
		{
			Magic = "ustar ";
			Version = " ";
			Name = "";
			LinkName = "";
			UserId = defaultUserId;
			GroupId = defaultGroupId;
			UserName = defaultUser;
			GroupName = defaultGroupName;
			Size = 0L;
		}

		[Obsolete("Use the Name property instead", true)]
		public string GetName()
		{
			return name;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void ParseBuffer(byte[] header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			int num = 0;
			name = ParseName(header, num, 100).ToString();
			num += 100;
			mode = (int)ParseOctal(header, num, 8);
			num += 8;
			UserId = (int)ParseOctal(header, num, 8);
			num += 8;
			GroupId = (int)ParseOctal(header, num, 8);
			num += 8;
			Size = ParseOctal(header, num, 12);
			num += 12;
			ModTime = GetDateTimeFromCTime(ParseOctal(header, num, 12));
			num += 12;
			checksum = (int)ParseOctal(header, num, 8);
			num += 8;
			TypeFlag = header[num++];
			LinkName = ParseName(header, num, 100).ToString();
			num += 100;
			Magic = ParseName(header, num, 6).ToString();
			num += 6;
			Version = ParseName(header, num, 2).ToString();
			num += 2;
			UserName = ParseName(header, num, 32).ToString();
			num += 32;
			GroupName = ParseName(header, num, 32).ToString();
			num += 32;
			DevMajor = (int)ParseOctal(header, num, 8);
			num += 8;
			DevMinor = (int)ParseOctal(header, num, 8);
			isChecksumValid = (Checksum == MakeCheckSum(header));
		}

		public void WriteHeader(byte[] outBuffer)
		{
			if (outBuffer == null)
			{
				throw new ArgumentNullException("outBuffer");
			}
			int offset = 0;
			offset = GetNameBytes(Name, outBuffer, offset, 100);
			offset = GetOctalBytes(mode, outBuffer, offset, 8);
			offset = GetOctalBytes(UserId, outBuffer, offset, 8);
			offset = GetOctalBytes(GroupId, outBuffer, offset, 8);
			offset = GetLongOctalBytes(Size, outBuffer, offset, 12);
			offset = GetLongOctalBytes(GetCTime(ModTime), outBuffer, offset, 12);
			int offset2 = offset;
			for (int i = 0; i < 8; i++)
			{
				outBuffer[offset++] = 32;
			}
			outBuffer[offset++] = TypeFlag;
			offset = GetNameBytes(LinkName, outBuffer, offset, 100);
			offset = GetAsciiBytes(Magic, 0, outBuffer, offset, 6);
			offset = GetNameBytes(Version, outBuffer, offset, 2);
			offset = GetNameBytes(UserName, outBuffer, offset, 32);
			offset = GetNameBytes(GroupName, outBuffer, offset, 32);
			if (TypeFlag == 51 || TypeFlag == 52)
			{
				offset = GetOctalBytes(DevMajor, outBuffer, offset, 8);
				offset = GetOctalBytes(DevMinor, outBuffer, offset, 8);
			}
			while (offset < outBuffer.Length)
			{
				outBuffer[offset++] = 0;
			}
			checksum = ComputeCheckSum(outBuffer);
			GetCheckSumOctalBytes(checksum, outBuffer, offset2, 8);
			isChecksumValid = true;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			TarHeader tarHeader = obj as TarHeader;
			if (tarHeader != null)
			{
				return name == tarHeader.name && mode == tarHeader.mode && UserId == tarHeader.UserId && GroupId == tarHeader.GroupId && Size == tarHeader.Size && ModTime == tarHeader.ModTime && Checksum == tarHeader.Checksum && TypeFlag == tarHeader.TypeFlag && LinkName == tarHeader.LinkName && Magic == tarHeader.Magic && Version == tarHeader.Version && UserName == tarHeader.UserName && GroupName == tarHeader.GroupName && DevMajor == tarHeader.DevMajor && DevMinor == tarHeader.DevMinor;
			}
			return false;
		}

		internal static void SetValueDefaults(int userId, string userName, int groupId, string groupName)
		{
			defaultUserId = (userIdAsSet = userId);
			defaultUser = (userNameAsSet = userName);
			defaultGroupId = (groupIdAsSet = groupId);
			defaultGroupName = (groupNameAsSet = groupName);
		}

		internal static void RestoreSetValues()
		{
			defaultUserId = userIdAsSet;
			defaultUser = userNameAsSet;
			defaultGroupId = groupIdAsSet;
			defaultGroupName = groupNameAsSet;
		}

		public static long ParseOctal(byte[] header, int offset, int length)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			long num = 0L;
			bool flag = true;
			int num2 = offset + length;
			for (int i = offset; i < num2 && header[i] != 0; i++)
			{
				if (header[i] == 32 || header[i] == 48)
				{
					if (flag)
					{
						continue;
					}
					if (header[i] == 32)
					{
						break;
					}
				}
				flag = false;
				num = (num << 3) + (header[i] - 48);
			}
			return num;
		}

		public static StringBuilder ParseName(byte[] header, int offset, int length)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot be less than zero");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Cannot be less than zero");
			}
			if (offset + length > header.Length)
			{
				throw new ArgumentException("Exceeds header size", "length");
			}
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = offset; i < offset + length && header[i] != 0; i++)
			{
				stringBuilder.Append((char)header[i]);
			}
			return stringBuilder;
		}

		public static int GetNameBytes(StringBuilder name, int nameOffset, byte[] buffer, int bufferOffset, int length)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return GetNameBytes(name.ToString(), nameOffset, buffer, bufferOffset, length);
		}

		public static int GetNameBytes(string name, int nameOffset, byte[] buffer, int bufferOffset, int length)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int i;
			for (i = 0; i < length - 1 && nameOffset + i < name.Length; i++)
			{
				buffer[bufferOffset + i] = (byte)name[nameOffset + i];
			}
			for (; i < length; i++)
			{
				buffer[bufferOffset + i] = 0;
			}
			return bufferOffset + length;
		}

		public static int GetNameBytes(StringBuilder name, byte[] buffer, int offset, int length)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return GetNameBytes(name.ToString(), 0, buffer, offset, length);
		}

		public static int GetNameBytes(string name, byte[] buffer, int offset, int length)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return GetNameBytes(name, 0, buffer, offset, length);
		}

		public static int GetAsciiBytes(string toAdd, int nameOffset, byte[] buffer, int bufferOffset, int length)
		{
			if (toAdd == null)
			{
				throw new ArgumentNullException("toAdd");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			for (int i = 0; i < length && nameOffset + i < toAdd.Length; i++)
			{
				buffer[bufferOffset + i] = (byte)toAdd[nameOffset + i];
			}
			return bufferOffset + length;
		}

		public static int GetOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = length - 1;
			buffer[offset + num] = 0;
			num--;
			if (value > 0)
			{
				long num2 = value;
				while (num >= 0 && num2 > 0)
				{
					buffer[offset + num] = (byte)(48 + (byte)(num2 & 7));
					num2 >>= 3;
					num--;
				}
			}
			while (num >= 0)
			{
				buffer[offset + num] = 48;
				num--;
			}
			return offset + length;
		}

		public static int GetLongOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			return GetOctalBytes(value, buffer, offset, length);
		}

		private static int GetCheckSumOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			GetOctalBytes(value, buffer, offset, length - 1);
			return offset + length;
		}

		private static int ComputeCheckSum(byte[] buffer)
		{
			int num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				num += buffer[i];
			}
			return num;
		}

		private static int MakeCheckSum(byte[] buffer)
		{
			int num = 0;
			for (int i = 0; i < 148; i++)
			{
				num += buffer[i];
			}
			for (int j = 0; j < 8; j++)
			{
				num += 32;
			}
			for (int k = 156; k < buffer.Length; k++)
			{
				num += buffer[k];
			}
			return num;
		}

		private static int GetCTime(DateTime dateTime)
		{
			return (int)((dateTime.Ticks - dateTime1970.Ticks) / 10000000);
		}

		private static DateTime GetDateTimeFromCTime(long ticks)
		{
			try
			{
				return new DateTime(dateTime1970.Ticks + ticks * 10000000);
			}
			catch (ArgumentOutOfRangeException)
			{
				return dateTime1970;
			}
		}
	}
}
