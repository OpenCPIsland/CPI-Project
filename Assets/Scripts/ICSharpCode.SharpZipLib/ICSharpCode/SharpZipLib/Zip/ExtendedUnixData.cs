using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ExtendedUnixData : ITaggedData
	{
		[Flags]
		public enum Flags : byte
		{
			ModificationTime = 0x1,
			AccessTime = 0x2,
			CreateTime = 0x4
		}

		private Flags _flags;

		private DateTime _modificationTime = new DateTime(1970, 1, 1);

		private DateTime _lastAccessTime = new DateTime(1970, 1, 1);

		private DateTime _createTime = new DateTime(1970, 1, 1);

		public short TagID
		{
			get
			{
				return 21589;
			}
		}

		public DateTime ModificationTime
		{
			get
			{
				return _modificationTime;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_flags |= Flags.ModificationTime;
				_modificationTime = value;
			}
		}

		public DateTime AccessTime
		{
			get
			{
				return _lastAccessTime;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_flags |= Flags.AccessTime;
				_lastAccessTime = value;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return _createTime;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_flags |= Flags.CreateTime;
				_createTime = value;
			}
		}

		private Flags Include
		{
			get
			{
				return _flags;
			}
			set
			{
				_flags = value;
			}
		}

		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream stream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(stream))
				{
					_flags = (Flags)zipHelperStream.ReadByte();
					if ((_flags & Flags.ModificationTime) != 0 && count >= 5)
					{
						int seconds = zipHelperStream.ReadLEInt();
						_modificationTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
					}
					if ((_flags & Flags.AccessTime) != 0)
					{
						int seconds2 = zipHelperStream.ReadLEInt();
						_lastAccessTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds2, 0)).ToLocalTime();
					}
					if ((_flags & Flags.CreateTime) != 0)
					{
						int seconds3 = zipHelperStream.ReadLEInt();
						_createTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds3, 0)).ToLocalTime();
					}
				}
			}
		}

		public byte[] GetData()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.IsStreamOwner = false;
					zipHelperStream.WriteByte((byte)_flags);
					if ((_flags & Flags.ModificationTime) != 0)
					{
						int value = (int)(_modificationTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value);
					}
					if ((_flags & Flags.AccessTime) != 0)
					{
						int value2 = (int)(_lastAccessTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value2);
					}
					if ((_flags & Flags.CreateTime) != 0)
					{
						int value3 = (int)(_createTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value3);
					}
					return memoryStream.ToArray();
				}
			}
		}

		public static bool IsValidValue(DateTime value)
		{
			if (!(value >= new DateTime(1901, 12, 13, 20, 45, 52)))
			{
				return value <= new DateTime(2038, 1, 19, 3, 14, 7);
			}
			return true;
		}
	}
}
