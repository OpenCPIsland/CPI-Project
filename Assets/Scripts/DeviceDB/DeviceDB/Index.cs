using System;
using System.Collections.Generic;
using System.IO;

namespace DeviceDB
{
	internal class Index<TEntryValue> where TEntryValue : IComparable<TEntryValue>
	{
		private const uint IdSize = 4u;

		private const uint FileHeaderSize = 1u;

		private const byte FormatVersion = 0;

		private readonly string path;

		private readonly Stream fileStream;

		private readonly BinaryReader fileStreamReader;

		private readonly byte[] serializedBytes;

		private readonly MemoryStream serializerStream;

		private readonly BinaryWriter serializerWriter;

		private readonly uint entrySize;

		private readonly IFixedSizeIndexValueType<TEntryValue> valueType;

		private Aes256Encryptor encryptor;

		private readonly IFileSystem fileSystem;

		private readonly JournalWriter journalWriter;

		public uint Count
		{
			get;
			private set;
		}

		public Index(string path, IFixedSizeIndexValueType<TEntryValue> valueType, Aes256Encryptor encryptor, IFileSystem fileSystem, JournalWriter journalWriter)
		{
			this.path = path;
			this.valueType = valueType;
			this.encryptor = encryptor;
			this.fileSystem = fileSystem;
			this.journalWriter = journalWriter;
			uint size = valueType.Size;
			entrySize = 4 + size;
			fileStream = fileSystem.OpenFileStream(path);
			serializedBytes = new byte[entrySize];
			serializerStream = new MemoryStream(serializedBytes);
			serializerWriter = new BinaryWriter(serializerStream);
			if (fileStream.Length >= 1)
			{
				int num = fileStream.ReadByte();
				if (num != 0)
				{
					throw new CorruptionException("Unsupported format version: " + num);
				}
				uint num2 = (uint)(fileStream.Length - 1);
				Count = num2 / entrySize;
				if (num2 % entrySize != 0)
				{
					uint entryPosition = GetEntryPosition(Count);
					fileStream.SetLength(entryPosition);
				}
			}
			else
			{
				fileStream.SetLength(1L);
				fileStream.WriteByte(0);
			}
			fileStreamReader = new BinaryReader(fileStream);
		}

		public void Insert(uint entryId, TEntryValue value)
		{
			SerializeNewEntry(entryId, value, encryptor);
			uint valueInsertionIndex = GetValueInsertionIndex(value);
			uint entryPosition = GetEntryPosition(valueInsertionIndex);
			uint num = (uint)fileStream.Length;
			uint num2 = num + entrySize;
			if (entryPosition < num)
			{
				uint num3 = num - entryPosition;
				uint num4 = num2 - 32768;
				uint num5 = num - 32768;
				uint num6 = num3 / 32768u;
				uint num7 = num3 % 32768u;
				for (uint num8 = 0u; num8 < num6; num8++)
				{
					journalWriter.WriteCopyEntry(path, num5, 32768u, num4);
					num4 -= 32768;
					num5 -= 32768;
				}
				if (num7 != 0)
				{
					uint destPosition = entryPosition + entrySize;
					journalWriter.WriteCopyEntry(path, entryPosition, num7, destPosition);
				}
			}
			journalWriter.WriteWriteEntry(path, entryPosition, serializedBytes);
			Count++;
		}

		public void Update(uint entryId, uint oldEntryId, TEntryValue value, TEntryValue oldValue)
		{
			int entryIndex = GetEntryIndex(oldEntryId, oldValue);
			if (entryIndex < 0)
			{
				throw new ArgumentException("Entry ID " + oldEntryId + " not found");
			}
			uint num = GetValueInsertionIndex(value);
			if (num != 0 && num > entryIndex)
			{
				num--;
			}
			uint entryPosition = GetEntryPosition(num);
			uint entryPosition2 = GetEntryPosition((uint)entryIndex);
			fileStream.Position = entryPosition2 + 4;
			uint entryMetadata = valueType.ReadEntryMetadata(fileStreamReader);
			if (entryIndex < num)
			{
				uint num2 = entryPosition - entryPosition2;
				uint num3 = num2 / 32768u;
				uint num4 = num2 % 32768u;
				uint num5 = entryPosition2;
				uint num6 = num5 + entrySize;
				for (uint num7 = 0u; num7 < num3; num7++)
				{
					journalWriter.WriteCopyEntry(path, num6, 32768u, num5);
					num5 += 32768;
					num6 += 32768;
				}
				if (num4 != 0)
				{
					uint srcPosition = num5 + entrySize;
					journalWriter.WriteCopyEntry(path, srcPosition, num4, num5);
				}
			}
			else if (entryIndex > num)
			{
				uint num8 = entryPosition2 - entryPosition;
				uint num9 = num8 / 32768u;
				uint num10 = num8 % 32768u;
				uint num11 = entryPosition2 + entrySize - 32768;
				uint num12 = entryPosition2 - 32768;
				for (uint num13 = 0u; num13 < num9; num13++)
				{
					journalWriter.WriteCopyEntry(path, num12, 32768u, num11);
					num11 -= 32768;
					num12 -= 32768;
				}
				if (num10 != 0)
				{
					uint destPosition = entryPosition + entrySize;
					journalWriter.WriteCopyEntry(path, entryPosition, num10, destPosition);
				}
			}
			SerializeUpdatedEntry(entryId, value, entryMetadata, encryptor);
			journalWriter.WriteWriteEntry(path, entryPosition, serializedBytes);
		}

