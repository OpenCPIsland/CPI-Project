using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public sealed class ZipExtraData : IDisposable
	{
		private int _index;

		private int _readValueStart;

		private int _readValueLength;

		private MemoryStream _newEntry;

		private byte[] _data;

		public int Length
		{
			get
			{
				return _data.Length;
			}
		}

		public int ValueLength
		{
			get
			{
				return _readValueLength;
			}
		}

		public int CurrentReadIndex
		{
			get
			{
				return _index;
			}
		}

		public int UnreadCount
		{
			get
			{
				if (_readValueStart > _data.Length || _readValueStart < 4)
				{
					throw new ZipException("Find must be called before calling a Read method");
				}
				return _readValueStart + _readValueLength - _index;
			}
		}

		public ZipExtraData()
		{
			Clear();
		}

		public ZipExtraData(byte[] data)
		{
			if (data == null)
			{
				_data = new byte[0];
			}
			else
			{
				_data = data;
			}
		}

		public byte[] GetEntryData()
		{
			if (Length > 65535)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			return (byte[])_data.Clone();
		}

		public void Clear()
		{
			if (_data == null || _data.Length != 0)
			{
				_data = new byte[0];
			}
		}

		public Stream GetStreamForTag(int tag)
		{
			Stream result = null;
			if (Find(tag))
			{
				result = new MemoryStream(_data, _index, _readValueLength, false);
			}
			return result;
		}

		private ITaggedData GetData(short tag)
		{
			ITaggedData result = null;
			if (Find(tag))
			{
				result = Create(tag, _data, _readValueStart, _readValueLength);
			}
			return result;
		}

		private static ITaggedData Create(short tag, byte[] data, int offset, int count)
		{
			ITaggedData taggedData = null;
			switch (tag)
			{
			case 10:
				taggedData = new NTTaggedData();
				break;
			case 21589:
				taggedData = new ExtendedUnixData();
				break;
			default:
				taggedData = new RawTaggedData(tag);
				break;
			}
			taggedData.SetData(data, offset, count);
			return taggedData;
		}

		public bool Find(int headerID)
		{
			_readValueStart = _data.Length;
			_readValueLength = 0;
			_index = 0;
			int num = _readValueStart;
			int num2 = headerID - 1;
			while (num2 != headerID && _index < _data.Length - 3)
			{
				num2 = ReadShortInternal();
				num = ReadShortInternal();
				if (num2 != headerID)
				{
					_index += num;
				}
			}
			bool flag = num2 == headerID && _index + num <= _data.Length;
			if (flag)
			{
				_readValueStart = _index;
				_readValueLength = num;
			}
			return flag;
		}

		public void AddEntry(ITaggedData taggedData)
		{
			if (taggedData == null)
			{
				throw new ArgumentNullException("taggedData");
			}
			AddEntry(taggedData.TagID, taggedData.GetData());
		}

		public void AddEntry(int headerID, byte[] fieldData)
		{
			if (headerID > 65535 || headerID < 0)
			{
				throw new ArgumentOutOfRangeException("headerID");
			}
			int num = (fieldData != null) ? fieldData.Length : 0;
			if (num > 65535)
			{
				throw new ArgumentOutOfRangeException("fieldData", "exceeds maximum length");
			}
			int num2 = _data.Length + num + 4;
			if (Find(headerID))
			{
				num2 -= ValueLength + 4;
			}
			if (num2 > 65535)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			Delete(headerID);
			byte[] array = new byte[num2];
			_data.CopyTo(array, 0);
			int index = _data.Length;
			_data = array;
			SetShort(ref index, headerID);
			SetShort(ref index, num);
			if (fieldData != null)
			{
				fieldData.CopyTo(array, index);
			}
		}

		public void StartNewEntry()
		{
			_newEntry = new MemoryStream();
		}

		public void AddNewEntry(int headerID)
		{
			byte[] fieldData = _newEntry.ToArray();
			_newEntry = null;
			AddEntry(headerID, fieldData);
		}

		public void AddData(byte data)
		{
			_newEntry.WriteByte(data);
		}

		public void AddData(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			_newEntry.Write(data, 0, data.Length);
		}

		public void AddLeShort(int toAdd)
		{
			_newEntry.WriteByte((byte)toAdd);
			_newEntry.WriteByte((byte)(toAdd >> 8));
		}

		public void AddLeInt(int toAdd)
		{
			AddLeShort((short)toAdd);
			AddLeShort((short)(toAdd >> 16));
		}

		public void AddLeLong(long toAdd)
		{
			AddLeInt((int)(toAdd & uint.MaxValue));
			AddLeInt((int)(toAdd >> 32));
		}

		public bool Delete(int headerID)
		{
			bool result = false;
			if (Find(headerID))
			{
				result = true;
				int num = _readValueStart - 4;
				byte[] array = new byte[_data.Length - (ValueLength + 4)];
				Array.Copy(_data, 0, array, 0, num);
				int num2 = num + ValueLength + 4;
				Array.Copy(_data, num2, array, num, _data.Length - num2);
				_data = array;
			}
			return result;
		}

		public long ReadLong()
		{
			ReadCheck(8);
			return (ReadInt() & uint.MaxValue) | ((long)ReadInt() << 32);
		}

		public int ReadInt()
		{
			ReadCheck(4);
			int result = _data[_index] + (_data[_index + 1] << 8) + (_data[_index + 2] << 16) + (_data[_index + 3] << 24);
			_index += 4;
			return result;
		}

		public int ReadShort()
		{
			ReadCheck(2);
			int result = _data[_index] + (_data[_index + 1] << 8);
			_index += 2;
			return result;
		}

		public int ReadByte()
		{
			int result = -1;
			if (_index < _data.Length && _readValueStart + _readValueLength > _index)
			{
				result = _data[_index];
				_index++;
			}
			return result;
		}

		public void Skip(int amount)
		{
			ReadCheck(amount);
			_index += amount;
		}

		private void ReadCheck(int length)
		{
			if (_readValueStart > _data.Length || _readValueStart < 4)
			{
				throw new ZipException("Find must be called before calling a Read method");
			}
			if (_index > _readValueStart + _readValueLength - length)
			{
				throw new ZipException("End of extra data");
			}
			if (_index + length < 4)
			{
				throw new ZipException("Cannot read before start of tag");
			}
		}

		private int ReadShortInternal()
		{
			if (_index > _data.Length - 2)
			{
				throw new ZipException("End of extra data");
			}
			int result = _data[_index] + (_data[_index + 1] << 8);
			_index += 2;
			return result;
		}

		private void SetShort(ref int index, int source)
		{
			_data[index] = (byte)source;
			_data[index + 1] = (byte)(source >> 8);
			index += 2;
		}

		public void Dispose()
		{
			if (_newEntry != null)
			{
				_newEntry.Close();
			}
		}
	}
}
