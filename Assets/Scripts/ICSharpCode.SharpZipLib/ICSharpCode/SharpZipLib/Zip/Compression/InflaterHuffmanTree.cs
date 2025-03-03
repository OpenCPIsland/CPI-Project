using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class InflaterHuffmanTree
	{
		private const int MAX_BITLEN = 15;

		private short[] tree;

		public static InflaterHuffmanTree defLitLenTree;

		public static InflaterHuffmanTree defDistTree;

		static InflaterHuffmanTree()
		{
			try
			{
				byte[] array = new byte[288];
				int num = 0;
				while (num < 144)
				{
					array[num++] = 8;
				}
				while (num < 256)
				{
					array[num++] = 9;
				}
				while (num < 280)
				{
					array[num++] = 7;
				}
				while (num < 288)
				{
					array[num++] = 8;
				}
				defLitLenTree = new InflaterHuffmanTree(array);
				array = new byte[32];
				num = 0;
				while (num < 32)
				{
					array[num++] = 5;
				}
				defDistTree = new InflaterHuffmanTree(array);
			}
			catch (Exception)
			{
				throw new SharpZipBaseException("InflaterHuffmanTree: static tree length illegal");
			}
		}

		public InflaterHuffmanTree(byte[] codeLengths)
		{
			BuildTree(codeLengths);
		}

		private void BuildTree(byte[] codeLengths)
		{
			int[] array = new int[16];
			int[] array2 = new int[16];
			foreach (int num in codeLengths)
			{
				if (num > 0)
				{
					array[num]++;
				}
			}
			int num2 = 0;
			int num3 = 512;
			for (int j = 1; j <= 15; j++)
			{
				array2[j] = num2;
				num2 += array[j] << 16 - j;
				if (j >= 10)
				{
					int num4 = array2[j] & 0x1FF80;
					int num5 = num2 & 0x1FF80;
					num3 += num5 - num4 >> 16 - j;
				}
			}
			tree = new short[num3];
			int num6 = 512;
			for (int num7 = 15; num7 >= 10; num7--)
			{
				int num8 = num2 & 0x1FF80;
				num2 -= array[num7] << 16 - num7;
				int num9 = num2 & 0x1FF80;
				for (int k = num9; k < num8; k += 128)
				{
					tree[DeflaterHuffman.BitReverse(k)] = (short)((-num6 << 4) | num7);
					num6 += 1 << num7 - 9;
				}
			}
			for (int l = 0; l < codeLengths.Length; l++)
			{
				int num10 = codeLengths[l];
				if (num10 == 0)
				{
					continue;
				}
				num2 = array2[num10];
				int num11 = DeflaterHuffman.BitReverse(num2);
				if (num10 <= 9)
				{
					do
					{
						tree[num11] = (short)((l << 4) | num10);
						num11 += 1 << num10;
					}
					while (num11 < 512);
				}
				else
				{
					int num12 = tree[num11 & 0x1FF];
					int num13 = 1 << (num12 & 0xF);
					num12 = -(num12 >> 4);
					do
					{
						tree[num12 | (num11 >> 9)] = (short)((l << 4) | num10);
						num11 += 1 << num10;
					}
					while (num11 < num13);
				}
				array2[num10] = num2 + (1 << 16 - num10);
			}
		}

		public int GetSymbol(StreamManipulator input)
		{
			int num;
			int num2;
			if ((num = input.PeekBits(9)) >= 0)
			{
				if ((num2 = tree[num]) >= 0)
				{
					input.DropBits(num2 & 0xF);
					return num2 >> 4;
				}
				int num3 = -(num2 >> 4);
				int bitCount = num2 & 0xF;
				if ((num = input.PeekBits(bitCount)) >= 0)
				{
					num2 = tree[num3 | (num >> 9)];
					input.DropBits(num2 & 0xF);
					return num2 >> 4;
				}
				int availableBits = input.AvailableBits;
				num = input.PeekBits(availableBits);
				num2 = tree[num3 | (num >> 9)];
				if ((num2 & 0xF) <= availableBits)
				{
					input.DropBits(num2 & 0xF);
					return num2 >> 4;
				}
				return -1;
			}
			int availableBits2 = input.AvailableBits;
			num = input.PeekBits(availableBits2);
			num2 = tree[num];
			if (num2 >= 0 && (num2 & 0xF) <= availableBits2)
			{
				input.DropBits(num2 & 0xF);
				return num2 >> 4;
			}
			return -1;
		}
	}
}
