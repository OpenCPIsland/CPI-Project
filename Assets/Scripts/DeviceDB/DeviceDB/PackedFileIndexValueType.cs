using System;
using System.Collections.Generic;
using System.IO;

namespace DeviceDB
{
	internal class PackedFileIndexValueType<TEntryValue> : IFixedSizeIndexValueType<TEntryValue>, IIndexValueType<TEntryValue>, IDisposable where TEntryValue : IComparable<TEntryValue>
	{
		private readonly IIndexValueType<TEntryValue> indexValueType;

		private readonly PackedFile packedFile;

		private readonly MemoryStream serializationStream;

		private readonly BinaryReader serializationReader;

		private readonly BinaryWriter serializationWriter;

		public uint Size
		{
			get
			{
				return 4u;
			}
		}

		public PackedFileIndexValueType(IIndexValueType<TEntryValue> indexValueType, PackedFile packedFile)
		{
			this.indexValueType = indexValueType;
			this.packedFile = packedFile;
			serializationStream = new MemoryStream();
			serializationReader = new BinaryReader(serializationStream);
			serializationWriter = new BinaryWriter(serializationStream);
		}

		public bool ChangeEncryptor(Aes256Encryptor oldEncryptor, Aes256Encryptor newEncryptor)
		{
			foreach (KeyValuePair<uint, byte[]> document2 in packedFile.Documents)
			{
				byte[] value = document2.Value;
				byte[] bytes = oldEncryptor.Decrypt(value);
				byte[] document = newEncryptor.Encrypt(bytes);
				packedFile.Update(document2.Key, document);
			}
			return false;
		}

		public void WriteEntryValue(TEntryValue entryValue, BinaryWriter writer, Aes256Encryptor encryptor)
		{
			serializationStream.Position = 0L;
			indexValueType.WriteEntryValue(entryValue, serializationWriter, encryptor);
			byte[] document = encryptor.Encrypt(serializationStream.ToArray());
			uint value = packedFile.Insert(document);
			writer.Write(value);
		}

		public void UpdateEntryValue(TEntryValue entryValue, uint metadata, BinaryWriter writer, Aes256Encryptor encryptor)
		{
			serializationStream.Position = 0L;
			indexValueType.WriteEntryValue(entryValue, serializationWriter, encryptor);
			byte[] document = encryptor.Encrypt(serializationStream.ToArray());
			uint value = packedFile.Update(metadata, document);
			writer.Write(value);
		}

		public uint ReadEntryMetadata(BinaryReader reader)
		{
			return reader.ReadUInt32();
		}

		public void ReadEntryValue(BinaryReader reader, Aes256Encryptor encryptor, out TEntryValue value, out uint metadata)
		{
			metadata = reader.ReadUInt32();
			byte[] bytes = packedFile.Find(metadata);
			byte[] array = encryptor.Decrypt(bytes);
			serializationStream.Position = 0L;
			serializationStream.Write(array, 0, array.Length);
			serializationStream.Position = 0L;
			uint metadata2;
			indexValueType.ReadEntryValue(serializationReader, encryptor, out value, out metadata2);
		}

		public void Dispose()
		{
			packedFile.Dispose();
		}

		public int Compare(TEntryValue value1, TEntryValue value2)
		{
			return indexValueType.Compare(value1, value2);
		}

		public void Remove(BinaryReader reader)
		{
			uint documentId = reader.ReadUInt32();
			packedFile.Remove(documentId);
		}

		public void Clear()
		{
			packedFile.Clear();
		}

		public void JournaledClear()
		{
			packedFile.JournaledClear();
		}

		public void Delete()
		{
			packedFile.Delete();
		}
	}
}
