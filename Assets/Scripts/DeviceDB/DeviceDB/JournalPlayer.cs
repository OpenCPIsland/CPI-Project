using System;
using System.IO;
using System.Text;

namespace DeviceDB
{
	internal class JournalPlayer
	{
		private struct Header
		{
			public JournalStatus Status;

			public int CurEntryIndex;

			public int NumEntries;
		}

		private struct ResizeEntry
		{
			public int Length;
		}

		private struct WriteEntry
		{
			public int FilePosition;

			public byte[] Bytes;

			public int Size;
		}

		private struct CopyEntry
		{
			public int SrcPosition;

			public int Size;

			public int DestPosition;
		}

		private const byte FormatVersion = 0;

		private const int HeaderSize = 10;

		private const int CurEntryIndexPosition = 6;

		public const uint CopyBufferSize = 32768u;

		private static readonly byte[] buffer = new byte[32768];

		private readonly IFileSystem fileSystem;

		private readonly Stream journalStream;

		private readonly BinaryReader reader;

		private readonly BinaryWriter writer;

		private bool isDisposed;

		public JournalPlayer(string path, IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			journalStream = fileSystem.OpenFileStream(path);
			writer = new BinaryWriter(journalStream, Encoding.UTF8);
			reader = new BinaryReader(journalStream, Encoding.UTF8);
		}

		public void Play()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("Can't play a disposed journal");
			}
			if (journalStream.Length >= 10)
			{
				journalStream.Position = 0L;
				Header header = ReadHeader();
				if (header.Status != JournalStatus.WritingDone || header.CurEntryIndex >= header.NumEntries)
				{
					journalStream.SetLength(0L);
				}
				else
				{
					try
					{
						SkipToEntryIndex(header.CurEntryIndex);
						ApplyEntries(header.CurEntryIndex, header.NumEntries);
						if (journalStream.Position != journalStream.Length)
						{
							throw new CorruptionException("Playback didn't reach end of journal");
						}
					}
					catch (FormatException innerException)
					{
						throw new CorruptionException("Format error during journal playback", innerException);
					}
					catch (IOException)
					{
					}
					finally
					{
						journalStream.SetLength(0L);
					}
				}
			}
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				writer.Close();
				reader.Close();
				isDisposed = true;
			}
		}

		private Header ReadHeader()
		{
			byte b = reader.ReadByte();
			if (b != 0)
			{
				throw new CorruptionException("Unhandled format version: " + b);
			}
			Header result = default(Header);
			result.Status = (JournalStatus)reader.ReadByte();
			result.NumEntries = reader.ReadInt32();
			result.CurEntryIndex = reader.ReadInt32();
			return result;
		}

		private void SkipToEntryIndex(int curEntryIndex)
		{
			for (int i = 0; i < curEntryIndex; i++)
			{
				JournalEntryType journalEntryType = (JournalEntryType)reader.ReadByte();
				switch (journalEntryType)
				{
				case JournalEntryType.Resize:
					reader.ReadString();
					ReadResizeEntry();
					break;
				case JournalEntryType.Write:
					reader.ReadString();
					SkipWriteEntry();
					break;
				case JournalEntryType.Copy:
					reader.ReadString();
					ReadCopyEntry();
					break;
				default:
					throw new CorruptionException("Unhandled entry type: " + journalEntryType);
				}
			}
		}

		private ResizeEntry ReadResizeEntry()
		{
			ResizeEntry result = default(ResizeEntry);
			result.Length = reader.ReadInt32();
			return result;
		}

		private void SkipWriteEntry()
		{
			reader.ReadInt32();
			int num = reader.ReadInt32();
			journalStream.Position += num;
		}

		private WriteEntry ReadWriteEntry()
		{
			WriteEntry writeEntry = default(WriteEntry);
			WriteEntry writeEntry2 = writeEntry;
			writeEntry2.FilePosition = reader.ReadInt32();
			writeEntry2.Size = reader.ReadInt32();
			writeEntry = writeEntry2;
			if ((long)writeEntry.Size <= 32768L)
			{
				int num = writeEntry.Size;
				int num2 = 0;
				do
				{
					int num3 = reader.Read(buffer, num2, num);
					num -= num3;
					num2 += num3;
				}
				while (num > 0);
				writeEntry.Bytes = buffer;
			}
			else
			{
				writeEntry.Bytes = reader.ReadBytes(writeEntry.Size);
			}
			return writeEntry;
		}

		private CopyEntry ReadCopyEntry()
		{
			CopyEntry result = default(CopyEntry);
			result.SrcPosition = reader.ReadInt32();
			result.Size = reader.ReadInt32();
			result.DestPosition = reader.ReadInt32();
			return result;
		}

		private void ApplyEntries(int curEntryIndex, int numEntries)
		{
			for (int i = curEntryIndex; i < numEntries; i++)
			{
				JournalEntryType journalEntryType = (JournalEntryType)reader.ReadByte();
				switch (journalEntryType)
				{
				case JournalEntryType.Resize:
				{
					string path3 = reader.ReadString();
					Stream targetFileStream3 = fileSystem.OpenFileStream(path3);
					ResizeEntry entry3 = ReadResizeEntry();
					ApplyResizeEntry(targetFileStream3, entry3);
					break;
				}
				case JournalEntryType.Write:
				{
					string path2 = reader.ReadString();
					Stream targetFileStream2 = fileSystem.OpenFileStream(path2);
					WriteEntry entry2 = ReadWriteEntry();
					ApplyWriteEntry(targetFileStream2, entry2);
					break;
				}
				case JournalEntryType.Copy:
				{
					string path = reader.ReadString();
					Stream targetFileStream = fileSystem.OpenFileStream(path);
					CopyEntry entry = ReadCopyEntry();
					ApplyCopyEntry(targetFileStream, entry);
					break;
				}
				default:
					throw new CorruptionException("Unhandled entry type: " + journalEntryType);
				}
				curEntryIndex++;
				SetCurEntryIndex(curEntryIndex);
			}
		}

		private static void ApplyResizeEntry(Stream targetFileStream, ResizeEntry entry)
		{
			targetFileStream.SetLength(entry.Length);
			targetFileStream.Flush();
		}

		private static void ApplyWriteEntry(Stream targetFileStream, WriteEntry entry)
		{
			targetFileStream.Seek(entry.FilePosition, SeekOrigin.Begin);
			targetFileStream.Write(entry.Bytes, 0, entry.Size);
			targetFileStream.Flush();
		}

		private static void ApplyCopyEntry(Stream targetFileStream, CopyEntry entry)
		{
			targetFileStream.Seek(entry.SrcPosition, SeekOrigin.Begin);
			int num = 0;
			int num2 = entry.Size;
			do
			{
				int num3 = targetFileStream.Read(buffer, num, num2);
				num2 -= num3;
				num += num3;
			}
			while (num2 > 0);
			targetFileStream.Seek(entry.DestPosition, SeekOrigin.Begin);
			targetFileStream.Write(buffer, 0, entry.Size);
			targetFileStream.Flush();
		}

		private void SetCurEntryIndex(int curEntryIndex)
		{
			long position = journalStream.Position;
			journalStream.Seek(6L, SeekOrigin.Begin);
			writer.Write(curEntryIndex);
			writer.Flush();
			journalStream.Seek(position, SeekOrigin.Begin);
		}
	}
}
