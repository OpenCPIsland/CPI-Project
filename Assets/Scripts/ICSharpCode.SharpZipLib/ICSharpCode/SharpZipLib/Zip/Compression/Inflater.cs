using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class Inflater
	{
		private const int DECODE_HEADER = 0;

		private const int DECODE_DICT = 1;

		private const int DECODE_BLOCKS = 2;

		private const int DECODE_STORED_LEN1 = 3;

		private const int DECODE_STORED_LEN2 = 4;

		private const int DECODE_STORED = 5;

		private const int DECODE_DYN_HEADER = 6;

		private const int DECODE_HUFFMAN = 7;

		private const int DECODE_HUFFMAN_LENBITS = 8;

		private const int DECODE_HUFFMAN_DIST = 9;

		private const int DECODE_HUFFMAN_DISTBITS = 10;

		private const int DECODE_CHKSUM = 11;

		private const int FINISHED = 12;

		private static readonly int[] CPLENS = new int[29]
		{
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			13,
			15,
			17,
			19,
			23,
			27,
			31,
			35,
			43,
			51,
			59,
			67,
			83,
			99,
			115,
			131,
			163,
			195,
			227,
			258
		};

		private static readonly int[] CPLEXT = new int[29]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			4,
			4,
			4,
			5,
			5,
			5,
			5,
			0
		};

		private static readonly int[] CPDIST = new int[30]
		{
			1,
			2,
			3,
			4,
			5,
			7,
			9,
			13,
			17,
			25,
			33,
			49,
			65,
			97,
			129,
			193,
			257,
			385,
			513,
			769,
			1025,
			1537,
			2049,
			3073,
			4097,
			6145,
			8193,
			12289,
			16385,
			24577
		};

		private static readonly int[] CPDEXT = new int[30]
		{
			0,
			0,
			0,
			0,
			1,
			1,
			2,
			2,
			3,
			3,
			4,
			4,
			5,
			5,
			6,
			6,
			7,
			7,
			8,
			8,
			9,
			9,
			10,
			10,
			11,
			11,
			12,
			12,
			13,
			13
		};

		private int mode;

		private int readAdler;

		private int neededBits;

		private int repLength;

		private int repDist;

		private int uncomprLen;

		private bool isLastBlock;

		private long totalOut;

		private long totalIn;

		private bool noHeader;

		private StreamManipulator input;

		private OutputWindow outputWindow;

		private InflaterDynHeader dynHeader;

		private InflaterHuffmanTree litlenTree;

		private InflaterHuffmanTree distTree;

		private Adler32 adler;

		public bool IsNeedingInput
		{
			get
			{
				return input.IsNeedingInput;
			}
		}

		public bool IsNeedingDictionary
		{
			get
			{
				if (mode == 1)
				{
					return neededBits == 0;
				}
				return false;
			}
		}

		public bool IsFinished
		{
			get
			{
				if (mode == 12)
				{
					return outputWindow.GetAvailable() == 0;
				}
				return false;
			}
		}

		public int Adler
		{
			get
			{
				if (!IsNeedingDictionary)
				{
					return (int)adler.Value;
				}
				return readAdler;
			}
		}

		public long TotalOut
		{
			get
			{
				return totalOut;
			}
		}

		public long TotalIn
		{
			get
			{
				return totalIn - RemainingInput;
			}
		}

		public int RemainingInput
		{
			get
			{
				return input.AvailableBytes;
			}
		}

		public Inflater()
			: this(false)
		{
		}

		public Inflater(bool noHeader)
		{
			this.noHeader = noHeader;
			adler = new Adler32();
			input = new StreamManipulator();
			outputWindow = new OutputWindow();
			mode = (noHeader ? 2 : 0);
		}

		public void Reset()
		{
			mode = (noHeader ? 2 : 0);
			totalIn = 0L;
			totalOut = 0L;
			input.Reset();
			outputWindow.Reset();
			dynHeader = null;
			litlenTree = null;
			distTree = null;
			isLastBlock = false;
			adler.Reset();
		}

		private bool DecodeHeader()
		{
			int num = input.PeekBits(16);
			if (num < 0)
			{
				return false;
			}
			input.DropBits(16);
			num = (((num << 8) | (num >> 8)) & 0xFFFF);
			if (num % 31 != 0)
			{
				throw new SharpZipBaseException("Header checksum illegal");
			}
			if ((num & 0xF00) != 2048)
			{
				throw new SharpZipBaseException("Compression Method unknown");
			}
			if ((num & 0x20) == 0)
			{
				mode = 2;
			}
			else
			{
				mode = 1;
				neededBits = 32;
			}
			return true;
		}

		private bool DecodeDict()
		{
			while (neededBits > 0)
			{
				int num = input.PeekBits(8);
				if (num < 0)
				{
					return false;
				}
				input.DropBits(8);
				readAdler = ((readAdler << 8) | num);
				neededBits -= 8;
			}
			return false;
		}

		private bool DecodeHuffman()
		{
			int num = outputWindow.GetFreeSpace();
			while (num >= 258)
			{
				switch (mode)
				{
				case 7:
				{
					int symbol;
					while (((symbol = litlenTree.GetSymbol(input)) & -256) == 0)
					{
						outputWindow.Write(symbol);
						if (--num < 258)
						{
							return true;
						}
					}
					if (symbol < 257)
					{
						if (symbol < 0)
						{
							return false;
						}
						distTree = null;
						litlenTree = null;
						mode = 2;
						return true;
					}
					try
					{
						repLength = CPLENS[symbol - 257];
						neededBits = CPLEXT[symbol - 257];
					}
					catch (Exception)
					{
						throw new SharpZipBaseException("Illegal rep length code");
					}
					goto case 8;
				}
				case 8:
					if (neededBits > 0)
					{
						mode = 8;
						int num2 = input.PeekBits(neededBits);
						if (num2 < 0)
						{
							return false;
						}
						input.DropBits(neededBits);
						repLength += num2;
					}
					mode = 9;
					goto case 9;
				case 9:
				{
					int symbol = distTree.GetSymbol(input);
					if (symbol < 0)
					{
						return false;
					}
					try
					{
						repDist = CPDIST[symbol];
						neededBits = CPDEXT[symbol];
					}
					catch (Exception)
					{
						throw new SharpZipBaseException("Illegal rep dist code");
					}
					goto case 10;
				}
				case 10:
					if (neededBits > 0)
					{
						mode = 10;
						int num3 = input.PeekBits(neededBits);
						if (num3 < 0)
						{
							return false;
						}
						input.DropBits(neededBits);
						repDist += num3;
					}
					break;
				default:
					throw new SharpZipBaseException("Inflater unknown mode");
				}
				outputWindow.Repeat(repLength, repDist);
				num -= repLength;
				mode = 7;
			}
			return true;
		}

		private bool DecodeChksum()
		{
			while (neededBits > 0)
			{
				int num = input.PeekBits(8);
				if (num < 0)
				{
					return false;
				}
				input.DropBits(8);
				readAdler = ((readAdler << 8) | num);
				neededBits -= 8;
			}
			if ((int)adler.Value != readAdler)
			{
				throw new SharpZipBaseException("Adler chksum doesn't match: " + (int)adler.Value + " vs. " + readAdler);
			}
			mode = 12;
			return false;
		}

		private bool Decode()
		{
			switch (mode)
			{
			case 0:
				return DecodeHeader();
			case 1:
				return DecodeDict();
			case 11:
				return DecodeChksum();
			case 2:
			{
				if (isLastBlock)
				{
					if (noHeader)
					{
						mode = 12;
						return false;
					}
					input.SkipToByteBoundary();
					neededBits = 32;
					mode = 11;
					return true;
				}
				int num = input.PeekBits(3);
				if (num < 0)
				{
					return false;
				}
				input.DropBits(3);
				if ((num & 1) != 0)
				{
					isLastBlock = true;
				}
				switch (num >> 1)
				{
				case 0:
					input.SkipToByteBoundary();
					mode = 3;
					break;
				case 1:
					litlenTree = InflaterHuffmanTree.defLitLenTree;
					distTree = InflaterHuffmanTree.defDistTree;
					mode = 7;
					break;
				case 2:
					dynHeader = new InflaterDynHeader();
					mode = 6;
					break;
				default:
					throw new SharpZipBaseException("Unknown block type " + num);
				}
				return true;
			}
			case 3:
				if ((uncomprLen = input.PeekBits(16)) < 0)
				{
					return false;
				}
				input.DropBits(16);
				mode = 4;
				goto case 4;
			case 4:
			{
				int num3 = input.PeekBits(16);
				if (num3 < 0)
				{
					return false;
				}
				input.DropBits(16);
				if (num3 != (uncomprLen ^ 0xFFFF))
				{
					throw new SharpZipBaseException("broken uncompressed block");
				}
				mode = 5;
				goto case 5;
			}
			case 5:
			{
				int num2 = outputWindow.CopyStored(input, uncomprLen);
				uncomprLen -= num2;
				if (uncomprLen == 0)
				{
					mode = 2;
					return true;
				}
				return !input.IsNeedingInput;
			}
			case 6:
				if (!dynHeader.Decode(input))
				{
					return false;
				}
				litlenTree = dynHeader.BuildLitLenTree();
				distTree = dynHeader.BuildDistTree();
				mode = 7;
				goto case 7;
			case 7:
			case 8:
			case 9:
			case 10:
				return DecodeHuffman();
			case 12:
				return false;
			default:
				throw new SharpZipBaseException("Inflater.Decode unknown mode");
			}
		}

		public void SetDictionary(byte[] buffer)
		{
			SetDictionary(buffer, 0, buffer.Length);
		}

		public void SetDictionary(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (!IsNeedingDictionary)
			{
				throw new InvalidOperationException("Dictionary is not needed");
			}
			adler.Update(buffer, index, count);
			if ((int)adler.Value != readAdler)
			{
				throw new SharpZipBaseException("Wrong adler checksum");
			}
			adler.Reset();
			outputWindow.CopyDict(buffer, index, count);
			mode = 2;
		}

		public void SetInput(byte[] buffer)
		{
			SetInput(buffer, 0, buffer.Length);
		}

		public void SetInput(byte[] buffer, int index, int count)
		{
			input.SetInput(buffer, index, count);
			totalIn += count;
		}

		public int Inflate(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return Inflate(buffer, 0, buffer.Length);
		}

		public int Inflate(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "count cannot be negative");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset cannot be negative");
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentException("count exceeds buffer bounds");
			}
			if (count == 0)
			{
				if (!IsFinished)
				{
					Decode();
				}
				return 0;
			}
			int num = 0;
			do
			{
				if (mode == 11)
				{
					continue;
				}
				int num2 = outputWindow.CopyOutput(buffer, offset, count);
				if (num2 > 0)
				{
					adler.Update(buffer, offset, num2);
					offset += num2;
					num += num2;
					totalOut += num2;
					count -= num2;
					if (count == 0)
					{
						return num;
					}
				}
			}
			while (Decode() || (outputWindow.GetAvailable() > 0 && mode != 11));
			return num;
		}
	}
}
