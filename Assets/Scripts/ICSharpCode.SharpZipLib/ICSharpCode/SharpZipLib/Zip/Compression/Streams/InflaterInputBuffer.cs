using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	public class InflaterInputBuffer
	{
		private int rawLength;

		private byte[] rawData;

		private int clearTextLength;

		private byte[] clearText;

		private byte[] internalClearText;

		private int available;

		private ICryptoTransform cryptoTransform;

		private Stream inputStream;

		public int RawLength
		{
			get
			{
				return rawLength;
			}
		}

		public byte[] RawData
		{
			get
			{
				return rawData;
			}
		}

		public int ClearTextLength
		{
			get
			{
				return clearTextLength;
			}
		}

		public byte[] ClearText
		{
			get
			{
				return clearText;
			}
		}

		public int Available
		{
			get
			{
				return available;
			}
			set
			{
				available = value;
			}
		}

		public ICryptoTransform CryptoTransform
		{
			set
			{
				cryptoTransform = value;
				if (cryptoTransform != null)
				{
					if (rawData == clearText)
					{
						if (internalClearText == null)
						{
							internalClearText = new byte[rawData.Length];
						}
						clearText = internalClearText;
					}
					clearTextLength = rawLength;
					if (available > 0)
					{
						cryptoTransform.TransformBlock(rawData, rawLength - available, available, clearText, rawLength - available);
					}
				}
				else
				{
					clearText = rawData;
					clearTextLength = rawLength;
				}
			}
		}

		public InflaterInputBuffer(Stream stream)
			: this(stream, 4096)
		{
		}

		public InflaterInputBuffer(Stream stream, int bufferSize)
		{
			inputStream = stream;
			if (bufferSize < 1024)
			{
				bufferSize = 1024;
			}
			rawData = new byte[bufferSize];
			clearText = rawData;
		}

		public void SetInflaterInput(Inflater inflater)
		{
			if (available > 0)
			{
				inflater.SetInput(clearText, clearTextLength - available, available);
				available = 0;
			}
		}

		public void Fill()
		{
			rawLength = 0;
			int num = rawData.Length;
			while (num > 0)
			{
				int num2 = inputStream.Read(rawData, rawLength, num);
				if (num2 <= 0)
				{
					break;
				}
				rawLength += num2;
				num -= num2;
			}
			if (cryptoTransform != null)
			{
				clearTextLength = cryptoTransform.TransformBlock(rawData, 0, rawLength, clearText, 0);
			}
			else
			{
				clearTextLength = rawLength;
			}
			available = clearTextLength;
		}

		public int ReadRawBuffer(byte[] buffer)
		{
			return ReadRawBuffer(buffer, 0, buffer.Length);
		}

		public int ReadRawBuffer(byte[] outBuffer, int offset, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = offset;
			int num2 = length;
			while (num2 > 0)
			{
				if (available <= 0)
				{
					Fill();
					if (available <= 0)
					{
						return 0;
					}
				}
				int num3 = Math.Min(num2, available);
				Array.Copy(rawData, rawLength - available, outBuffer, num, num3);
				num += num3;
				num2 -= num3;
				available -= num3;
			}
			return length;
		}

		public int ReadClearTextBuffer(byte[] outBuffer, int offset, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = offset;
			int num2 = length;
			while (num2 > 0)
			{
				if (available <= 0)
				{
					Fill();
					if (available <= 0)
					{
						return 0;
					}
				}
				int num3 = Math.Min(num2, available);
				Array.Copy(clearText, clearTextLength - available, outBuffer, num, num3);
				num += num3;
				num2 -= num3;
				available -= num3;
			}
			return length;
		}

		public int ReadLeByte()
		{
			if (available <= 0)
			{
				Fill();
				if (available <= 0)
				{
					throw new ZipException("EOF in header");
				}
			}
			byte result = rawData[rawLength - available];
			available--;
			return result;
		}

		public int ReadLeShort()
		{
			return ReadLeByte() | (ReadLeByte() << 8);
		}

		public int ReadLeInt()
		{
			return ReadLeShort() | (ReadLeShort() << 16);
		}

		public long ReadLeLong()
		{
			return (uint)ReadLeInt() | ((long)ReadLeInt() << 32);
		}
	}
}
