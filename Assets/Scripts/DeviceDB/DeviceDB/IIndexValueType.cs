using System;
using System.IO;

namespace DeviceDB
{
	internal interface IIndexValueType<TEntryValue> : IDisposable where TEntryValue : IComparable<TEntryValue>
	{
		bool ChangeEncryptor(Aes256Encryptor oldEncryptor, Aes256Encryptor newEncryptor);

		void WriteEntryValue(TEntryValue entryValue, BinaryWriter writer, Aes256Encryptor encryptor);

		void UpdateEntryValue(TEntryValue entryValue, uint metadata, BinaryWriter writer, Aes256Encryptor encryptor);

		uint ReadEntryMetadata(BinaryReader reader);

		void ReadEntryValue(BinaryReader reader, Aes256Encryptor encryptor, out TEntryValue value, out uint metadata);

		void Remove(BinaryReader reader);

		void Clear();

		void JournaledClear();

		void Delete();

		int Compare(TEntryValue value1, TEntryValue value2);
	}
}
