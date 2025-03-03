using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarBuffer
	{
		public const int BlockSize = 512;

		public const int DefaultBlockFactor = 20;

		public const int DefaultRecordSize = 10240;

		private Stream inputStream;

		private Stream outputStream;

		private byte[] recordBuffer;

		private int currentBlockIndex;

		private int currentRecordIndex;

		private int recordSize = 10240;

		private int blockFactor = 20;

		private bool isStreamOwner_ = true;

		public int RecordSize
		{
			get
			{
				return recordSize;
			}
		}

		public int BlockFactor
		{
			get
			{
				return blockFactor;
			}
		}

		public int CurrentBlock
		{
			get
			{
				return currentBlockIndex;
			}
		}

		public bool IsStreamOwner
		{
			get
			{
				return isStreamOwner_;
			}
			set
			{
				isStreamOwner_ = value;
			}
		}

		public int CurrentRecord
		{
			get
			{
				return currentRecordIndex;
			}
		}

		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return recordSize;
		}

		[Obsolete("Use BlockFactor property instead")]
		public int GetBlockFactor()
		{
			return blockFactor;
		}

		protected TarBuffer()
		{
		}

		public static TarBuffer CreateInputTarBuffer(Stream inputStream)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			return CreateInputTarBuffer(inputStream, 20);
		}

		public static TarBuffer CreateInputTarBuffer(Stream inputStream, int blockFactor)
		{
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			if (blockFactor <= 0)
			{
				throw new ArgumentOutOfRangeException("blockFactor", "Factor cannot be negative");
			}
			TarBuffer tarBuffer = new TarBuffer();
			tarBuffer.inputStream = inputStream;
			tarBuffer.outputStream = null;
			tarBuffer.Initialize(blockFactor);
			return tarBuffer;
		}

		public static TarBuffer CreateOutputTarBuffer(Stream outputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			return CreateOutputTarBuffer(outputStream, 20);
		}

		public static TarBuffer CreateOutputTarBuffer(Stream outputStream, int blockFactor)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (blockFactor <= 0)
			{
				throw new ArgumentOutOfRangeException("blockFactor", "Factor cannot be negative");
			}
			TarBuffer tarBuffer = new TarBuffer();
			tarBuffer.inputStream = null;
			tarBuffer.outputStream = outputStream;
			tarBuffer.Initialize(blockFactor);
			return tarBuffer;
		}

		private void Initialize(int archiveBlockFactor)
		{
			blockFactor = archiveBlockFactor;
			recordSize = archiveBlockFactor * 512;
			recordBuffer = new byte[RecordSize];
			if (inputStream != null)
			{
				currentRecordIndex = -1;
				currentBlockIndex = BlockFactor;
			}
			else
			{
				currentRecordIndex = 0;
				currentBlockIndex = 0;
			}
		}

		[Obsolete("Use IsEndOfArchiveBlock instead")]
		public bool IsEOFBlock(byte[] block)
		{
			if (block == null)
			{
				throw new ArgumentNullException("block");
			}
			if (block.Length != 512)
			{
				throw new ArgumentException("block length is invalid");
			}
			for (int i = 0; i < 512; i++)
			{
				if (block[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsEndOfArchiveBlock(byte[] block)
		{
			if (block == null)
			{
				throw new ArgumentNullException("block");
			}
			if (block.Length != 512)
			{
				throw new ArgumentException("block length is invalid");
			}
			for (int i = 0; i < 512; i++)
			{
				if (block[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		public void SkipBlock()
		{
			if (inputStream == null)
			{
				throw new TarException("no input stream defined");
			}
			if (currentBlockIndex >= BlockFactor && !ReadRecord())
			{
				throw new TarException("Failed to read a record");
			}
			currentBlockIndex++;
		}

		public byte[] ReadBlock()
		{
			if (inputStream == null)
			{
				throw new TarException("TarBuffer.ReadBlock - no input stream defined");
			}
			if (currentBlockIndex >= BlockFactor && !ReadRecord())
			{
				throw new TarException("Failed to read a record");
			}
			byte[] array = new byte[512];
			Array.Copy(recordBuffer, currentBlockIndex * 512, array, 0, 512);
			currentBlockIndex++;
			return array;
		}

		private bool ReadRecord()
		{
			if (inputStream == null)
			{
				throw new TarException("no input stream stream defined");
			}
			currentBlockIndex = 0;
			int num = 0;
			int num2 = RecordSize;
			while (num2 > 0)
			{
				long num3 = inputStream.Read(recordBuffer, num, num2);
				if (num3 <= 0)
				{
					break;
				}
				num += (int)num3;
				num2 -= (int)num3;
			}
			currentRecordIndex++;
			return true;
		}

		[Obsolete("Use CurrentBlock property instead")]
		public int GetCurrentBlockNum()
		{
			return currentBlockIndex;
		}

		[Obsolete("Use CurrentRecord property instead")]
		public int GetCurrentRecordNum()
		{
			return currentRecordIndex;
		}

		public void WriteBlock(byte[] block)
		{
			if (block == null)
			{
				throw new ArgumentNullException("block");
			}
			if (outputStream == null)
			{
				throw new TarException("TarBuffer.WriteBlock - no output stream defined");
			}
			if (block.Length != 512)
			{
				string message = string.Format("TarBuffer.WriteBlock - block to write has length '{0}' which is not the block size of '{1}'", block.Length, 512);
				throw new TarException(message);
			}
			if (currentBlockIndex >= BlockFactor)
			{
				WriteRecord();
			}
			Array.Copy(block, 0, recordBuffer, currentBlockIndex * 512, 512);
			currentBlockIndex++;
		}

		public void WriteBlock(byte[] buffer, int offset)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (outputStream == null)
			{
				throw new TarException("TarBuffer.WriteBlock - no output stream stream defined");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset + 512 > buffer.Length)
			{
				string message = string.Format("TarBuffer.WriteBlock - record has length '{0}' with offset '{1}' which is less than the record size of '{2}'", buffer.Length, offset, recordSize);
				throw new TarException(message);
			}
			if (currentBlockIndex >= BlockFactor)
			{
				WriteRecord();
			}
			Array.Copy(buffer, offset, recordBuffer, currentBlockIndex * 512, 512);
			currentBlockIndex++;
		}

		private void WriteRecord()
		{
			if (outputStream == null)
			{
				throw new TarException("TarBuffer.WriteRecord no output stream defined");
			}
			outputStream.Write(recordBuffer, 0, RecordSize);
			outputStream.Flush();
			currentBlockIndex = 0;
			currentRecordIndex++;
		}

		private void WriteFinalRecord()
		{
			if (outputStream == null)
			{
				throw new TarException("TarBuffer.WriteFinalRecord no output stream defined");
			}
			if (currentBlockIndex > 0)
			{
				int num = currentBlockIndex * 512;
				Array.Clear(recordBuffer, num, RecordSize - num);
				WriteRecord();
			}
			outputStream.Flush();
		}

		public void Close()
		{
			if (outputStream != null)
			{
				WriteFinalRecord();
				if (isStreamOwner_)
				{
					outputStream.Close();
				}
				outputStream = null;
			}
			else if (inputStream != null)
			{
				if (isStreamOwner_)
				{
					inputStream.Close();
				}
				inputStream = null;
			}
		}
	}
}