		public void Update(uint entryId, uint oldEntryId, TEntryValue oldValue)
		{
			int entryIndex = GetEntryIndex(oldEntryId, oldValue);
			if (entryIndex < 0)
			{
				throw new ArgumentException("Entry ID " + oldEntryId + " not found");
			}
			uint entryPosition = GetEntryPosition((uint)entryIndex);
			serializerStream.Position = 0L;
			serializerWriter.Write(entryId);
			journalWriter.WriteWriteEntry(path, entryPosition, serializedBytes, 4u);
		}

		public void Remove(uint entryId, TEntryValue value)
		{
			int entryIndex = GetEntryIndex(entryId, value);
			if (entryIndex < 0)
			{
				throw new ArgumentException("Entry ID " + entryId + " not found");
			}
			uint entryPosition = GetEntryPosition((uint)entryIndex);
			fileStream.Position = entryPosition + 4;
			valueType.Remove(fileStreamReader);
			uint num = (uint)fileStream.Length;
			uint num2 = num - entryPosition - entrySize;
			uint num3 = num2 / 32768u;
			uint num4 = num2 % 32768u;
			uint num5 = entryPosition;
			uint num6 = num5 + entrySize;
			for (uint num7 = 0u; num7 < num3; num7++)
			{
				journalWriter.WriteCopyEntry(path, num6, 32768u, num5);
				num5 += 32768;
				num6 += 32768;
			}
			if (num4 != 0)
			{
				uint srcPosition = num5 + entrySize;
				journalWriter.WriteCopyEntry(path, srcPosition, num4, num5);
			}
			uint size = num - entrySize;
			journalWriter.WriteResizeEntry(path, size);
			Count--;
		}

		public IEnumerable<KeyValuePair<uint, TEntryValue>> Find(TEntryValue value, Predicate<TEntryValue> matcher)
		{
			uint index = GetValueInsertionIndex(value);
			if (index < Count)
			{
				uint readEntryId3;
				TEntryValue readEntryValue3;
				uint readEntryMetadata3;
				ReadEntryAtIndex(index, out readEntryId3, out readEntryValue3, out readEntryMetadata3);
				if (matcher(readEntryValue3))
				{
					yield return new KeyValuePair<uint, TEntryValue>(readEntryId3, readEntryValue3);
				}
			}
			if (index != 0)
			{
				uint j = index - 1;
				while (true)
				{
					uint readEntryId2;
					TEntryValue readEntryValue2;
					uint readEntryMetadata2;
					ReadEntryAtIndex(j, out readEntryId2, out readEntryValue2, out readEntryMetadata2);
					if (!matcher(readEntryValue2))
					{
						break;
					}
					yield return new KeyValuePair<uint, TEntryValue>(readEntryId2, readEntryValue2);
					if (j == 0)
					{
						break;
					}
					j--;
				}
			}
			for (uint i = index + 1; i < Count; i++)
			{
				uint readEntryId;
				TEntryValue readEntryValue;
				uint readEntryMetadata;
				ReadEntryAtIndex(i, out readEntryId, out readEntryValue, out readEntryMetadata);
				if (!matcher(readEntryValue))
				{
					break;
				}
				yield return new KeyValuePair<uint, TEntryValue>(readEntryId, readEntryValue);
			}
		}

		public uint? FindMaxId()
		{
			if (Count == 0)
			{
				return null;
			}
			return ReadEntryIdAtIndex(Count - 1);
		}

		public uint? FindMinId()
		{
			if (Count == 0)
			{
				return null;
			}
			return ReadEntryIdAtIndex(0u);
		}

		public KeyValuePair<uint, TEntryValue>? FindMax()
		{
			if (Count == 0)
			{
				return null;
			}
			uint entryId;
			TEntryValue entryValue;
			uint entryMetadata;
			ReadEntryAtIndex(Count - 1, out entryId, out entryValue, out entryMetadata);
			return new KeyValuePair<uint, TEntryValue>(entryId, entryValue);
		}

		public KeyValuePair<uint, TEntryValue>? FindMin()
		{
			if (Count == 0)
			{
				return null;
			}
			uint entryId;
			TEntryValue entryValue;
			uint entryMetadata;
			ReadEntryAtIndex(0u, out entryId, out entryValue, out entryMetadata);
			return new KeyValuePair<uint, TEntryValue>(entryId, entryValue);
		}

