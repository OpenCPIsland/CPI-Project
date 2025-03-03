using System;

namespace DeviceDB
{
	internal class IndexFactory
	{
		private readonly string dir;

		private readonly JournalWriter journalWriter;

		private readonly IFileSystem fileSystem;

		public IndexFactory(string dir, JournalWriter journalWriter, IFileSystem fileSystem)
		{
			this.dir = dir;
			this.journalWriter = journalWriter;
			this.fileSystem = fileSystem;
		}

		public Index<TValue> Create<TValue>(string fieldName, Aes256Encryptor encryptor) where TValue : IComparable<TValue>
		{
			string path = HashedPathGenerator.GetPath(dir, fieldName);
			Type typeFromHandle = typeof(TValue);
			if (typeFromHandle == typeof(bool))
			{
				BoolIndexValueType valueType = new BoolIndexValueType();
				return (Index<TValue>)(object)new Index<bool>(path, valueType, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(sbyte))
			{
				SByteIndexValueType valueType2 = new SByteIndexValueType();
				return (Index<TValue>)(object)new Index<sbyte>(path, valueType2, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(byte))
			{
				ByteIndexValueType valueType3 = new ByteIndexValueType();
				return (Index<TValue>)(object)new Index<byte>(path, valueType3, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(short))
			{
				ShortIndexValueType valueType4 = new ShortIndexValueType();
				return (Index<TValue>)(object)new Index<short>(path, valueType4, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(ushort))
			{
				UShortIndexValueType valueType5 = new UShortIndexValueType();
				return (Index<TValue>)(object)new Index<ushort>(path, valueType5, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(int))
			{
				IntIndexValueType valueType6 = new IntIndexValueType();
				return (Index<TValue>)(object)new Index<int>(path, valueType6, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(uint))
			{
				UIntIndexValueType valueType7 = new UIntIndexValueType();
				return (Index<TValue>)(object)new Index<uint>(path, valueType7, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(long))
			{
				LongIndexValueType valueType8 = new LongIndexValueType();
				return (Index<TValue>)(object)new Index<long>(path, valueType8, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(ulong))
			{
				ULongIndexValueType valueType9 = new ULongIndexValueType();
				return (Index<TValue>)(object)new Index<ulong>(path, valueType9, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(float))
			{
				FloatIndexValueType valueType10 = new FloatIndexValueType();
				return (Index<TValue>)(object)new Index<float>(path, valueType10, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(double))
			{
				DoubleIndexValueType valueType11 = new DoubleIndexValueType();
				return (Index<TValue>)(object)new Index<double>(path, valueType11, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(char))
			{
				CharIndexValueType valueType12 = new CharIndexValueType();
				return (Index<TValue>)(object)new Index<char>(path, valueType12, encryptor, fileSystem, journalWriter);
			}
			if (typeFromHandle == typeof(string))
			{
				StringIndexValueType indexValueType = new StringIndexValueType();
				string path2 = HashedPathGenerator.GetPath(dir, fieldName + "_strings");
				string path3 = HashedPathGenerator.GetPath(dir, fieldName + "_stringsMeta");
				PackedFile packedFile = new PackedFile(path2, path3, journalWriter, fileSystem);
				PackedFileIndexValueType<string> valueType13 = new PackedFileIndexValueType<string>(indexValueType, packedFile);
				return (Index<TValue>)(object)new Index<string>(path, valueType13, encryptor, fileSystem, journalWriter);
			}
			throw new FormatException("Unhandled indexed field type: " + typeFromHandle);
		}
	}
}
