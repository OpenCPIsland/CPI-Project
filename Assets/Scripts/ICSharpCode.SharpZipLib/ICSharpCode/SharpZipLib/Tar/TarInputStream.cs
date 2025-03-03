using System;
using System.IO;
using System.Text;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarInputStream : Stream
	{
		public interface IEntryFactory
		{
			TarEntry CreateEntry(string name);

			TarEntry CreateEntryFromFile(string fileName);

			TarEntry CreateEntry(byte[] headerBuffer);
		}

		public class EntryFactoryAdapter : IEntryFactory
		{
			public TarEntry CreateEntry(string name)
			{
				return TarEntry.CreateTarEntry(name);
			}

			public TarEntry CreateEntryFromFile(string fileName)
			{
				return TarEntry.CreateEntryFromFile(fileName);
			}

			public TarEntry CreateEntry(byte[] headerBuffer)
			{
				return new TarEntry(headerBuffer);
			}
		}

		protected bool hasHitEOF;

		protected long entrySize;

		protected long entryOffset;

		protected byte[] readBuffer;

		protected TarBuffer tarBuffer;

		private TarEntry currentEntry;

		protected IEntryFactory entryFactory;

		private readonly Stream inputStream;

		public bool IsStreamOwner
		{
			get
			{
				return tarBuffer.IsStreamOwner;
			}
			set
			{
				tarBuffer.IsStreamOwner = value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return inputStream.CanRead;
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
				return inputStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return inputStream.Position;
			}
			set
			{
				throw new NotSupportedException("TarInputStream Seek not supported");
			}
		}

		public int RecordSize
		{
			get
			{
				return tarBuffer.RecordSize;
			}
		}

		public long Available
		{
			get
			{
				return entrySize - entryOffset;
			}
		}

		public bool IsMarkSupported
		{
			get
			{
				return false;
			}
		}

		public TarInputStream(Stream inputStream)
			: this(inputStream, 20)
		{
		}

		public TarInputStream(Stream inputStream, int blockFactor)
		{
			this.inputStream = inputStream;
			tarBuffer = TarBuffer.CreateInputTarBuffer(inputStream, blockFactor);
		}

		public override void Flush()
		{
			inputStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("TarInputStream Seek not supported");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("TarInputStream SetLength not supported");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("TarInputStream Write not supported");
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("TarInputStream WriteByte not supported");
		}

		public override int ReadByte()
		{
			byte[] array = new byte[1];
			int num = Read(array, 0, 1);
			if (num <= 0)
			{
				return -1;
			}
			return array[0];
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = 0;
			if (entryOffset >= entrySize)
			{
				return 0;
			}
			long num2 = count;
			if (num2 + entryOffset > entrySize)
			{
				num2 = entrySize - entryOffset;
			}
			if (readBuffer != null)
			{
				int num3 = (int)((num2 > readBuffer.Length) ? readBuffer.Length : num2);
				Array.Copy(readBuffer, 0, buffer, offset, num3);
				if (num3 >= readBuffer.Length)
				{
					readBuffer = null;
				}
				else
				{
					int num4 = readBuffer.Length - num3;
					byte[] destinationArray = new byte[num4];
					Array.Copy(readBuffer, num3, destinationArray, 0, num4);
					readBuffer = destinationArray;
				}
				num += num3;
				num2 -= num3;
				offset += num3;
			}
			while (num2 > 0)
			{
				byte[] array = tarBuffer.ReadBlock();
				if (array == null)
				{
					throw new TarException("unexpected EOF with " + num2 + " bytes unread");
				}
				int num5 = (int)num2;
				int num6 = array.Length;
				if (num6 > num5)
				{
					Array.Copy(array, 0, buffer, offset, num5);
					readBuffer = new byte[num6 - num5];
					Array.Copy(array, num5, readBuffer, 0, num6 - num5);
				}
				else
				{
					num5 = num6;
					Array.Copy(array, 0, buffer, offset, num6);
				}
				num += num5;
				num2 -= num5;
				offset += num5;
			}
			entryOffset += num;
			return num;
		}

		public override void Close()
		{
			tarBuffer.Close();
		}

		public void SetEntryFactory(IEntryFactory factory)
		{
			entryFactory = factory;
		}

		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return tarBuffer.RecordSize;
		}

		public void Skip(long skipCount)
		{
			byte[] array = new byte[8192];
			long num = skipCount;
			while (num > 0)
			{
				int count = (int)((num > array.Length) ? array.Length : num);
				int num2 = Read(array, 0, count);
				if (num2 == -1)
				{
					break;
				}
				num -= num2;
			}
		}

		public void Mark(int markLimit)
		{
		}

		public void Reset()
		{
		}

		public TarEntry GetNextEntry()
		{
			if (hasHitEOF)
			{
				return null;
			}
			if (currentEntry != null)
			{
				SkipToNextEntry();
			}
			byte[] array = tarBuffer.ReadBlock();
			if (array == null)
			{
				hasHitEOF = true;
			}
			else if (TarBuffer.IsEndOfArchiveBlock(array))
			{
				hasHitEOF = true;
			}
			if (hasHitEOF)
			{
				currentEntry = null;
			}
			else
			{
				try
				{
					TarHeader tarHeader = new TarHeader();
					tarHeader.ParseBuffer(array);
					if (!tarHeader.IsChecksumValid)
					{
						throw new TarException("Header checksum is invalid");
					}
					entryOffset = 0L;
					entrySize = tarHeader.Size;
					StringBuilder stringBuilder = null;
					if (tarHeader.TypeFlag == 76)
					{
						byte[] array2 = new byte[512];
						long num = entrySize;
						stringBuilder = new StringBuilder();
						while (num > 0)
						{
							int num2 = Read(array2, 0, (int)((num > array2.Length) ? array2.Length : num));
							if (num2 == -1)
							{
								throw new InvalidHeaderException("Failed to read long name entry");
							}
							stringBuilder.Append(TarHeader.ParseName(array2, 0, num2).ToString());
							num -= num2;
						}
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag == 103)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag == 120)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag == 86)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					else if (tarHeader.TypeFlag != 48 && tarHeader.TypeFlag != 0 && tarHeader.TypeFlag != 53)
					{
						SkipToNextEntry();
						array = tarBuffer.ReadBlock();
					}
					if (entryFactory == null)
					{
						currentEntry = new TarEntry(array);
						if (stringBuilder != null)
						{
							currentEntry.Name = stringBuilder.ToString();
						}
					}
					else
					{
						currentEntry = entryFactory.CreateEntry(array);
					}
					entryOffset = 0L;
					entrySize = currentEntry.Size;
				}
				catch (InvalidHeaderException ex)
				{
					entrySize = 0L;
					entryOffset = 0L;
					currentEntry = null;
					string message = string.Format("Bad header in record {0} block {1} {2}", tarBuffer.CurrentRecord, tarBuffer.CurrentBlock, ex.Message);
					throw new InvalidHeaderException(message);
				}
			}
			return currentEntry;
		}

		public void CopyEntryContents(Stream outputStream)
		{
			byte[] array = new byte[32768];
			while (true)
			{
				int num = Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				outputStream.Write(array, 0, num);
			}
		}

		private void SkipToNextEntry()
		{
			long num = entrySize - entryOffset;
			if (num > 0)
			{
				Skip(num);
			}
			readBuffer = null;
		}
	}
}
