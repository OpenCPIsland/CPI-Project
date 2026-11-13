using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DeviceDB
{
	internal static class BinarySerializer
	{
		private enum FieldTypeId : byte
		{
			Bool,
			SByte,
			Byte,
			Short,
			UShort,
			Int,
			UInt,
			Long,
			ULong,
			Float,
			Double,
			Char,
			String,
			ByteArray
		}

		private struct FieldTypeMapping
		{
			public FieldTypeId Id;

			public Type Type;

			public Func<BinaryReader, object> Reader;

			public Action<object, BinaryWriter> Writer;
		}

		public const byte FormatVersion = 0;

		private static readonly FieldTypeMapping[] fieldTypeMappings = new FieldTypeMapping[14]
		{
			new FieldTypeMapping
			{
				Id = FieldTypeId.Bool,
				Type = typeof(bool),
				Reader = ((BinaryReader r) => r.ReadBoolean()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((bool)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.SByte,
				Type = typeof(sbyte),
				Reader = ((BinaryReader r) => r.ReadSByte()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((sbyte)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Byte,
				Type = typeof(byte),
				Reader = ((BinaryReader r) => r.ReadByte()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((byte)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Short,
				Type = typeof(short),
				Reader = ((BinaryReader r) => r.ReadInt16()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((short)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.UShort,
				Type = typeof(ushort),
				Reader = ((BinaryReader r) => r.ReadUInt16()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((ushort)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Int,
				Type = typeof(int),
				Reader = ((BinaryReader r) => r.ReadInt32()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((int)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.UInt,
				Type = typeof(uint),
				Reader = ((BinaryReader r) => r.ReadUInt32()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((uint)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Long,
				Type = typeof(long),
				Reader = ((BinaryReader r) => r.ReadInt64()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((long)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.ULong,
				Type = typeof(ulong),
				Reader = ((BinaryReader r) => r.ReadUInt64()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((ulong)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Float,
				Type = typeof(float),
				Reader = ((BinaryReader r) => r.ReadSingle()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((float)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Double,
				Type = typeof(double),
				Reader = ((BinaryReader r) => r.ReadDouble()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((double)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.Char,
				Type = typeof(char),
				Reader = ((BinaryReader r) => r.ReadChar()),
				Writer = delegate(object o, BinaryWriter w)
				{
					w.Write((char)o);
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.String,
				Type = typeof(string),
				Reader = ((BinaryReader r) => (!r.ReadBoolean()) ? r.ReadString() : null),
				Writer = delegate(object o, BinaryWriter w)
				{
					if (o == null)
					{
						w.Write(true);
					}
					else
					{
						w.Write(false);
						w.Write((string)o);
					}
				}
			},
			new FieldTypeMapping
			{
				Id = FieldTypeId.ByteArray,
				Type = typeof(byte[]),
				Reader = delegate(BinaryReader r)
				{
					if (r.ReadBoolean())
					{
						return null;
					}
					ushort count = r.ReadUInt16();
					return r.ReadBytes(count);
				},
				Writer = delegate(object o, BinaryWriter w)
				{
					if (o == null)
					{
						w.Write(true);
					}
					else
					{
						w.Write(false);
						byte[] array = (byte[])o;
						int num = array.Length;
						if (num > 65535)
						{
							throw new FormatException("Can't serialize byte[] because it's length (" + num + ") is longer than the maximum (" + ushort.MaxValue + ")");
						}
						w.Write((ushort)num);
						w.Write((byte[])o, 0, num);
					}
				}
			}
		};

		public static byte[] Serialize(object obj, Type objType)
		{
			if (obj == null)
			{
				throw new ArgumentException("Can't serialize null", "obj");
			}
			SerializedTypeReflection typeReflection = SerializerReflectionCache.GetTypeReflection(objType);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
				binaryWriter.Write((byte)0);
				binaryWriter.Write(typeReflection.Id);
				SerializedFieldReflection[] fieldReflections = typeReflection.FieldReflections;
				binaryWriter.Write((byte)fieldReflections.Length);
				SerializedFieldReflection[] array = fieldReflections;
				foreach (SerializedFieldReflection serializedFieldReflection in array)
				{
					FieldInfo fieldInfo = serializedFieldReflection.FieldInfo;
					Type fieldType = fieldInfo.FieldType;
					FieldTypeId fieldTypeId = GetFieldTypeId(fieldType);
					binaryWriter.Write((byte)fieldTypeId);
					binaryWriter.Write(serializedFieldReflection.Id);
					object value = fieldInfo.GetValue(obj);
					FieldTypeMapping[] array2 = fieldTypeMappings;
					for (int j = 0; j < array2.Length; j++)
					{
						FieldTypeMapping fieldTypeMapping = array2[j];
						if (fieldTypeMapping.Type == fieldType)
						{
							fieldTypeMapping.Writer(value, binaryWriter);
							break;
						}
					}
				}
				return memoryStream.ToArray();
			}
		}

		public static TDocument Deserialize<TDocument>(byte[] bytes) where TDocument : AbstractDocument, new()
		{
			if (bytes == null)
			{
				throw new ArgumentException("Can't deserialize null", "bytes");
			}
			using (MemoryStream input = new MemoryStream(bytes))
			{
				BinaryReader binaryReader = new BinaryReader(input);
				try
				{
					byte b = binaryReader.ReadByte();
					if (b != 0)
					{
						throw new CorruptionException("Can't deserialize format version " + b);
					}
					byte typeId = binaryReader.ReadByte();
					SerializedTypeReflection typeReflection = SerializerReflectionCache.GetTypeReflection(typeId);
					TDocument val = (TDocument)Activator.CreateInstance(typeReflection.Type);
					val.TypeId = typeId;
					SerializedFieldReflection[] fieldReflections = typeReflection.FieldReflections;
					byte b2 = binaryReader.ReadByte();
					for (int i = 0; i < b2; i++)
					{
						byte b3 = binaryReader.ReadByte();
						Type fieldType = GetFieldType(b3);
						byte b4 = binaryReader.ReadByte();
						SerializedFieldReflection fieldInfo = GetFieldInfo(fieldReflections, b4);
						FieldTypeId fieldTypeId = (FieldTypeId)b3;
						FieldTypeMapping[] array = fieldTypeMappings;
						for (int j = 0; j < array.Length; j++)
						{
							FieldTypeMapping fieldTypeMapping = array[j];
							if (fieldTypeMapping.Id == fieldTypeId)
							{
								object value = fieldTypeMapping.Reader(binaryReader);
								if (fieldInfo == null)
								{
									break;
								}
								FieldInfo fieldInfo2 = fieldInfo.FieldInfo;
								Type fieldType2 = fieldInfo2.FieldType;
								if (fieldType == fieldType2)
								{
									fieldInfo2.SetValue(val, value);
									break;
								}
								throw new CorruptionException(string.Concat("Field ", b4, " has type ", fieldType, " in the data to deserialize and type ", fieldType2, " in the code"));
							}
						}
					}
					return val;
				}
				catch (Exception innerException)
				{
					throw new CorruptionException("Unknown error while deserializing", innerException);
				}
			}
		}

		private static SerializedFieldReflection GetFieldInfo(IEnumerable<SerializedFieldReflection> fieldReflections, byte fieldId)
		{
			return fieldReflections.FirstOrDefault((SerializedFieldReflection fieldReflection) => fieldReflection.Id == fieldId);
		}

		private static FieldTypeId GetFieldTypeId(Type fieldType)
		{
			FieldTypeMapping[] array = fieldTypeMappings;
			for (int i = 0; i < array.Length; i++)
			{
				FieldTypeMapping fieldTypeMapping = array[i];
				if (fieldTypeMapping.Type == fieldType)
				{
					return fieldTypeMapping.Id;
				}
			}
			throw new FormatException("Unsupported type: " + fieldType);
		}

		private static Type GetFieldType(byte index)
		{
			if (index >= fieldTypeMappings.Length)
			{
				throw new CorruptionException("Unknown type: " + index);
			}
			return fieldTypeMappings[index].Type;
		}
	}
}
