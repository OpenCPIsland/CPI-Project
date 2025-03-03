using System;
using System.Text;

public sealed class SwrveMD5Core
{
	private SwrveMD5Core()
	{
	}

	public static byte[] GetHash(string input, Encoding encoding)
	{
		if (null == input)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		if (null == encoding)
		{
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding");
		}
		byte[] bytes = encoding.GetBytes(input);
		return GetHash(bytes);
	}

	public static byte[] GetHash(string input)
	{
		return GetHash(input, new UTF8Encoding());
	}

	public static string GetHashString(byte[] input)
	{
		if (null == input)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		string text = BitConverter.ToString(GetHash(input));
		return text.Replace("-", "");
	}

	public static string GetHashString(string input, Encoding encoding)
	{
		if (null == input)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		if (null == encoding)
		{
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHashString(string) overload to use UTF8 Encoding");
		}
		byte[] bytes = encoding.GetBytes(input);
		return GetHashString(bytes);
	}

	public static string GetHashString(string input)
	{
		return GetHashString(input, new UTF8Encoding());
	}

	public static byte[] GetHash(byte[] input)
	{
		if (null == input)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		SwrveABCDStruct ABCDValue = default(SwrveABCDStruct);
		ABCDValue.A = 1732584193u;
		ABCDValue.B = 4023233417u;
		ABCDValue.C = 2562383102u;
		ABCDValue.D = 271733878u;
		int i;
		for (i = 0; i <= input.Length - 64; i += 64)
		{
			GetHashBlock(input, ref ABCDValue, i);
		}
		return GetHashFinalBlock(input, i, input.Length - i, ABCDValue, (long)input.Length * 8L);
	}

	internal static byte[] GetHashFinalBlock(byte[] input, int ibStart, int cbSize, SwrveABCDStruct ABCD, long len)
	{
		byte[] array = new byte[64];
		byte[] bytes = BitConverter.GetBytes(len);
		Array.Copy(input, ibStart, array, 0, cbSize);
		array[cbSize] = 128;
		if (cbSize < 56)
		{
			Array.Copy(bytes, 0, array, 56, 8);
			GetHashBlock(array, ref ABCD, 0);
		}
		else
		{
			GetHashBlock(array, ref ABCD, 0);
			array = new byte[64];
			Array.Copy(bytes, 0, array, 56, 8);
			GetHashBlock(array, ref ABCD, 0);
		}
		byte[] array2 = new byte[16];
		Array.Copy(BitConverter.GetBytes(ABCD.A), 0, array2, 0, 4);
		Array.Copy(BitConverter.GetBytes(ABCD.B), 0, array2, 4, 4);
		Array.Copy(BitConverter.GetBytes(ABCD.C), 0, array2, 8, 4);
		Array.Copy(BitConverter.GetBytes(ABCD.D), 0, array2, 12, 4);
		return array2;
	}

