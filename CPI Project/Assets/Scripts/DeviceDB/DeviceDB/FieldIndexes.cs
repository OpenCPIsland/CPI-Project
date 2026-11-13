using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeviceDB
{
	internal class FieldIndexes<TDocument> where TDocument : AbstractDocument, new()
	{
		private readonly DocumentTypeReflection typeReflection;

		private readonly Dictionary<string, Type> fieldTypes;

		private readonly Dictionary<string, object> fieldIndexes;

		private readonly Dictionary<string, Index<bool>> boolIndexes;

		private readonly Dictionary<string, Index<sbyte>> sbyteIndexes;

		private readonly Dictionary<string, Index<byte>> byteIndexes;

		private readonly Dictionary<string, Index<short>> shortIndexes;

		private readonly Dictionary<string, Index<ushort>> ushortIndexes;

		private readonly Dictionary<string, Index<int>> intIndexes;

		private readonly Dictionary<string, Index<uint>> uintIndexes;

		private readonly Dictionary<string, Index<long>> longIndexes;

		private readonly Dictionary<string, Index<ulong>> ulongIndexes;

		private readonly Dictionary<string, Index<float>> floatIndexes;

		private readonly Dictionary<string, Index<double>> doubleIndexes;

		private readonly Dictionary<string, Index<char>> charIndexes;

		private readonly Dictionary<string, Index<string>> stringIndexes;

		public FieldIndexes(IndexFactory indexFactory, Aes256Encryptor encryptor)
		{
			typeReflection = DocumentReflectionCache.GetTypeReflection<TDocument>();
			fieldTypes = new Dictionary<string, Type>();
			fieldIndexes = new Dictionary<string, object>();
			boolIndexes = new Dictionary<string, Index<bool>>();
			sbyteIndexes = new Dictionary<string, Index<sbyte>>();
			byteIndexes = new Dictionary<string, Index<byte>>();
			shortIndexes = new Dictionary<string, Index<short>>();
			ushortIndexes = new Dictionary<string, Index<ushort>>();
			intIndexes = new Dictionary<string, Index<int>>();
			uintIndexes = new Dictionary<string, Index<uint>>();
			longIndexes = new Dictionary<string, Index<long>>();
			ulongIndexes = new Dictionary<string, Index<ulong>>();
			floatIndexes = new Dictionary<string, Index<float>>();
			doubleIndexes = new Dictionary<string, Index<double>>();
			charIndexes = new Dictionary<string, Index<char>>();
			stringIndexes = new Dictionary<string, Index<string>>();
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			int num = 0;
			string name;
			Type fieldType;
			while (true)
			{
				if (num >= fieldReflections.Length)
				{
					return;
				}
				DocumentFieldReflection documentFieldReflection = fieldReflections[num];
				FieldInfo fieldInfo = documentFieldReflection.FieldInfo;
				name = fieldInfo.Name;
				fieldType = fieldInfo.FieldType;
				fieldTypes.Add(name, fieldType);
				if (fieldType == typeof(bool))
				{
					Index<bool> value = indexFactory.Create<bool>(name, encryptor);
					boolIndexes.Add(name, value);
					fieldIndexes.Add(name, value);
				}
				else if (fieldType == typeof(sbyte))
				{
					Index<sbyte> value2 = indexFactory.Create<sbyte>(name, encryptor);
					sbyteIndexes.Add(name, value2);
					fieldIndexes.Add(name, value2);
				}
				else if (fieldType == typeof(byte))
				{
					Index<byte> value3 = indexFactory.Create<byte>(name, encryptor);
					byteIndexes.Add(name, value3);
					fieldIndexes.Add(name, value3);
				}
				else if (fieldType == typeof(short))
				{
					Index<short> value4 = indexFactory.Create<short>(name, encryptor);
					shortIndexes.Add(name, value4);
					fieldIndexes.Add(name, value4);
				}
				else if (fieldType == typeof(ushort))
				{
					Index<ushort> value5 = indexFactory.Create<ushort>(name, encryptor);
					ushortIndexes.Add(name, value5);
					fieldIndexes.Add(name, value5);
				}
				else if (fieldType == typeof(int))
				{
					Index<int> value6 = indexFactory.Create<int>(name, encryptor);
					intIndexes.Add(name, value6);
					fieldIndexes.Add(name, value6);
				}
				else if (fieldType == typeof(uint))
				{
					Index<uint> value7 = indexFactory.Create<uint>(name, encryptor);
					uintIndexes.Add(name, value7);
					fieldIndexes.Add(name, value7);
				}
				else if (fieldType == typeof(long))
				{
					Index<long> value8 = indexFactory.Create<long>(name, encryptor);
					longIndexes.Add(name, value8);
					fieldIndexes.Add(name, value8);
				}
				else if (fieldType == typeof(ulong))
				{
					Index<ulong> value9 = indexFactory.Create<ulong>(name, encryptor);
					ulongIndexes.Add(name, value9);
					fieldIndexes.Add(name, value9);
				}
				else if (fieldType == typeof(float))
				{
					Index<float> value10 = indexFactory.Create<float>(name, encryptor);
					floatIndexes.Add(name, value10);
					fieldIndexes.Add(name, value10);
				}
				else if (fieldType == typeof(double))
				{
					Index<double> value11 = indexFactory.Create<double>(name, encryptor);
					doubleIndexes.Add(name, value11);
					fieldIndexes.Add(name, value11);
				}
				else if (fieldType == typeof(char))
				{
					Index<char> value12 = indexFactory.Create<char>(name, encryptor);
					charIndexes.Add(name, value12);
					fieldIndexes.Add(name, value12);
				}
				else
				{
					if (fieldType != typeof(string))
					{
						break;
					}
					Index<string> value13 = indexFactory.Create<string>(name, encryptor);
					stringIndexes.Add(name, value13);
					fieldIndexes.Add(name, value13);
				}
				num++;
			}
			throw new FormatException("Unhandled field " + name + " with type " + fieldType);
		}

		public Index<TField> GetIndex<TField>(string fieldName) where TField : IComparable<TField>
		{
			if (!fieldTypes.ContainsKey(fieldName))
			{
				throw new ArgumentException("Field " + fieldName + " isn't indexed");
			}
			return (Index<TField>)fieldIndexes[fieldName];
		}

		public void Insert(TDocument document)
		{
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			foreach (DocumentFieldReflection documentFieldReflection in fieldReflections)
			{
				FieldInfo fieldInfo = documentFieldReflection.FieldInfo;
				object value = fieldInfo.GetValue(document);
				string name = fieldInfo.Name;
				Type fieldType = fieldInfo.FieldType;
				if (fieldType == typeof(bool))
				{
					boolIndexes[name].Insert(document.Id, (bool)value);
				}
				else if (fieldType == typeof(sbyte))
				{
					sbyteIndexes[name].Insert(document.Id, (sbyte)value);
				}
				else if (fieldType == typeof(byte))
				{
					byteIndexes[name].Insert(document.Id, (byte)value);
				}
				else if (fieldType == typeof(short))
				{
					shortIndexes[name].Insert(document.Id, (short)value);
				}
				else if (fieldType == typeof(ushort))
				{
					ushortIndexes[name].Insert(document.Id, (ushort)value);
				}
				else if (fieldType == typeof(int))
				{
					intIndexes[name].Insert(document.Id, (int)value);
				}
				else if (fieldType == typeof(uint))
				{
					uintIndexes[name].Insert(document.Id, (uint)value);
				}
				else if (fieldType == typeof(long))
				{
					longIndexes[name].Insert(document.Id, (long)value);
				}
				else if (fieldType == typeof(ulong))
				{
					ulongIndexes[name].Insert(document.Id, (ulong)value);
				}
				else if (fieldType == typeof(float))
				{
					floatIndexes[name].Insert(document.Id, (float)value);
				}
				else if (fieldType == typeof(double))
				{
					doubleIndexes[name].Insert(document.Id, (double)value);
				}
				else if (fieldType == typeof(char))
				{
					charIndexes[name].Insert(document.Id, (char)value);
				}
				else if (fieldType == typeof(string))
				{
					stringIndexes[name].Insert(document.Id, (string)value);
				}
			}
		}

		public void Update(TDocument document, TDocument oldDocument)
		{
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			foreach (DocumentFieldReflection documentFieldReflection in fieldReflections)
			{
				FieldInfo fieldInfo = documentFieldReflection.FieldInfo;
				string name = fieldInfo.Name;
				Type fieldType = fieldInfo.FieldType;
				object value = fieldInfo.GetValue(document);
				object value2 = fieldInfo.GetValue(oldDocument);
				if (fieldType == typeof(bool))
				{
					Index<bool> index = boolIndexes[name];
					bool flag = (bool)value;
					bool flag2 = (bool)value2;
					if (flag == flag2)
					{
						if (document.Id != oldDocument.Id)
						{
							index.Update(document.Id, oldDocument.Id, flag2);
						}
					}
					else
					{
						index.Update(document.Id, oldDocument.Id, flag, flag2);
					}
				}
				else if (fieldType == typeof(sbyte))
				{
					Index<sbyte> index2 = sbyteIndexes[name];
					sbyte b = (sbyte)value;
					sbyte b2 = (sbyte)value2;
					if (b == b2)
					{
						if (document.Id != oldDocument.Id)
						{
							index2.Update(document.Id, oldDocument.Id, b2);
						}
					}
					else
					{
						index2.Update(document.Id, oldDocument.Id, b, b2);
					}
				}
				else if (fieldType == typeof(byte))
				{
					Index<byte> index3 = byteIndexes[name];
					byte b3 = (byte)value;
					byte b4 = (byte)value2;
					if (b3 == b4)
					{
						if (document.Id != oldDocument.Id)
						{
							index3.Update(document.Id, oldDocument.Id, b4);
						}
					}
					else
					{
						index3.Update(document.Id, oldDocument.Id, b3, b4);
					}
				}
				else if (fieldType == typeof(short))
				{
					Index<short> index4 = shortIndexes[name];
					short num = (short)value;
					short num2 = (short)value2;
					if (num == num2)
					{
						if (document.Id != oldDocument.Id)
						{
							index4.Update(document.Id, oldDocument.Id, num2);
						}
					}
					else
					{
						index4.Update(document.Id, oldDocument.Id, num, num2);
					}
				}
				else if (fieldType == typeof(ushort))
				{
					Index<ushort> index5 = ushortIndexes[name];
					ushort num3 = (ushort)value;
					ushort num4 = (ushort)value2;
					if (num3 == num4)
					{
						if (document.Id != oldDocument.Id)
						{
							index5.Update(document.Id, oldDocument.Id, num4);
						}
					}
					else
					{
						index5.Update(document.Id, oldDocument.Id, num3, num4);
					}
				}
				else if (fieldType == typeof(int))
				{
					Index<int> index6 = intIndexes[name];
					int num5 = (int)value;
					int num6 = (int)value2;
					if (num5 == num6)
					{
						if (document.Id != oldDocument.Id)
						{
							index6.Update(document.Id, oldDocument.Id, num6);
						}
					}
					else
					{
						index6.Update(document.Id, oldDocument.Id, num5, num6);
					}
				}
				else if (fieldType == typeof(uint))
				{
					Index<uint> index7 = uintIndexes[name];
					uint num7 = (uint)value;
					uint num8 = (uint)value2;
					if (num7 == num8)
					{
						if (document.Id != oldDocument.Id)
						{
							index7.Update(document.Id, oldDocument.Id, num8);
						}
					}
					else
					{
						index7.Update(document.Id, oldDocument.Id, num7, num8);
					}
				}
				else if (fieldType == typeof(long))
				{
					Index<long> index8 = longIndexes[name];
					long num9 = (long)value;
					long num10 = (long)value2;
					if (num9 == num10)
					{
						if (document.Id != oldDocument.Id)
						{
							index8.Update(document.Id, oldDocument.Id, num10);
						}
					}
					else
					{
						index8.Update(document.Id, oldDocument.Id, num9, num10);
					}
				}
				else if (fieldType == typeof(ulong))
				{
					Index<ulong> index9 = ulongIndexes[name];
					ulong num11 = (ulong)value;
					ulong num12 = (ulong)value2;
					if (num11 == num12)
					{
						if (document.Id != oldDocument.Id)
						{
							index9.Update(document.Id, oldDocument.Id, num12);
						}
					}
					else
					{
						index9.Update(document.Id, oldDocument.Id, num11, num12);
					}
				}
				else if (fieldType == typeof(float))
				{
					Index<float> index10 = floatIndexes[name];
					float num13 = (float)value;
					float num14 = (float)value2;
					if (num13 == num14)
					{
						if (document.Id != oldDocument.Id)
						{
							index10.Update(document.Id, oldDocument.Id, num14);
						}
					}
					else
					{
						index10.Update(document.Id, oldDocument.Id, num13, num14);
					}
				}
				else if (fieldType == typeof(double))
				{
					Index<double> index11 = doubleIndexes[name];
					double num15 = (double)value;
					double num16 = (double)value2;
					if (num15 == num16)
					{
						if (document.Id != oldDocument.Id)
						{
							index11.Update(document.Id, oldDocument.Id, num16);
						}
					}
					else
					{
						index11.Update(document.Id, oldDocument.Id, num15, num16);
					}
				}
				else if (fieldType == typeof(char))
				{
					Index<char> index12 = charIndexes[name];
					char c = (char)value;
					char c2 = (char)value2;
					if (c == c2)
					{
						if (document.Id != oldDocument.Id)
						{
							index12.Update(document.Id, oldDocument.Id, c2);
						}
					}
					else
					{
						index12.Update(document.Id, oldDocument.Id, c, c2);
					}
				}
				else
				{
					if (fieldType != typeof(string))
					{
						continue;
					}
					Index<string> index13 = stringIndexes[name];
					string text = (string)value;
					string text2 = (string)value2;
					if (text == text2)
					{
						if (document.Id != oldDocument.Id)
						{
							index13.Update(document.Id, oldDocument.Id, text2);
						}
					}
					else
					{
						index13.Update(document.Id, oldDocument.Id, text, text2);
					}
				}
			}
		}

		public void Delete(TDocument document)
		{
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			foreach (DocumentFieldReflection documentFieldReflection in fieldReflections)
			{
				FieldInfo fieldInfo = documentFieldReflection.FieldInfo;
				string name = fieldInfo.Name;
				Type fieldType = fieldInfo.FieldType;
				object value = fieldInfo.GetValue(document);
				if (fieldType == typeof(bool))
				{
					boolIndexes[name].Remove(document.Id, (bool)value);
				}
				else if (fieldType == typeof(sbyte))
				{
					sbyteIndexes[name].Remove(document.Id, (sbyte)value);
				}
				else if (fieldType == typeof(byte))
				{
					byteIndexes[name].Remove(document.Id, (byte)value);
				}
				else if (fieldType == typeof(short))
				{
					shortIndexes[name].Remove(document.Id, (short)value);
				}
				else if (fieldType == typeof(ushort))
				{
					ushortIndexes[name].Remove(document.Id, (ushort)value);
				}
				else if (fieldType == typeof(int))
				{
					intIndexes[name].Remove(document.Id, (int)value);
				}
				else if (fieldType == typeof(uint))
				{
					uintIndexes[name].Remove(document.Id, (uint)value);
				}
				else if (fieldType == typeof(long))
				{
					longIndexes[name].Remove(document.Id, (long)value);
				}
				else if (fieldType == typeof(ulong))
				{
					ulongIndexes[name].Remove(document.Id, (ulong)value);
				}
				else if (fieldType == typeof(float))
				{
					floatIndexes[name].Remove(document.Id, (float)value);
				}
				else if (fieldType == typeof(double))
				{
					doubleIndexes[name].Remove(document.Id, (double)value);
				}
				else if (fieldType == typeof(char))
				{
					charIndexes[name].Remove(document.Id, (char)value);
				}
				else if (fieldType == typeof(string))
				{
					stringIndexes[name].Remove(document.Id, (string)value);
				}
			}
		}

		public void Delete()
		{
			foreach (KeyValuePair<string, Index<bool>> boolIndex in boolIndexes)
			{
				boolIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<sbyte>> sbyteIndex in sbyteIndexes)
			{
				sbyteIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<byte>> byteIndex in byteIndexes)
			{
				byteIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<short>> shortIndex in shortIndexes)
			{
				shortIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<ushort>> ushortIndex in ushortIndexes)
			{
				ushortIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<int>> intIndex in intIndexes)
			{
				intIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<uint>> uintIndex in uintIndexes)
			{
				uintIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<long>> longIndex in longIndexes)
			{
				longIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<ulong>> ulongIndex in ulongIndexes)
			{
				ulongIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<float>> floatIndex in floatIndexes)
			{
				floatIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<double>> doubleIndex in doubleIndexes)
			{
				doubleIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<char>> charIndex in charIndexes)
			{
				charIndex.Value.Delete();
			}
			foreach (KeyValuePair<string, Index<string>> stringIndex in stringIndexes)
			{
				stringIndex.Value.Delete();
			}
		}

		public void JournaledClear()
		{
			foreach (KeyValuePair<string, Index<bool>> boolIndex in boolIndexes)
			{
				boolIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<sbyte>> sbyteIndex in sbyteIndexes)
			{
				sbyteIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<byte>> byteIndex in byteIndexes)
			{
				byteIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<short>> shortIndex in shortIndexes)
			{
				shortIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<ushort>> ushortIndex in ushortIndexes)
			{
				ushortIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<int>> intIndex in intIndexes)
			{
				intIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<uint>> uintIndex in uintIndexes)
			{
				uintIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<long>> longIndex in longIndexes)
			{
				longIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<ulong>> ulongIndex in ulongIndexes)
			{
				ulongIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<float>> floatIndex in floatIndexes)
			{
				floatIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<double>> doubleIndex in doubleIndexes)
			{
				doubleIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<char>> charIndex in charIndexes)
			{
				charIndex.Value.JournaledClear();
			}
			foreach (KeyValuePair<string, Index<string>> stringIndex in stringIndexes)
			{
				stringIndex.Value.JournaledClear();
			}
		}

		public void ChangeEncryptor(Aes256Encryptor encryptor)
		{
			foreach (KeyValuePair<string, Index<bool>> boolIndex in boolIndexes)
			{
				boolIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<sbyte>> sbyteIndex in sbyteIndexes)
			{
				sbyteIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<byte>> byteIndex in byteIndexes)
			{
				byteIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<short>> shortIndex in shortIndexes)
			{
				shortIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<ushort>> ushortIndex in ushortIndexes)
			{
				ushortIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<int>> intIndex in intIndexes)
			{
				intIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<uint>> uintIndex in uintIndexes)
			{
				uintIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<long>> longIndex in longIndexes)
			{
				longIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<ulong>> ulongIndex in ulongIndexes)
			{
				ulongIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<float>> floatIndex in floatIndexes)
			{
				floatIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<double>> doubleIndex in doubleIndexes)
			{
				doubleIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<char>> charIndex in charIndexes)
			{
				charIndex.Value.ChangeEncryptor(encryptor);
			}
			foreach (KeyValuePair<string, Index<string>> stringIndex in stringIndexes)
			{
				stringIndex.Value.ChangeEncryptor(encryptor);
			}
		}

		public void Dispose()
		{
			foreach (KeyValuePair<string, Index<bool>> boolIndex in boolIndexes)
			{
				boolIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<sbyte>> sbyteIndex in sbyteIndexes)
			{
				sbyteIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<byte>> byteIndex in byteIndexes)
			{
				byteIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<short>> shortIndex in shortIndexes)
			{
				shortIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<ushort>> ushortIndex in ushortIndexes)
			{
				ushortIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<int>> intIndex in intIndexes)
			{
				intIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<uint>> uintIndex in uintIndexes)
			{
				uintIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<long>> longIndex in longIndexes)
			{
				longIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<ulong>> ulongIndex in ulongIndexes)
			{
				ulongIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<float>> floatIndex in floatIndexes)
			{
				floatIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<double>> doubleIndex in doubleIndexes)
			{
				doubleIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<char>> charIndex in charIndexes)
			{
				charIndex.Value.Dispose();
			}
			foreach (KeyValuePair<string, Index<string>> stringIndex in stringIndexes)
			{
				stringIndex.Value.Dispose();
			}
		}

		public IEnumerable<string> GetMissingIndexedFieldNames(uint count)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Index<bool>> boolIndex in boolIndexes)
			{
				if (boolIndex.Value.Count < count)
				{
					list.Add(boolIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<sbyte>> sbyteIndex in sbyteIndexes)
			{
				if (sbyteIndex.Value.Count < count)
				{
					list.Add(sbyteIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<byte>> byteIndex in byteIndexes)
			{
				if (byteIndex.Value.Count < count)
				{
					list.Add(byteIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<short>> shortIndex in shortIndexes)
			{
				if (shortIndex.Value.Count < count)
				{
					list.Add(shortIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<ushort>> ushortIndex in ushortIndexes)
			{
				if (ushortIndex.Value.Count < count)
				{
					list.Add(ushortIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<int>> intIndex in intIndexes)
			{
				if (intIndex.Value.Count < count)
				{
					list.Add(intIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<uint>> uintIndex in uintIndexes)
			{
				if (uintIndex.Value.Count < count)
				{
					list.Add(uintIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<long>> longIndex in longIndexes)
			{
				if (longIndex.Value.Count < count)
				{
					list.Add(longIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<ulong>> ulongIndex in ulongIndexes)
			{
				if (ulongIndex.Value.Count < count)
				{
					list.Add(ulongIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<float>> floatIndex in floatIndexes)
			{
				if (floatIndex.Value.Count < count)
				{
					list.Add(floatIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<double>> doubleIndex in doubleIndexes)
			{
				if (doubleIndex.Value.Count < count)
				{
					list.Add(doubleIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<char>> charIndex in charIndexes)
			{
				if (charIndex.Value.Count < count)
				{
					list.Add(charIndex.Key);
				}
			}
			foreach (KeyValuePair<string, Index<string>> stringIndex in stringIndexes)
			{
				if (stringIndex.Value.Count < count)
				{
					list.Add(stringIndex.Key);
				}
			}
			return list;
		}

		public void ClearField(string fieldName)
		{
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			int num = 0;
			FieldInfo fieldInfo;
			while (true)
			{
				if (num < fieldReflections.Length)
				{
					DocumentFieldReflection documentFieldReflection = fieldReflections[num];
					fieldInfo = documentFieldReflection.FieldInfo;
					if (fieldInfo.Name == fieldName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Type fieldType = fieldInfo.FieldType;
			if (fieldType == typeof(bool))
			{
				boolIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(sbyte))
			{
				sbyteIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(byte))
			{
				byteIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(short))
			{
				shortIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(ushort))
			{
				ushortIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(int))
			{
				intIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(uint))
			{
				uintIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(long))
			{
				longIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(ulong))
			{
				ulongIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(float))
			{
				floatIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(double))
			{
				doubleIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(char))
			{
				charIndexes[fieldName].Clear();
			}
			else if (fieldType == typeof(string))
			{
				stringIndexes[fieldName].Clear();
			}
		}

		public IComparable GetFieldValue(string fieldName, TDocument document)
		{
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			foreach (DocumentFieldReflection documentFieldReflection in fieldReflections)
			{
				FieldInfo fieldInfo = documentFieldReflection.FieldInfo;
				if (fieldInfo.Name == fieldName)
				{
					return (IComparable)fieldInfo.GetValue(document);
				}
			}
			throw new ArgumentException("Field " + fieldName + " not found");
		}

		public void InsertPreSorted(string fieldName, IEnumerable<KeyValuePair<uint, IComparable>> entries)
		{
			DocumentFieldReflection[] fieldReflections = typeReflection.FieldReflections;
			int num = 0;
			FieldInfo fieldInfo;
			while (true)
			{
				if (num < fieldReflections.Length)
				{
					DocumentFieldReflection documentFieldReflection = fieldReflections[num];
					fieldInfo = documentFieldReflection.FieldInfo;
					if (fieldInfo.Name == fieldName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Type fieldType = fieldInfo.FieldType;
			if (fieldType == typeof(bool))
			{
				boolIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(sbyte))
			{
				sbyteIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(byte))
			{
				byteIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(short))
			{
				shortIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(ushort))
			{
				ushortIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(int))
			{
				intIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(uint))
			{
				uintIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(long))
			{
				longIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(ulong))
			{
				ulongIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(float))
			{
				floatIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(double))
			{
				doubleIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(char))
			{
				charIndexes[fieldName].InsertPreSorted(entries);
			}
			else if (fieldType == typeof(string))
			{
				stringIndexes[fieldName].InsertPreSorted(entries);
			}
		}
	}
}