		public void ChangeEncryptor(Aes256Encryptor encryptor)
		{
			if (valueType.ChangeEncryptor(this.encryptor, encryptor))
			{
				uint num = 1u;
				fileStream.Position = num;
				for (uint num2 = 0u; num2 < Count; num2++)
				{
					uint entryId;
					TEntryValue entryValue;
					uint entryMetadata;
					ReadEntry(this.encryptor, out entryId, out entryValue, out entryMetadata);
					SerializeUpdatedEntry(entryId, entryValue, entryMetadata, encryptor);
					journalWriter.WriteWriteEntry(path, num, serializedBytes);
					num += entrySize;
				}
			}
			this.encryptor = encryptor;
		}

		public void Clear()
		{
			valueType.Clear();
			fileStream.SetLength(1L);
			Count = 0u;
		}

		public void JournaledClear()
		{
			valueType.JournaledClear();
			journalWriter.WriteResizeEntry(path, 1u);
			Count = 0u;
		}

		public void Delete()
		{
			Dispose();
			valueType.Delete();
			fileSystem.DeleteFile(path);
		}

		public void Dispose()
		{
			fileStreamReader.Close();
			valueType.Dispose();
		}

		public void InsertPreSorted(IEnumerable<KeyValuePair<uint, IComparable>> entries)
		{
			uint num = GetEntryPosition(0u);
			fileStream.Position = num;
			foreach (KeyValuePair<uint, IComparable> entry in entries)
			{
				SerializeNewEntry(entry.Key, (TEntryValue)entry.Value, encryptor);
				fileStream.Write(serializedBytes, 0, (int)entrySize);
				num += entrySize;
				Count++;
			}
		}

		private uint ReadEntryIdAtIndex(uint index)
		{
			uint entryPosition = GetEntryPosition(index);
			fileStream.Position = entryPosition;
			return fileStreamReader.ReadUInt32();
		}

		private void ReadEntryAtIndex(uint index, out uint entryId, out TEntryValue entryValue, out uint entryMetadata)
		{
			uint entryPosition = GetEntryPosition(index);
			fileStream.Position = entryPosition;
			ReadEntry(encryptor, out entryId, out entryValue, out entryMetadata);
		}

		private void ReadEntry(Aes256Encryptor encryptor, out uint entryId, out TEntryValue entryValue, out uint entryMetadata)
		{
			entryId = fileStreamReader.ReadUInt32();
			valueType.ReadEntryValue(fileStreamReader, encryptor, out entryValue, out entryMetadata);
		}

		private uint GetEntryPosition(uint index)
		{
			return 1 + index * entrySize;
		}

		private void SerializeNewEntry(uint entryId, TEntryValue value, Aes256Encryptor encryptor)
		{
			serializerStream.Position = 0L;
			serializerWriter.Write(entryId);
			valueType.WriteEntryValue(value, serializerWriter, encryptor);
		}

		private void SerializeUpdatedEntry(uint entryId, TEntryValue value, uint entryMetadata, Aes256Encryptor encryptor)
		{
			serializerStream.Position = 0L;
			serializerWriter.Write(entryId);
			valueType.UpdateEntryValue(value, entryMetadata, serializerWriter, encryptor);
		}

		private uint GetValueInsertionIndex(TEntryValue value)
		{
			uint num = 0u;
			uint num2 = Count;
			while (num2 > num)
			{
				uint num3 = num2 - num;
				uint num4 = num + num3 / 2u;
				uint num5 = 1 + num4 * entrySize + 4;
				fileStream.Position = num5;
				TEntryValue value2;
				uint metadata;
				valueType.ReadEntryValue(fileStreamReader, encryptor, out value2, out metadata);
				int num6 = valueType.Compare(value2, value);
				if (num6 == 0)
				{
					return num4;
				}
				if (num6 < 0)
				{
					num = num4 + 1;
				}
				else
				{
					num2 = num4;
				}
			}
			return num;
		}

		private int GetEntryIndex(uint entryId, TEntryValue value)
		{
			uint valueInsertionIndex = GetValueInsertionIndex(value);
			if (valueInsertionIndex < Count)
			{
				uint num = ReadEntryIdAtIndex(valueInsertionIndex);
				if (num == entryId)
				{
					return (int)valueInsertionIndex;
				}
			}
			if (valueInsertionIndex != 0)
			{
				uint num2 = valueInsertionIndex - 1;
				while (true)
				{
					uint entryId2;
					TEntryValue entryValue;
					uint entryMetadata;
					ReadEntryAtIndex(num2, out entryId2, out entryValue, out entryMetadata);
					if (entryId2 == entryId)
					{
						return (int)num2;
					}
					if (valueType.Compare(entryValue, value) != 0 || num2 == 0)
					{
						break;
					}
					num2--;
				}
			}
			for (uint num3 = valueInsertionIndex + 1; num3 < Count; num3++)
			{
				uint entryId3;
				TEntryValue entryValue2;
				uint entryMetadata2;
				ReadEntryAtIndex(num3, out entryId3, out entryValue2, out entryMetadata2);
				if (entryId3 == entryId)
				{
					return (int)num3;
				}
				if (valueType.Compare(entryValue2, value) != 0)
				{
					break;
				}
			}
			return -1;
		}
	}
}
