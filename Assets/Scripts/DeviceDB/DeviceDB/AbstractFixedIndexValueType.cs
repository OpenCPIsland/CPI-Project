using System;
using System.IO;

namespace DeviceDB
{
	internal abstract class AbstractFixedIndexValueType<TEntryValue> : IFixedSizeIndexValueType<TEntryValue>, IIndexValueType<TEntryValue>, IDisposable where TEntryValue : IComparable<TEntryValue>
	{
		public uint Size
		{
			get
			{
				return 16u;
			}
		}

		public bool ChangeEncryptor(Aes256Encryptor oldEncryptor, Aes256Encryptor newEncryptor)
		{
			return true;
		}

		public void WriteEntryValue(TEntryValue entryValue, BinaryWriter writer, Aes256Encryptor encryptor)
		{
			byte[] bytes = Serialize(entryValue);
			byte[] buffer = encryptor.Encrypt(bytes);
			writer.Write(buffer);
		}

		public void UpdateEntryValue(TEntryValue entryValue, uint metadata, BinaryWriter writer, Aes256Encryptor encryptor)
		{
			byte[] bytes = Serialize(entryValue);
			byte[] buffer = encryptor.Encrypt(bytes);
			writer.Write(buffer);
		}

		public uint ReadEntryMetadata(BinaryReader reader)
		{
			return 0u;
		}

		public void ReadEntryValue(BinaryReader reader, Aes256Encryptor encryptor, out TEntryValue value, out uint metadata)
		{
			byte[] bytes = reader.ReadBytes((int)Size);
			byte[] bytes2 = encryptor.Decrypt(bytes);
			value = Deserialize(bytes2);
			metadata = 0u;
		}

		public void Dispose()
		{
		}

		public int Compare(TEntryValue value1, TEntryValue value2)
		{
			return value1.CompareTo(value2);
		}

		public void Remove(BinaryReader reader)
		{
		}

		public void Clear()
		{
		}

		public void JournaledClear()
		{
		}

		public void Delete()
		{
		}

		protected abstract byte[] Serialize(TEntryValue entryValue);

		protected abstract TEntryValue Deserialize(byte[] bytes);
	}
}
