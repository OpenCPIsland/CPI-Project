#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public enum SfsType : byte
	{
		Null = 0,
		Bool = 1,
		Byte = 2,
		Short = 3,
		Int = 4,
		Long = 5,
		Float = 6,
		Double = 7,
		UtfString = 8,
		IntArray = 12,
		FloatArray = 14,
		UtfStringArray = 16,
		SfsArray = 17,
		SfsObject = 18,
		ShortString = 19,
		Text = 20,
		Vector2 = 22,
		Vector3 = 23
	}

	public static class SfsVar
	{
		public const byte Null = 0;
		public const byte Bool = 1;
		public const byte Byte = 2;
		public const byte Short = 3;
		public const byte Int = 4;
		public const byte Long = 5;
		public const byte Float = 6;
		public const byte Double = 7;
		public const byte String = 8;
		public const byte Object = 9;
		public const byte Array = 10;
	}

	public sealed class SfsValue
	{
		public readonly SfsType Type;

		public readonly object Value;

		public SfsValue(SfsType type, object value)
		{
			Type = type;
			Value = value;
		}

		public static readonly SfsValue NullValue = new SfsValue(SfsType.Null, null);

		public string AsString
		{
			get
			{
				return (Type == SfsType.UtfString) ? (string)Value : null;
			}
		}

		public SfsObject AsObject
		{
			get
			{
				return Value as SfsObject;
			}
		}

		public SfsArray AsArray
		{
			get
			{
				return Value as SfsArray;
			}
		}

		public bool TryInteger(out long result)
		{
			switch (Type)
			{
			case SfsType.Byte:
				result = (byte)Value;
				return true;
			case SfsType.Short:
				result = (short)Value;
				return true;
			case SfsType.Int:
				result = (int)Value;
				return true;
			case SfsType.Long:
				result = (long)Value;
				return true;
			default:
				result = 0;
				return false;
			}
		}
	}

	public sealed class SfsArray
	{
		public readonly List<SfsValue> Items = new List<SfsValue>();

		public int Count
		{
			get
			{
				return Items.Count;
			}
		}

		public SfsValue this[int index]
		{
			get
			{
				return (index >= 0 && index < Items.Count) ? Items[index] : null;
			}
		}

		public SfsArray Add(SfsValue value)
		{
			Items.Add(value);
			return this;
		}

		public SfsArray AddInt(int v)
		{
			return Add(new SfsValue(SfsType.Int, v));
		}

		public SfsArray AddShort(short v)
		{
			return Add(new SfsValue(SfsType.Short, v));
		}

		public SfsArray AddBool(bool v)
		{
			return Add(new SfsValue(SfsType.Bool, v));
		}

		public SfsArray AddByte(byte v)
		{
			return Add(new SfsValue(SfsType.Byte, v));
		}

		public SfsArray AddDouble(double v)
		{
			return Add(new SfsValue(SfsType.Double, v));
		}

		public SfsArray AddNull()
		{
			return Add(SfsValue.NullValue);
		}

		public SfsArray AddString(string v)
		{
			return Add(new SfsValue(SfsType.UtfString, v));
		}

		public SfsArray AddFloats(float[] v)
		{
			return Add(new SfsValue(SfsType.FloatArray, v));
		}

		public SfsArray AddArray(SfsArray v)
		{
			return Add(new SfsValue(SfsType.SfsArray, v));
		}

		public SfsArray AddObject(SfsObject v)
		{
			return Add(new SfsValue(SfsType.SfsObject, v));
		}
	}

	public sealed class SfsObject
	{
		private readonly List<KeyValuePair<string, SfsValue>> entries = new List<KeyValuePair<string, SfsValue>>();

		public IEnumerable<KeyValuePair<string, SfsValue>> Entries
		{
			get
			{
				return entries;
			}
		}

		public int Count
		{
			get
			{
				return entries.Count;
			}
		}

		public SfsValue Get(string key)
		{
			for (int i = entries.Count - 1; i >= 0; i--)
			{
				if (entries[i].Key == key)
				{
					return entries[i].Value;
				}
			}
			return null;
		}

		public SfsObject Put(string key, SfsValue value)
		{
			entries.Add(new KeyValuePair<string, SfsValue>(key, value));
			return this;
		}

		public SfsObject PutBool(string key, bool v)
		{
			return Put(key, new SfsValue(SfsType.Bool, v));
		}

		public SfsObject PutByte(string key, byte v)
		{
			return Put(key, new SfsValue(SfsType.Byte, v));
		}

		public SfsObject PutShort(string key, short v)
		{
			return Put(key, new SfsValue(SfsType.Short, v));
		}

		public SfsObject PutInt(string key, int v)
		{
			return Put(key, new SfsValue(SfsType.Int, v));
		}

		public SfsObject PutLong(string key, long v)
		{
			return Put(key, new SfsValue(SfsType.Long, v));
		}

		public SfsObject PutString(string key, string v)
		{
			return Put(key, new SfsValue(SfsType.UtfString, v));
		}

		public SfsObject PutFloats(string key, float[] v)
		{
			return Put(key, new SfsValue(SfsType.FloatArray, v));
		}

		public SfsObject PutInts(string key, int[] v)
		{
			return Put(key, new SfsValue(SfsType.IntArray, v));
		}

		public SfsObject PutStrings(string key, string[] v)
		{
			return Put(key, new SfsValue(SfsType.UtfStringArray, v));
		}

		public SfsObject PutObject(string key, SfsObject v)
		{
			return Put(key, new SfsValue(SfsType.SfsObject, v));
		}

		public SfsObject PutArray(string key, SfsArray v)
		{
			return Put(key, new SfsValue(SfsType.SfsArray, v));
		}

		public string GetString(string key)
		{
			SfsValue v = Get(key);
			return (v != null) ? v.AsString : null;
		}

		public SfsObject GetObject(string key)
		{
			SfsValue v = Get(key);
			return (v != null && v.AsObject != null) ? v.AsObject : new SfsObject();
		}

		public float[] GetFloats(string key)
		{
			SfsValue v = Get(key);
			return (v != null && v.Type == SfsType.FloatArray) ? (float[])v.Value : null;
		}

		public bool TryGetByte(string key, out byte b)
		{
			SfsValue v = Get(key);
			if (v != null && v.Type == SfsType.Byte)
			{
				b = (byte)v.Value;
				return true;
			}
			b = 0;
			return false;
		}

		public bool TryGetInteger(string key, out long l)
		{
			SfsValue v = Get(key);
			if (v != null)
			{
				return v.TryInteger(out l);
			}
			l = 0;
			return false;
		}
	}

	public sealed class SfsHeader
	{
		public bool Compressed;

		public bool Encrypted;

		public int HeaderLength;

		public long BodyLength;

		public long TotalLength
		{
			get
			{
				return HeaderLength + BodyLength;
			}
		}
	}

	public static class SfsCodec
	{
		private const int MaxNesting = 32;

		public static SfsHeader TryReadHeader(byte[] buf, int offset, int count)
		{
			if (count < 1)
			{
				return null;
			}
			byte flags = buf[offset];
			bool big = (flags & 0x20) != 0;
			int lenSize = big ? 4 : 2;
			if (count < 1 + lenSize)
			{
				return null;
			}
			long len = big ? (long)ReadUInt32(buf, offset + 1) : ReadUInt16(buf, offset + 1);
			SfsHeader h = new SfsHeader();
			h.Compressed = (flags & 0x80) != 0;
			h.Encrypted = (flags & 0x40) != 0;
			h.HeaderLength = 1 + lenSize;
			h.BodyLength = len;
			return h;
		}

		public static void DecodeBody(byte[] buf, int offset, int length, out int controllerId, out int actionId, out SfsObject parameters)
		{
			if (length < 3)
			{
				throw new InvalidDataException("short body");
			}
			controllerId = buf[offset];
			actionId = (short)ReadUInt16(buf, offset + 1);
			int pos = offset + 3;
			int end = offset + length;
			parameters = DecodeObject(buf, ref pos, end, 0);
		}

		private static void Need(int pos, int n, int end)
		{
			if (pos + n > end)
			{
				throw new InvalidDataException("truncated");
			}
		}

		private static SfsObject DecodeObject(byte[] buf, ref int pos, int end, int depth)
		{
			Need(pos, 3, end);
			if (buf[pos] != (byte)SfsType.SfsObject)
			{
				throw new InvalidDataException("expected object");
			}
			pos++;
			int count = ReadUInt16(buf, pos);
			pos += 2;
			SfsObject obj = new SfsObject();
			for (int i = 0; i < count; i++)
			{
				Need(pos, 1, end);
				int keyLen = buf[pos];
				pos++;
				Need(pos, keyLen, end);
				string key = Encoding.UTF8.GetString(buf, pos, keyLen);
				pos += keyLen;
				obj.Put(key, DecodeValue(buf, ref pos, end, depth + 1));
			}
			return obj;
		}

		private static SfsValue DecodeValue(byte[] buf, ref int pos, int end, int depth)
		{
			if (depth > MaxNesting)
			{
				throw new InvalidDataException("too deep");
			}
			Need(pos, 1, end);
			SfsType type = (SfsType)buf[pos];
			switch (type)
			{
			case SfsType.Null:
				pos++;
				return SfsValue.NullValue;
			case SfsType.Bool:
				Need(pos, 2, end);
				pos += 2;
				return new SfsValue(type, buf[pos - 1] != 0);
			case SfsType.Byte:
				Need(pos, 2, end);
				pos += 2;
				return new SfsValue(type, buf[pos - 1]);
			case SfsType.Short:
				Need(pos, 3, end);
				pos += 3;
				return new SfsValue(type, (short)ReadUInt16(buf, pos - 2));
			case SfsType.Int:
				Need(pos, 5, end);
				pos += 5;
				return new SfsValue(type, (int)ReadUInt32(buf, pos - 4));
			case SfsType.Long:
				Need(pos, 9, end);
				pos += 9;
				return new SfsValue(type, ReadInt64(buf, pos - 8));
			case SfsType.Float:
				Need(pos, 5, end);
				pos += 5;
				return new SfsValue(type, ReadFloat(buf, pos - 4));
			case SfsType.Double:
				Need(pos, 9, end);
				pos += 9;
				return new SfsValue(type, BitConverter.Int64BitsToDouble(ReadInt64(buf, pos - 8)));
			case SfsType.UtfString:
			{
				Need(pos, 3, end);
				int len = ReadUInt16(buf, pos + 1);
				pos += 3;
				Need(pos, len, end);
				string s = Encoding.UTF8.GetString(buf, pos, len);
				pos += len;
				return new SfsValue(SfsType.UtfString, s);
			}
			case SfsType.Text:
			{
				Need(pos, 5, end);
				long len = ReadUInt32(buf, pos + 1);
				pos += 5;
				if (len > end - pos)
				{
					throw new InvalidDataException("truncated");
				}
				string s = Encoding.UTF8.GetString(buf, pos, (int)len);
				pos += (int)len;
				return new SfsValue(SfsType.UtfString, s);
			}
			case SfsType.ShortString:
			{
				Need(pos, 2, end);
				int len = buf[pos + 1];
				pos += 2;
				Need(pos, len, end);
				string s = Encoding.UTF8.GetString(buf, pos, len);
				pos += len;
				return new SfsValue(SfsType.UtfString, s);
			}
			case SfsType.SfsObject:
				return new SfsValue(type, DecodeObject(buf, ref pos, end, depth));
			case SfsType.FloatArray:
			{
				Need(pos, 3, end);
				int count = ReadUInt16(buf, pos + 1);
				pos += 3;
				Need(pos, count * 4, end);
				float[] values = new float[count];
				for (int i = 0; i < count; i++)
				{
					values[i] = ReadFloat(buf, pos + i * 4);
				}
				pos += count * 4;
				return new SfsValue(type, values);
			}
			case SfsType.Vector2:
			case SfsType.Vector3:
			{
				int count = (type == SfsType.Vector2) ? 2 : 3;
				Need(pos, 1 + count * 4, end);
				pos++;
				float[] values = new float[count];
				for (int i = 0; i < count; i++)
				{
					values[i] = ReadFloat(buf, pos + i * 4);
				}
				pos += count * 4;
				return new SfsValue(SfsType.FloatArray, values);
			}
			case SfsType.IntArray:
			{
				Need(pos, 3, end);
				int count = ReadUInt16(buf, pos + 1);
				pos += 3;
				Need(pos, count * 4, end);
				int[] values = new int[count];
				for (int i = 0; i < count; i++)
				{
					values[i] = (int)ReadUInt32(buf, pos + i * 4);
				}
				pos += count * 4;
				return new SfsValue(type, values);
			}
			case SfsType.UtfStringArray:
			{
				Need(pos, 3, end);
				int count = (short)ReadUInt16(buf, pos + 1);
				if (count < 0)
				{
					throw new InvalidDataException("negative size");
				}
				pos += 3;
				string[] values = new string[count];
				for (int i = 0; i < count; i++)
				{
					Need(pos, 2, end);
					int len = ReadUInt16(buf, pos);
					pos += 2;
					Need(pos, len, end);
					values[i] = Encoding.UTF8.GetString(buf, pos, len);
					pos += len;
				}
				return new SfsValue(type, values);
			}
			case SfsType.SfsArray:
			{
				Need(pos, 3, end);
				int count = ReadUInt16(buf, pos + 1);
				pos += 3;
				SfsArray arr = new SfsArray();
				for (int i = 0; i < count; i++)
				{
					arr.Add(DecodeValue(buf, ref pos, end, depth + 1));
				}
				return new SfsValue(type, arr);
			}
			default:
				throw new InvalidDataException("unsupported type " + (int)type);
			}
		}

		public static byte[] EncodePacket(int controllerId, int actionId, SfsObject parameters)
		{
			MemoryStream body = new MemoryStream();
			body.WriteByte((byte)controllerId);
			WriteUInt16(body, (ushort)(short)actionId);
			WriteObject(body, parameters);
			byte[] payload = body.ToArray();
			MemoryStream framed = new MemoryStream(payload.Length + 5);
			if (payload.Length <= 0xFFFF)
			{
				framed.WriteByte(0x00);
				WriteUInt16(framed, (ushort)payload.Length);
			}
			else
			{
				framed.WriteByte(0x20);
				WriteUInt32(framed, (uint)payload.Length);
			}
			framed.Write(payload, 0, payload.Length);
			return framed.ToArray();
		}

		private static void WriteObject(Stream s, SfsObject obj)
		{
			s.WriteByte((byte)SfsType.SfsObject);
			WriteUInt16(s, (ushort)obj.Count);
			foreach (KeyValuePair<string, SfsValue> kv in obj.Entries)
			{
				byte[] key = Encoding.UTF8.GetBytes(kv.Key);
				if (key.Length > 255)
				{
					throw new InvalidDataException("key too long");
				}
				s.WriteByte((byte)key.Length);
				s.Write(key, 0, key.Length);
				WriteValue(s, kv.Value);
			}
		}

		private static void WriteArray(Stream s, SfsArray arr)
		{
			s.WriteByte((byte)SfsType.SfsArray);
			WriteUInt16(s, (ushort)arr.Count);
			foreach (SfsValue v in arr.Items)
			{
				WriteValue(s, v);
			}
		}

		private static void WriteValue(Stream s, SfsValue v)
		{
			switch (v.Type)
			{
			case SfsType.Null:
				s.WriteByte(0);
				break;
			case SfsType.Bool:
				s.WriteByte(1);
				s.WriteByte((byte)(((bool)v.Value) ? 1 : 0));
				break;
			case SfsType.Byte:
				s.WriteByte(2);
				s.WriteByte((byte)v.Value);
				break;
			case SfsType.Short:
				s.WriteByte(3);
				WriteUInt16(s, (ushort)(short)v.Value);
				break;
			case SfsType.Int:
				s.WriteByte(4);
				WriteUInt32(s, (uint)(int)v.Value);
				break;
			case SfsType.Long:
				s.WriteByte(5);
				WriteInt64(s, (long)v.Value);
				break;
			case SfsType.Float:
				s.WriteByte(6);
				WriteUInt32(s, (uint)BitConverter.ToInt32(BitConverter.GetBytes((float)v.Value), 0));
				break;
			case SfsType.Double:
				s.WriteByte(7);
				WriteInt64(s, BitConverter.DoubleToInt64Bits((double)v.Value));
				break;
			case SfsType.UtfString:
			{
				byte[] b = Encoding.UTF8.GetBytes((string)v.Value);
				if (b.Length > 0xFFFF)
				{
					s.WriteByte(20);
					WriteUInt32(s, (uint)b.Length);
				}
				else
				{
					s.WriteByte(8);
					WriteUInt16(s, (ushort)b.Length);
				}
				s.Write(b, 0, b.Length);
				break;
			}
			case SfsType.IntArray:
			{
				int[] a = (int[])v.Value;
				s.WriteByte(12);
				WriteUInt16(s, (ushort)a.Length);
				foreach (int i in a)
				{
					WriteUInt32(s, (uint)i);
				}
				break;
			}
			case SfsType.FloatArray:
			{
				float[] a = (float[])v.Value;
				s.WriteByte(14);
				WriteUInt16(s, (ushort)a.Length);
				foreach (float f in a)
				{
					WriteUInt32(s, (uint)BitConverter.ToInt32(BitConverter.GetBytes(f), 0));
				}
				break;
			}
			case SfsType.UtfStringArray:
			{
				string[] a = (string[])v.Value;
				s.WriteByte(16);
				WriteUInt16(s, (ushort)(short)a.Length);
				foreach (string str in a)
				{
					byte[] b = Encoding.UTF8.GetBytes(str);
					WriteUInt16(s, (ushort)b.Length);
					s.Write(b, 0, b.Length);
				}
				break;
			}
			case SfsType.SfsArray:
				WriteArray(s, (SfsArray)v.Value);
				break;
			case SfsType.SfsObject:
				WriteObject(s, (SfsObject)v.Value);
				break;
			default:
				throw new InvalidDataException("unsupported type");
			}
		}

		private static int ReadUInt16(byte[] b, int o)
		{
			return (b[o] << 8) | b[o + 1];
		}

		private static uint ReadUInt32(byte[] b, int o)
		{
			return (uint)((b[o] << 24) | (b[o + 1] << 16) | (b[o + 2] << 8) | b[o + 3]);
		}

		private static long ReadInt64(byte[] b, int o)
		{
			return ((long)ReadUInt32(b, o) << 32) | ReadUInt32(b, o + 4);
		}

		private static float ReadFloat(byte[] b, int o)
		{
			return BitConverter.ToSingle(BitConverter.GetBytes((int)ReadUInt32(b, o)), 0);
		}

		private static void WriteUInt16(Stream s, ushort v)
		{
			s.WriteByte((byte)(v >> 8));
			s.WriteByte((byte)v);
		}

		private static void WriteUInt32(Stream s, uint v)
		{
			s.WriteByte((byte)(v >> 24));
			s.WriteByte((byte)(v >> 16));
			s.WriteByte((byte)(v >> 8));
			s.WriteByte((byte)v);
		}

		private static void WriteInt64(Stream s, long v)
		{
			WriteUInt32(s, (uint)(v >> 32));
			WriteUInt32(s, (uint)v);
		}
	}
}
#endif
