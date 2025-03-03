using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.LZW
{
	public class LzwInputStream : Stream
	{
		private const int TBL_CLEAR = 256;

		private const int TBL_FIRST = 257;

		private const int EXTRA = 64;

		private Stream baseInputStream;

		private bool isStreamOwner = true;

		private bool isClosed;

		private readonly byte[] one = new byte[1];

		private bool headerParsed;

		private int[] tabPrefix;

		private byte[] tabSuffix;

		private readonly int[] zeros = new int[256];

		private byte[] stack;

		private bool blockMode;

		private int nBits;

		private int maxBits;

		private int maxMaxCode;

		private int maxCode;

		private int bitMask;

		private int oldCode;

		private byte finChar;

		private int stackP;

		private int freeEnt;

		private readonly byte[] data = new byte[8192];

		private int bitPos;

		private int end;

		private int got;

		private bool eof;

		public bool IsStreamOwner
		{
			get
			{
				return isStreamOwner;
			}
			set
			{
				isStreamOwner = value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return baseInputStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return got;
			}
		}

		public override long Position
		{
			get
			{
				return baseInputStream.Position;
			}
			set
			{
				throw new NotSupportedException("InflaterInputStream Position not supported");
			}
		}

		public LzwInputStream(Stream baseInputStream)
		{
			this.baseInputStream = baseInputStream;
		}

		public override int ReadByte()
		{
			int num = Read(one, 0, 1);
			if (num == 1)
			{
				return one[0] & 0xFF;
			}
			return -1;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!headerParsed)
			{
				ParseHeader();
			}
			if (eof)
			{
				return -1;
			}
			int num = offset;
			int[] array = tabPrefix;
			byte[] array2 = tabSuffix;
			byte[] array3 = stack;
			int num2 = nBits;
			int num3 = maxCode;
			int num4 = maxMaxCode;
			int num5 = bitMask;
			int num6 = oldCode;
			byte b = finChar;
			int num7 = stackP;
			int num8 = freeEnt;
			byte[] array4 = data;
			int num9 = bitPos;
			int num10 = array3.Length - num7;
			if (num10 > 0)
			{
				int num11 = (num10 >= count) ? count : num10;
				Array.Copy(array3, num7, buffer, offset, num11);
				offset += num11;
				count -= num11;
				num7 += num11;
			}
			if (count == 0)
			{
				stackP = num7;
				return offset - num;
			}
			while (true)
			{
				if (end < 64)
				{
					Fill();
				}
				int num12 = (got > 0) ? (end - end % num2 << 3) : ((end << 3) - (num2 - 1));
				while (true)
				{
					if (num9 < num12)
					{
						if (count == 0)
						{
							nBits = num2;
							maxCode = num3;
							maxMaxCode = num4;
							bitMask = num5;
							oldCode = num6;
							finChar = b;
							stackP = num7;
							freeEnt = num8;
							bitPos = num9;
							return offset - num;
						}
						if (num8 > num3)
						{
							int num13 = num2 << 3;
							num9 = num9 - 1 + num13 - (num9 - 1 + num13) % num13;
							num2++;
							num3 = ((num2 == maxBits) ? num4 : ((1 << num2) - 1));
							num5 = (1 << num2) - 1;
							num9 = ResetBuf(num9);
							break;
						}
						int num14 = num9 >> 3;
						int num15 = (((array4[num14] & 0xFF) | ((array4[num14 + 1] & 0xFF) << 8) | ((array4[num14 + 2] & 0xFF) << 16)) >> (num9 & 7)) & num5;
						num9 += num2;
						if (num6 == -1)
						{
							if (num15 >= 256)
							{
								throw new LzwException("corrupt input: " + num15 + " > 255");
							}
							b = (byte)(num6 = num15);
							buffer[offset++] = b;
							count--;
							continue;
						}
						if (num15 == 256 && blockMode)
						{
							Array.Copy(zeros, 0, array, 0, zeros.Length);
							num8 = 256;
							int num16 = num2 << 3;
							num9 = num9 - 1 + num16 - (num9 - 1 + num16) % num16;
							num2 = 9;
							num3 = (1 << num2) - 1;
							num5 = num3;
							num9 = ResetBuf(num9);
							break;
						}
						int num17 = num15;
						num7 = array3.Length;
						if (num15 >= num8)
						{
							if (num15 > num8)
							{
								throw new LzwException("corrupt input: code=" + num15 + ", freeEnt=" + num8);
							}
							array3[--num7] = b;
							num15 = num6;
						}
						while (num15 >= 256)
						{
							array3[--num7] = array2[num15];
							num15 = array[num15];
						}
						b = array2[num15];
						buffer[offset++] = b;
						count--;
						num10 = array3.Length - num7;
						int num18 = (num10 >= count) ? count : num10;
						Array.Copy(array3, num7, buffer, offset, num18);
						offset += num18;
						count -= num18;
						num7 += num18;
						if (num8 < num4)
						{
							array[num8] = num6;
							array2[num8] = b;
							num8++;
						}
						num6 = num17;
						if (count != 0)
						{
							continue;
						}
						nBits = num2;
						maxCode = num3;
						bitMask = num5;
						oldCode = num6;
						finChar = b;
						stackP = num7;
						freeEnt = num8;
						bitPos = num9;
						return offset - num;
					}
					num9 = ResetBuf(num9);
					if (got > 0)
					{
						break;
					}
					nBits = num2;
					maxCode = num3;
					bitMask = num5;
					oldCode = num6;
					finChar = b;
					stackP = num7;
					freeEnt = num8;
					bitPos = num9;
					eof = true;
					return offset - num;
				}
			}
		}

		private int ResetBuf(int bitPosition)
		{
			int num = bitPosition >> 3;
			Array.Copy(data, num, data, 0, end - num);
			end -= num;
			return 0;
		}

		private void Fill()
		{
			got = baseInputStream.Read(data, end, data.Length - 1 - end);
			if (got > 0)
			{
				end += got;
			}
		}

		private void ParseHeader()
		{
			headerParsed = true;
			byte[] array = new byte[3];
			int num = baseInputStream.Read(array, 0, array.Length);
			if (num < 0)
			{
				throw new LzwException("Failed to read LZW header");
			}
			if (array[0] != 31 || array[1] != 157)
			{
				throw new LzwException(string.Format("Wrong LZW header. Magic bytes don't match. 0x{0:x2} 0x{1:x2}", array[0], array[1]));
			}
			blockMode = ((array[2] & 0x80) > 0);
			maxBits = (array[2] & 0x1F);
			if (maxBits > 16)
			{
				throw new LzwException("Stream compressed with " + maxBits + " bits, but decompression can only handle " + 16 + " bits.");
			}
			if ((array[2] & 0x60) > 0)
			{
				throw new LzwException("Unsupported bits set in the header.");
			}
			maxMaxCode = 1 << maxBits;
			nBits = 9;
			maxCode = (1 << nBits) - 1;
			bitMask = maxCode;
			oldCode = -1;
			finChar = 0;
			freeEnt = (blockMode ? 257 : 256);
			tabPrefix = new int[1 << maxBits];
			tabSuffix = new byte[1 << maxBits];
			stack = new byte[1 << maxBits];
			stackP = stack.Length;
			for (int num2 = 255; num2 >= 0; num2--)
			{
				tabSuffix[num2] = (byte)num2;
			}
		}

		public override void Flush()
		{
			baseInputStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		public override void Close()
		{
			if (!isClosed)
			{
				isClosed = true;
				if (isStreamOwner)
				{
					baseInputStream.Close();
				}
			}
		}
	}
}
