using System;
using System.IO;

namespace DeviceDB
{
	internal class StringIndexValueType : IIndexValueType<string>, IDisposable
	{
		public bool ChangeEncryptor(Aes256Encryptor oldEncryptor, Aes256Encryptor newEncryptor)
		{
			return true;
		}

		public void WriteEntryValue(string entryValue, BinaryWriter writer, Aes256Encryptor encryptor)
		{
			if (entryValue == null)
			{
				writer.Write(true);
				return;
			}
			writer.Write(false);
			writer.Write(entryValue);
		}

		public void UpdateEntryValue(string entryValue, uint metadata, BinaryWriter writer, Aes256Encryptor encryptor)
		{
			if (entryValue == null)
			{
				writer.Write(true);
				return;
			}
			writer.Write(false);
			writer.Write(entryValue);
		}

		public uint ReadEntryMetadata(BinaryReader reader)
		{
			return 0u;
		}

		public void ReadEntryValue(BinaryReader reader, Aes256Encryptor encryptor, out string value, out uint metadata)
		{
			bool flag = reader.ReadBoolean();
			value = ((!flag) ? reader.ReadString() : null);
			metadata = 0u;
		}

		public void Dispose()
		{
		}

		public int Compare(string value1, string value2)
		{
			return NullSafeComparer.Compare(value1, value2);
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
	}
}
