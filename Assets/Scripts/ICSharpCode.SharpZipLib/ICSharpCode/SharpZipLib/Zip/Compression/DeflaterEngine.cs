using ICSharpCode.SharpZipLib.Checksums;
using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class DeflaterEngine : DeflaterConstants
	{
		private const int TooFar = 4096;

		private int ins_h;

		private short[] head;

		private short[] prev;

		private int matchStart;

		private int matchLen;

		private bool prevAvailable;

		private int blockStart;

		private int strstart;

		private int lookahead;

		private byte[] window;

		private DeflateStrategy strategy;

		private int max_chain;

		private int max_lazy;

		private int niceLength;

		private int goodLength;

		private int compressionFunction;

		private byte[] inputBuf;

		private long totalIn;

		private int inputOff;

		private int inputEnd;

		private DeflaterPending pending;

		private DeflaterHuffman huffman;

		private Adler32 adler;

		public int Adler
		{
			get
			{
				return (int)adler.Value;
			}
		}

		public long TotalIn
		{
			get
			{
				return totalIn;
			}
		}

		public DeflateStrategy Strategy
		{
			get
			{
				return strategy;
			}
			set
			{
				strategy = value;
			}
		}

		public DeflaterEngine(DeflaterPending pending)
		{
			this.pending = pending;
			huffman = new DeflaterHuffman(pending);
			adler = new Adler32();
			window = new byte[65536];
			head = new short[32768];
			prev = new short[32768];
			blockStart = (strstart = 1);
		}

		public bool Deflate(bool flush, bool finish)
		{
			bool flag;
			do
			{
				FillWindow();
				bool flush2 = flush && inputOff == inputEnd;
				switch (compressionFunction)
				{
				case 0:
					flag = DeflateStored(flush2, finish);
					break;
				case 1:
					flag = DeflateFast(flush2, finish);
					break;
				case 2:
					flag = DeflateSlow(flush2, finish);
					break;
				default:
					throw new InvalidOperationException("unknown compressionFunction");
				}
			}
			while (pending.IsFlushed && flag);
			return flag;
		}

		public void SetInput(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (inputOff < inputEnd)
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}
			int num = offset + count;
			if (offset > num || num > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			inputBuf = buffer;
			inputOff = offset;
			inputEnd = num;
		}

		public bool NeedsInput()
		{
			return inputEnd == inputOff;
		}

		public void SetDictionary(byte[] buffer, int offset, int length)
		{
			adler.Update(buffer, offset, length);
			if (length >= 3)
			{
				if (length > 32506)
				{
					offset += length - 32506;
					length = 32506;
				}
				Array.Copy(buffer, offset, window, strstart, length);
				UpdateHash();
				length--;
				while (--length > 0)
				{
					InsertString();
					strstart++;
				}
				strstart += 2;
				blockStart = strstart;
			}
		}

		public void Reset()
		{
			huffman.Reset();
			adler.Reset();
			blockStart = (strstart = 1);
			lookahead = 0;
			totalIn = 0L;
			prevAvailable = false;
			matchLen = 2;
			for (int i = 0; i < 32768; i++)
			{
				head[i] = 0;
			}
			for (int j = 0; j < 32768; j++)
			{
				prev[j] = 0;
			}
		}

		public void ResetAdler()
		{
			adler.Reset();
		}

		public void SetLevel(int level)
		{
			if (level < 0 || level > 9)
			{
				throw new ArgumentOutOfRangeException("level");
			}
			goodLength = DeflaterConstants.GOOD_LENGTH[level];
			max_lazy = DeflaterConstants.MAX_LAZY[level];
			niceLength = DeflaterConstants.NICE_LENGTH[level];
			max_chain = DeflaterConstants.MAX_CHAIN[level];
			if (DeflaterConstants.COMPR_FUNC[level] == compressionFunction)
			{
				return;
			}
			switch (compressionFunction)
			{
			case 0:
				if (strstart > blockStart)
				{
					huffman.FlushStoredBlock(window, blockStart, strstart - blockStart, false);
					blockStart = strstart;
				}
				UpdateHash();
				break;
			case 1:
				if (strstart > blockStart)
				{
					huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
					blockStart = strstart;
				}
				break;
			case 2:
				if (prevAvailable)
				{
					huffman.TallyLit(window[strstart - 1] & 0xFF);
				}
				if (strstart > blockStart)
				{
					huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
					blockStart = strstart;
				}
				prevAvailable = false;
				matchLen = 2;
				break;
			}
			compressionFunction = DeflaterConstants.COMPR_FUNC[level];
		}

		public void FillWindow()
		{
			if (strstart >= 65274)
			{
				SlideWindow();
			}
			while (lookahead < 262 && inputOff < inputEnd)
			{
				int num = 65536 - lookahead - strstart;
				if (num > inputEnd - inputOff)
				{
					num = inputEnd - inputOff;
				}
				Array.Copy(inputBuf, inputOff, window, strstart + lookahead, num);
				adler.Update(inputBuf, inputOff, num);
				inputOff += num;
				totalIn += num;
				lookahead += num;
			}
			if (lookahead >= 3)
			{
				UpdateHash();
			}
		}

		private void UpdateHash()
		{
			ins_h = ((window[strstart] << 5) ^ window[strstart + 1]);
		}

		private int InsertString()
		{
			int num = ((ins_h << 5) ^ window[strstart + 2]) & 0x7FFF;
			short num2 = prev[strstart & 0x7FFF] = head[num];
			head[num] = (short)strstart;
			ins_h = num;
			return num2 & 0xFFFF;
		}

		private void SlideWindow()
		{
			Array.Copy(window, 32768, window, 0, 32768);
			matchStart -= 32768;
			strstart -= 32768;
			blockStart -= 32768;
			for (int i = 0; i < 32768; i++)
			{
				int num = head[i] & 0xFFFF;
				head[i] = (short)((num >= 32768) ? (num - 32768) : 0);
			}
			for (int j = 0; j < 32768; j++)
			{
				int num2 = prev[j] & 0xFFFF;
				prev[j] = (short)((num2 >= 32768) ? (num2 - 32768) : 0);
			}
		}

		private bool FindLongestMatch(int curMatch)
		{
			int num = max_chain;
			int num2 = niceLength;
			short[] array = prev;
			int num3 = strstart;
			int num4 = strstart + matchLen;
			int num5 = Math.Max(matchLen, 2);
			int num6 = Math.Max(strstart - 32506, 0);
			int num7 = strstart + 258 - 1;
			byte b = window[num4 - 1];
			byte b2 = window[num4];
			if (num5 >= goodLength)
			{
				num >>= 2;
			}
			if (num2 > lookahead)
			{
				num2 = lookahead;
			}
			do
			{
				if (window[curMatch + num5] != b2 || window[curMatch + num5 - 1] != b || window[curMatch] != window[num3] || window[curMatch + 1] != window[num3 + 1])
				{
					continue;
				}
				int num8 = curMatch + 2;
				num3 += 2;
				while (window[++num3] == window[++num8] && window[++num3] == window[++num8] && window[++num3] == window[++num8] && window[++num3] == window[++num8] && window[++num3] == window[++num8] && window[++num3] == window[++num8] && window[++num3] == window[++num8] && window[++num3] == window[++num8] && num3 < num7)
				{
				}
				if (num3 > num4)
				{
					matchStart = curMatch;
					num4 = num3;
					num5 = num3 - strstart;
					if (num5 >= num2)
					{
						break;
					}
					b = window[num4 - 1];
					b2 = window[num4];
				}
				num3 = strstart;
			}
			while ((curMatch = (array[curMatch & 0x7FFF] & 0xFFFF)) > num6 && --num != 0);
			matchLen = Math.Min(num5, lookahead);
			return matchLen >= 3;
		}

		private bool DeflateStored(bool flush, bool finish)
		{
			if (!flush && lookahead == 0)
			{
				return false;
			}
			strstart += lookahead;
			lookahead = 0;
			int num = strstart - blockStart;
			if (num >= DeflaterConstants.MAX_BLOCK_SIZE || (blockStart < 32768 && num >= 32506) || flush)
			{
				bool flag = finish;
				if (num > DeflaterConstants.MAX_BLOCK_SIZE)
				{
					num = DeflaterConstants.MAX_BLOCK_SIZE;
					flag = false;
				}
				huffman.FlushStoredBlock(window, blockStart, num, flag);
				blockStart += num;
				return !flag;
			}
			return true;
		}

		private bool DeflateFast(bool flush, bool finish)
		{
			if (lookahead < 262 && !flush)
			{
				return false;
			}
			while (lookahead >= 262 || flush)
			{
				if (lookahead == 0)
				{
					huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
					blockStart = strstart;
					return false;
				}
				if (strstart > 65274)
				{
					SlideWindow();
				}
				int num;
				if (lookahead >= 3 && (num = InsertString()) != 0 && strategy != DeflateStrategy.HuffmanOnly && strstart - num <= 32506 && FindLongestMatch(num))
				{
					bool flag = huffman.TallyDist(strstart - matchStart, matchLen);
					lookahead -= matchLen;
					if (matchLen <= max_lazy && lookahead >= 3)
					{
						while (--matchLen > 0)
						{
							strstart++;
							InsertString();
						}
						strstart++;
					}
					else
					{
						strstart += matchLen;
						if (lookahead >= 2)
						{
							UpdateHash();
						}
					}
					matchLen = 2;
					if (!flag)
					{
						continue;
					}
				}
				else
				{
					huffman.TallyLit(window[strstart] & 0xFF);
					strstart++;
					lookahead--;
				}
				if (huffman.IsFull())
				{
					bool flag2 = finish && lookahead == 0;
					huffman.FlushBlock(window, blockStart, strstart - blockStart, flag2);
					blockStart = strstart;
					return !flag2;
				}
			}
			return true;
		}

		private bool DeflateSlow(bool flush, bool finish)
		{
			if (lookahead < 262 && !flush)
			{
				return false;
			}
			while (lookahead >= 262 || flush)
			{
				if (lookahead == 0)
				{
					if (prevAvailable)
					{
						huffman.TallyLit(window[strstart - 1] & 0xFF);
					}
					prevAvailable = false;
					huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
					blockStart = strstart;
					return false;
				}
				if (strstart >= 65274)
				{
					SlideWindow();
				}
				int num = matchStart;
				int num2 = matchLen;
				if (lookahead >= 3)
				{
					int num3 = InsertString();
					if (strategy != DeflateStrategy.HuffmanOnly && num3 != 0 && strstart - num3 <= 32506 && FindLongestMatch(num3) && matchLen <= 5 && (strategy == DeflateStrategy.Filtered || (matchLen == 3 && strstart - matchStart > 4096)))
					{
						matchLen = 2;
					}
				}
				if (num2 >= 3 && matchLen <= num2)
				{
					huffman.TallyDist(strstart - 1 - num, num2);
					num2 -= 2;
					do
					{
						strstart++;
						lookahead--;
						if (lookahead >= 3)
						{
							InsertString();
						}
					}
					while (--num2 > 0);
					strstart++;
					lookahead--;
					prevAvailable = false;
					matchLen = 2;
				}
				else
				{
					if (prevAvailable)
					{
						huffman.TallyLit(window[strstart - 1] & 0xFF);
					}
					prevAvailable = true;
					strstart++;
					lookahead--;
				}
				if (huffman.IsFull())
				{
					int num4 = strstart - blockStart;
					if (prevAvailable)
					{
						num4--;
					}
					bool flag = finish && lookahead == 0 && !prevAvailable;
					huffman.FlushBlock(window, blockStart, num4, flag);
					blockStart += num4;
					return !flag;
				}
			}
			return true;
		}
	}
}
