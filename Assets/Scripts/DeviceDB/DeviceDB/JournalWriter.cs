using System;
using System.IO;
using System.Text;

namespace DeviceDB
{
	internal class JournalWriter
	{
		private readonly string path;

		private readonly IFileSystem fileSystem;

		private const byte FormatVersion = 0;

		private readonly Stream stream;

		private readonly BinaryWriter writer;

		private bool isStarted;

		private int numEntries;

		private bool isDisposed;

		public JournalWriter(string path, IFileSystem fileSystem)
		{
			this.path = path;
			this.fileSystem = fileSystem;
			stream = fileSystem.OpenFileStream(path);
			writer = new BinaryWriter(stream, Encoding.UTF8);
		}

		public void Start()
		{
			if (isStarted)
			{
				throw new InvalidOperationException("Need to call Finish() first");
			}
			stream.Seek(0L, SeekOrigin.Begin);
			stream.SetLength(10L);
			writer.Write((byte)0);
			writer.Write((byte)0);
			writer.Write(0);
			writer.Write(0);
			isStarted = true;
		}

		public void WriteResizeEntry(string filePath, uint size)
		{
			EnsureStarted();
			writer.Write((byte)0);
			writer.Write(filePath);
			writer.Write(size);
			numEntries++;
		}

		public void WriteWriteEntry(string filePath, uint filePosition, byte[] data)
		{
			EnsureStarted();
			writer.Write((byte)1);
			writer.Write(filePath);
			writer.Write(filePosition);
			writer.Write(data.Length);
			writer.Write(data);
			numEntries++;
		}

		public void WriteWriteEntry(string filePath, uint filePosition, byte[] data, uint len)
		{
			EnsureStarted();
			writer.Write((byte)1);
			writer.Write(filePath);
			writer.Write(filePosition);
			writer.Write(len);
			writer.Write(data, 0, (int)len);
			numEntries++;
		}

		public void WriteCopyEntry(string filePath, uint srcPosition, uint numBytes, uint destPosition)
		{
			EnsureStarted();
			writer.Write((byte)2);
			writer.Write(filePath);
			writer.Write(srcPosition);
			writer.Write(numBytes);
			writer.Write(destPosition);
			numEntries++;
		}

		public void Finish()
		{
			EnsureStarted();
			if (numEntries == 0)
			{
				stream.SetLength(0L);
			}
			else
			{
				stream.Seek(1L, SeekOrigin.Begin);
				writer.Write((byte)1);
				writer.Write(numEntries);
				writer.Flush();
				numEntries = 0;
			}
			isStarted = false;
		}

		public void Discard()
		{
			stream.Seek(0L, SeekOrigin.Begin);
			stream.SetLength(0L);
			numEntries = 0;
			isStarted = false;
		}

		public void Delete()
		{
			if (!isDisposed)
			{
				stream.Dispose();
				fileSystem.DeleteFile(path);
				isDisposed = true;
			}
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				writer.Close();
				stream.Dispose();
				isDisposed = true;
			}
		}

		private void EnsureStarted()
		{
			if (!isStarted)
			{
				throw new InvalidOperationException("Need to call Start() first");
			}
		}
	}
}
