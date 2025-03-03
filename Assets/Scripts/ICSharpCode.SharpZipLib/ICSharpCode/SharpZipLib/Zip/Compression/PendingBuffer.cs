using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class PendingBuffer
	{
		private byte[] buffer_;

		private int start;

		private int end;

		private uint bits;

		private int bitCount;

		public int BitCount
		{
			get
			{
				return bitCount;
			}
		}

		public bool IsFlushed
		{
			get
			{
				return end == 0;
			}
		}

		public PendingBuffer()
			: this(4096)
		{
		}

		public PendingBuffer(int bufferSize)
		{
			buffer_ = new byte[bufferSize];
		}

		public void Reset()
		{
			start = (end = (bitCount = 0));
		}

		public void WriteByte(int value)
		{
			buffer_[end++] = (byte)value;
		}

		public void WriteShort(int value)
		{
			buffer_[end++] = (byte)value;
			buffer_[end++] = (byte)(value >> 8);
		}

		public void WriteInt(int value)
		{
			buffer_[end++] = (byte)value;
			buffer_[end++] = (byte)(value >> 8);
			buffer_[end++] = (byte)(value >> 16);
			buffer_[end++] = (byte)(value >> 24);
		}

		public void WriteBlock(byte[] block, int offset, int length)
		{
			Array.Copy(block, offset, buffer_, end, length);
			end += length;
		}

		public void AlignToByte()
		{
			if (bitCount > 0)
			{
				buffer_[end++] = (byte)bits;
				if (bitCount > 8)
				{
					buffer_[end++] = (byte)(bits >> 8);
				}
			}
			bits = 0u;
			bitCount = 0;
		}

		public void WriteBits(int b, int count)
		{
			bits |= (uint)(b << bitCount);
			bitCount += count;
			if (bitCount >= 16)
			{
				buffer_[end++] = (byte)bits;
				buffer_[end++] = (byte)(bits >> 8);
				bits >>= 16;
				bitCount -= 16;
			}
		}

		public void WriteShortMSB(int s)
		{
			buffer_[end++] = (byte)(s >> 8);
			buffer_[end++] = (byte)s;
		}

		public int Flush(byte[] output, int offset, int length)
		{
			if (bitCount >= 8)
			{
				buffer_[end++] = (byte)bits;
				bits >>= 8;
				bitCount -= 8;
			}
			if (length > end - start)
			{
				length = end - start;
				Array.Copy(buffer_, start, output, offset, length);
				start = 0;
				end = 0;
			}
			else
			{
				Array.Copy(buffer_, start, output, offset, length);
				start += length;
			}
			return length;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[end - start];
			Array.Copy(buffer_, start, array, 0, array.Length);
			start = 0;
			end = 0;
			return array;
		}
	}
}