	internal static void GetHashBlock(byte[] input, ref SwrveABCDStruct ABCDValue, int ibStart)
	{
		uint[] array = Converter(input, ibStart);
		uint a = ABCDValue.A;
		uint b = ABCDValue.B;
		uint c = ABCDValue.C;
		uint d = ABCDValue.D;
		a = r1(a, b, c, d, array[0], 7, 3614090360u);
		d = r1(d, a, b, c, array[1], 12, 3905402710u);
		c = r1(c, d, a, b, array[2], 17, 606105819u);
		b = r1(b, c, d, a, array[3], 22, 3250441966u);
		a = r1(a, b, c, d, array[4], 7, 4118548399u);
		d = r1(d, a, b, c, array[5], 12, 1200080426u);
		c = r1(c, d, a, b, array[6], 17, 2821735955u);
		b = r1(b, c, d, a, array[7], 22, 4249261313u);
		a = r1(a, b, c, d, array[8], 7, 1770035416u);
		d = r1(d, a, b, c, array[9], 12, 2336552879u);
		c = r1(c, d, a, b, array[10], 17, 4294925233u);
		b = r1(b, c, d, a, array[11], 22, 2304563134u);
		a = r1(a, b, c, d, array[12], 7, 1804603682u);
		d = r1(d, a, b, c, array[13], 12, 4254626195u);
		c = r1(c, d, a, b, array[14], 17, 2792965006u);
		b = r1(b, c, d, a, array[15], 22, 1236535329u);
		a = r2(a, b, c, d, array[1], 5, 4129170786u);
		d = r2(d, a, b, c, array[6], 9, 3225465664u);
		c = r2(c, d, a, b, array[11], 14, 643717713u);
		b = r2(b, c, d, a, array[0], 20, 3921069994u);
		a = r2(a, b, c, d, array[5], 5, 3593408605u);
		d = r2(d, a, b, c, array[10], 9, 38016083u);
		c = r2(c, d, a, b, array[15], 14, 3634488961u);
		b = r2(b, c, d, a, array[4], 20, 3889429448u);
		a = r2(a, b, c, d, array[9], 5, 568446438u);
		d = r2(d, a, b, c, array[14], 9, 3275163606u);
		c = r2(c, d, a, b, array[3], 14, 4107603335u);
		b = r2(b, c, d, a, array[8], 20, 1163531501u);
		a = r2(a, b, c, d, array[13], 5, 2850285829u);
		d = r2(d, a, b, c, array[2], 9, 4243563512u);
		c = r2(c, d, a, b, array[7], 14, 1735328473u);
		b = r2(b, c, d, a, array[12], 20, 2368359562u);
		a = r3(a, b, c, d, array[5], 4, 4294588738u);
		d = r3(d, a, b, c, array[8], 11, 2272392833u);
		c = r3(c, d, a, b, array[11], 16, 1839030562u);
		b = r3(b, c, d, a, array[14], 23, 4259657740u);
		a = r3(a, b, c, d, array[1], 4, 2763975236u);
		d = r3(d, a, b, c, array[4], 11, 1272893353u);
		c = r3(c, d, a, b, array[7], 16, 4139469664u);
		b = r3(b, c, d, a, array[10], 23, 3200236656u);
		a = r3(a, b, c, d, array[13], 4, 681279174u);
		d = r3(d, a, b, c, array[0], 11, 3936430074u);
		c = r3(c, d, a, b, array[3], 16, 3572445317u);
		b = r3(b, c, d, a, array[6], 23, 76029189u);
		a = r3(a, b, c, d, array[9], 4, 3654602809u);
		d = r3(d, a, b, c, array[12], 11, 3873151461u);
		c = r3(c, d, a, b, array[15], 16, 530742520u);
		b = r3(b, c, d, a, array[2], 23, 3299628645u);
		a = r4(a, b, c, d, array[0], 6, 4096336452u);
		d = r4(d, a, b, c, array[7], 10, 1126891415u);
		c = r4(c, d, a, b, array[14], 15, 2878612391u);
		b = r4(b, c, d, a, array[5], 21, 4237533241u);
		a = r4(a, b, c, d, array[12], 6, 1700485571u);
		d = r4(d, a, b, c, array[3], 10, 2399980690u);
		c = r4(c, d, a, b, array[10], 15, 4293915773u);
		b = r4(b, c, d, a, array[1], 21, 2240044497u);
		a = r4(a, b, c, d, array[8], 6, 1873313359u);
		d = r4(d, a, b, c, array[15], 10, 4264355552u);
		c = r4(c, d, a, b, array[6], 15, 2734768916u);
		b = r4(b, c, d, a, array[13], 21, 1309151649u);
		a = r4(a, b, c, d, array[4], 6, 4149444226u);
		d = r4(d, a, b, c, array[11], 10, 3174756917u);
		c = r4(c, d, a, b, array[2], 15, 718787259u);
		b = r4(b, c, d, a, array[9], 21, 3951481745u);
		ABCDValue.A = a + ABCDValue.A;
		ABCDValue.B = b + ABCDValue.B;
		ABCDValue.C = c + ABCDValue.C;
		ABCDValue.D = d + ABCDValue.D;
	}

	private static uint r1(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + LSR((uint)((int)a + ((int)(b & c) | (((int)b ^ -1) & (int)d)) + (int)x + (int)t), s);
	}

	private static uint r2(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + LSR((uint)((int)a + ((int)(b & d) | ((int)c & ((int)d ^ -1))) + (int)x + (int)t), s);
	}

	private static uint r3(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + LSR(a + (b ^ c ^ d) + x + t, s);
	}

	private static uint r4(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + LSR((uint)((int)a + ((int)c ^ ((int)b | ((int)d ^ -1))) + (int)x + (int)t), s);
	}

	private static uint LSR(uint i, int s)
	{
		return (i << s) | (i >> 32 - s);
	}

	private static uint[] Converter(byte[] input, int ibStart)
	{
		if (null == input)
		{
			throw new ArgumentNullException("input", "Unable convert null array to array of uInts");
		}
		uint[] array = new uint[16];
		for (int i = 0; i < 16; i++)
		{
			array[i] = input[ibStart + i * 4];
			array[i] += (uint)(input[ibStart + i * 4 + 1] << 8);
			array[i] += (uint)(input[ibStart + i * 4 + 2] << 16);
			array[i] += (uint)(input[ibStart + i * 4 + 3] << 24);
		}
		return array;
	}
}
