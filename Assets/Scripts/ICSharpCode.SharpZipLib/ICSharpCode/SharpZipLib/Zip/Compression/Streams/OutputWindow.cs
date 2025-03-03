using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	public class OutputWindow
	{
		private const int WindowSize = 32768;

		private const int WindowMask = 32767;

		private byte[] window = new byte[32768];

		private int windowEnd;

		private int windowFilled;

		public void Write(int value)
		{
			if (windowFilled++ == 32768)
			{
				throw new InvalidOperationException("Window full");
			}
			window[windowEnd++] = (byte)value;
			windowEnd &= 32767;
		}

		private void SlowRepeat(int repStart, int length, int distance)
		{
			while (length-- > 0)
			{
				window[windowEnd++] = window[repStart++];
				windowEnd &= 32767;
				repStart &= 0x7FFF;
			}
		}

		public void Repeat(int length, int distance)
		{
			if ((windowFilled += length) > 32768)
			{
				throw new InvalidOperationException("Window full");
			}
			int num = (windowEnd - distance) & 0x7FFF;
			int num2 = 32768 - length;
			if (num <= num2 && windowEnd < num2)
			{
				if (length <= distance)
				{
					Array.Copy(window, num, window, windowEnd, length);
					windowEnd += length;
					return;
				}
				while (length-- > 0)
				{
					window[windowEnd++] = window[num++];
				}
			}
			else
			{
				SlowRepeat(num, length, distance);
			}
		}

		public int CopyStored(StreamManipulator input, int length)
		{
			length = Math.Min(Math.Min(length, 32768 - windowFilled), input.AvailableBytes);
			int num = 32768 - windowEnd;
			int num2;
			if (length > num)
			{
				num2 = input.CopyBytes(window, windowEnd, num);
				if (num2 == num)
				{
					num2 += input.CopyBytes(window, 0, length - num);
				}
			}
			else
			{
				num2 = input.CopyBytes(window, windowEnd, length);
			}
			windowEnd = ((windowEnd + num2) & 0x7FFF);
			windowFilled += num2;
			return num2;
		}

		public void CopyDict(byte[] dictionary, int offset, int length)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (windowFilled > 0)
			{
				throw new InvalidOperationException();
			}
			if (length > 32768)
			{
				offset += length - 32768;
				length = 32768;
			}
			Array.Copy(dictionary, offset, window, 0, length);
			windowEnd = (length & 0x7FFF);
		}

		public int GetFreeSpace()
		{
			return 32768 - windowFilled;
		}

		public int GetAvailable()
		{
			return windowFilled;
		}

		public int CopyOutput(byte[] output, int offset, int len)
		{
			int num = windowEnd;
			if (len > windowFilled)
			{
				len = windowFilled;
			}
			else
			{
				num = ((windowEnd - windowFilled + len) & 0x7FFF);
			}
			int num2 = len;
			int num3 = len - num;
			if (num3 > 0)
			{
				Array.Copy(window, 32768 - num3, output, offset, num3);
				offset += num3;
				len = num;
			}
			Array.Copy(window, num - len, output, offset, len);
			windowFilled -= num2;
			if (windowFilled < 0)
			{
				throw new InvalidOperationException();
			}
			return num2;
		}

		public void Reset()
		{
			windowFilled = (windowEnd = 0);
		}
	}
}
