using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class Deflater
	{
		public const int BEST_COMPRESSION = 9;

		public const int BEST_SPEED = 1;

		public const int DEFAULT_COMPRESSION = -1;

		public const int NO_COMPRESSION = 0;

		public const int DEFLATED = 8;

		private const int IS_SETDICT = 1;

		private const int IS_FLUSHING = 4;

		private const int IS_FINISHING = 8;

		private const int INIT_STATE = 0;

		private const int SETDICT_STATE = 1;

		private const int BUSY_STATE = 16;

		private const int FLUSHING_STATE = 20;

		private const int FINISHING_STATE = 28;

		private const int FINISHED_STATE = 30;

		private const int CLOSED_STATE = 127;

		private int level;

		private bool noZlibHeaderOrFooter;

		private int state;

		private long totalOut;

		private DeflaterPending pending;

		private DeflaterEngine engine;

		public int Adler
		{
			get
			{
				return engine.Adler;
			}
		}

		public long TotalIn
		{
			get
			{
				return engine.TotalIn;
			}
		}

		public long TotalOut
		{
			get
			{
				return totalOut;
			}
		}

		public bool IsFinished
		{
			get
			{
				if (state == 30)
				{
					return pending.IsFlushed;
				}
				return false;
			}
		}

		public bool IsNeedingInput
		{
			get
			{
				return engine.NeedsInput();
			}
		}

		public Deflater()
			: this(-1, false)
		{
		}

		public Deflater(int level)
			: this(level, false)
		{
		}

		public Deflater(int level, bool noZlibHeaderOrFooter)
		{
			switch (level)
			{
			case -1:
				level = 6;
				break;
			default:
				throw new ArgumentOutOfRangeException("level");
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				break;
			}
			pending = new DeflaterPending();
			engine = new DeflaterEngine(pending);
			this.noZlibHeaderOrFooter = noZlibHeaderOrFooter;
			SetStrategy(DeflateStrategy.Default);
			SetLevel(level);
			Reset();
		}

		public void Reset()
		{
			state = (noZlibHeaderOrFooter ? 16 : 0);
			totalOut = 0L;
			pending.Reset();
			engine.Reset();
		}

		public void Flush()
		{
			state |= 4;
		}

		public void Finish()
		{
			state |= 12;
		}

		public void SetInput(byte[] input)
		{
			SetInput(input, 0, input.Length);
		}

		public void SetInput(byte[] input, int offset, int count)
		{
			if ((state & 8) != 0)
			{
				throw new InvalidOperationException("Finish() already called");
			}
			engine.SetInput(input, offset, count);
		}

		public void SetLevel(int level)
		{
			switch (level)
			{
			case -1:
				level = 6;
				break;
			default:
				throw new ArgumentOutOfRangeException("level");
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				break;
			}
			if (this.level != level)
			{
				this.level = level;
				engine.SetLevel(level);
			}
		}

		public int GetLevel()
		{
			return level;
		}

		public void SetStrategy(DeflateStrategy strategy)
		{
			engine.Strategy = strategy;
		}

		public int Deflate(byte[] output)
		{
			return Deflate(output, 0, output.Length);
		}

		public int Deflate(byte[] output, int offset, int length)
		{
			int num = length;
			if (state == 127)
			{
				throw new InvalidOperationException("Deflater closed");
			}
			if (state < 16)
			{
				int num2 = 30720;
				int num3 = level - 1 >> 1;
				if (num3 < 0 || num3 > 3)
				{
					num3 = 3;
				}
				num2 |= num3 << 6;
				if ((state & 1) != 0)
				{
					num2 |= 0x20;
				}
				num2 += 31 - num2 % 31;
				pending.WriteShortMSB(num2);
				if ((state & 1) != 0)
				{
					int adler = engine.Adler;
					engine.ResetAdler();
					pending.WriteShortMSB(adler >> 16);
					pending.WriteShortMSB(adler & 0xFFFF);
				}
				state = (0x10 | (state & 0xC));
			}
			while (true)
			{
				int num4 = pending.Flush(output, offset, length);
				offset += num4;
				totalOut += num4;
				length -= num4;
				if (length == 0 || state == 30)
				{
					break;
				}
				if (engine.Deflate((state & 4) != 0, (state & 8) != 0))
				{
					continue;
				}
				if (state == 16)
				{
					return num - length;
				}
				if (state == 20)
				{
					if (level != 0)
					{
						for (int num5 = 8 + (-pending.BitCount & 7); num5 > 0; num5 -= 10)
						{
							pending.WriteBits(2, 10);
						}
					}
					state = 16;
				}
				else if (state == 28)
				{
					pending.AlignToByte();
					if (!noZlibHeaderOrFooter)
					{
						int adler2 = engine.Adler;
						pending.WriteShortMSB(adler2 >> 16);
						pending.WriteShortMSB(adler2 & 0xFFFF);
					}
					state = 30;
				}
			}
			return num - length;
		}

		public void SetDictionary(byte[] dictionary)
		{
			SetDictionary(dictionary, 0, dictionary.Length);
		}

		public void SetDictionary(byte[] dictionary, int index, int count)
		{
			if (state != 0)
			{
				throw new InvalidOperationException();
			}
			state = 1;
			engine.SetDictionary(dictionary, index, count);
		}
	}
}
