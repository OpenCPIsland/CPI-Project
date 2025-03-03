using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Encryption;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipInputStream : InflaterInputStream
	{
		private delegate int ReadDataHandler(byte[] b, int offset, int length);

		private ReadDataHandler internalReader;

		private Crc32 crc = new Crc32();

		private ZipEntry entry;

		private long size;

		private int method;

		private int flags;

		private string password;

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public bool CanDecompressEntry
		{
			get
			{
				if (entry != null)
				{
					return entry.CanDecompress;
				}
				return false;
			}
		}

		public override int Available
		{
			get
			{
				if (entry == null)
				{
					return 0;
				}
				return 1;
			}
		}

		public override long Length
		{
			get
			{
				if (entry != null)
				{
					if (entry.Size >= 0)
					{
						return entry.Size;
					}
					throw new ZipException("Length not available for the current entry");
				}
				throw new InvalidOperationException("No current entry");
			}
		}

		public ZipInputStream(Stream baseInputStream)
			: base(baseInputStream, new Inflater(true))
		{
			internalReader = ReadingNotAvailable;
		}

		public ZipInputStream(Stream baseInputStream, int bufferSize)
			: base(baseInputStream, new Inflater(true), bufferSize)
		{
			internalReader = ReadingNotAvailable;
		}

		public ZipEntry GetNextEntry()
		{
			if (crc == null)
			{
				throw new InvalidOperationException("Closed.");
			}
			if (entry != null)
			{
				CloseEntry();
			}
			int num = inputBuffer.ReadLeInt();
			switch (num)
			{
			case 33639248:
			case 84233040:
			case 101010256:
			case 101075792:
			case 117853008:
				Close();
				return null;
			case 134695760:
			case 808471376:
				num = inputBuffer.ReadLeInt();
				break;
			}
			if (num != 67324752)
			{
				throw new ZipException("Wrong Local header signature: 0x" + string.Format("{0:X}", num));
			}
			short versionRequiredToExtract = (short)inputBuffer.ReadLeShort();
			flags = inputBuffer.ReadLeShort();
			method = inputBuffer.ReadLeShort();
			uint num2 = (uint)inputBuffer.ReadLeInt();
			int num3 = inputBuffer.ReadLeInt();
			csize = inputBuffer.ReadLeInt();
			size = inputBuffer.ReadLeInt();
			int num4 = inputBuffer.ReadLeShort();
			int num5 = inputBuffer.ReadLeShort();
			bool flag = (flags & 1) == 1;
			byte[] array = new byte[num4];
			inputBuffer.ReadRawBuffer(array);
			string name = ZipConstants.ConvertToStringExt(flags, array);
			entry = new ZipEntry(name, versionRequiredToExtract);
			entry.Flags = flags;
			entry.CompressionMethod = (CompressionMethod)method;
			if ((flags & 8) == 0)
			{
				entry.Crc = (num3 & uint.MaxValue);
				entry.Size = (size & uint.MaxValue);
				entry.CompressedSize = (csize & uint.MaxValue);
				entry.CryptoCheckValue = (byte)((num3 >> 24) & 0xFF);
			}
			else
			{
				if (num3 != 0)
				{
					entry.Crc = (num3 & uint.MaxValue);
				}
				if (size != 0)
				{
					entry.Size = (size & uint.MaxValue);
				}
				if (csize != 0)
				{
					entry.CompressedSize = (csize & uint.MaxValue);
				}
				entry.CryptoCheckValue = (byte)((num2 >> 8) & 0xFF);
			}
			entry.DosTime = num2;
			if (num5 > 0)
			{
				byte[] array2 = new byte[num5];
				inputBuffer.ReadRawBuffer(array2);
				entry.ExtraData = array2;
			}
			entry.ProcessExtraData(true);
			if (entry.CompressedSize >= 0)
			{
				csize = entry.CompressedSize;
			}
			if (entry.Size >= 0)
			{
				size = entry.Size;
			}
			if (method == 0 && ((!flag && csize != size) || (flag && csize - 12 != size)))
			{
				throw new ZipException("Stored, but compressed != uncompressed");
			}
			if (entry.IsCompressionMethodSupported())
			{
				internalReader = InitialRead;
			}
			else
			{
				internalReader = ReadingNotSupported;
			}
			return entry;
		}

		private void ReadDataDescriptor()
		{
			if (inputBuffer.ReadLeInt() != 134695760)
			{
				throw new ZipException("Data descriptor signature not found");
			}
			entry.Crc = (inputBuffer.ReadLeInt() & uint.MaxValue);
			if (entry.LocalHeaderRequiresZip64)
			{
				csize = inputBuffer.ReadLeLong();
				size = inputBuffer.ReadLeLong();
			}
			else
			{
				csize = inputBuffer.ReadLeInt();
				size = inputBuffer.ReadLeInt();
			}
			entry.CompressedSize = csize;
			entry.Size = size;
		}

		private void CompleteCloseEntry(bool testCrc)
		{
			StopDecrypting();
			if ((flags & 8) != 0)
			{
				ReadDataDescriptor();
			}
			size = 0L;
			if (testCrc && (crc.Value & uint.MaxValue) != entry.Crc && entry.Crc != -1)
			{
				throw new ZipException("CRC mismatch");
			}
			crc.Reset();
			if (method == 8)
			{
				inf.Reset();
			}
			entry = null;
		}

		public void CloseEntry()
		{
			if (crc == null)
			{
				throw new InvalidOperationException("Closed");
			}
			if (entry == null)
			{
				return;
			}
			if (method == 8)
			{
				if ((flags & 8) != 0)
				{
					byte[] array = new byte[4096];
					while (Read(array, 0, array.Length) > 0)
					{
					}
					return;
				}
				csize -= inf.TotalIn;
				inputBuffer.Available += inf.RemainingInput;
			}
			if (inputBuffer.Available > csize && csize >= 0)
			{
				inputBuffer.Available = (int)(inputBuffer.Available - csize);
			}
			else
			{
				csize -= inputBuffer.Available;
				inputBuffer.Available = 0;
				while (csize != 0)
				{
					long num = Skip(csize);
					if (num <= 0)
					{
						throw new ZipException("Zip archive ends early.");
					}
					csize -= num;
				}
			}
			CompleteCloseEntry(false);
		}

		public override int ReadByte()
		{
			byte[] array = new byte[1];
			if (Read(array, 0, 1) <= 0)
			{
				return -1;
			}
			return array[0] & 0xFF;
		}

		private int ReadingNotAvailable(byte[] destination, int offset, int count)
		{
			throw new InvalidOperationException("Unable to read from this stream");
		}

		private int ReadingNotSupported(byte[] destination, int offset, int count)
		{
			throw new ZipException("The compression method for this entry is not supported");
		}

		private int InitialRead(byte[] destination, int offset, int count)
		{
			if (!CanDecompressEntry)
			{
				throw new ZipException("Library cannot extract this entry. Version required is (" + entry.Version + ")");
			}
			if (entry.IsCrypted)
			{
				if (password == null)
				{
					throw new ZipException("No password set.");
				}
				PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
				byte[] rgbKey = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(password));
				inputBuffer.CryptoTransform = pkzipClassicManaged.CreateDecryptor(rgbKey, null);
				byte[] array = new byte[12];
				inputBuffer.ReadClearTextBuffer(array, 0, 12);
				if (array[11] != entry.CryptoCheckValue)
				{
					throw new ZipException("Invalid password");
				}
				if (csize >= 12)
				{
					csize -= 12L;
				}
				else if ((entry.Flags & 8) == 0)
				{
					throw new ZipException(string.Format("Entry compressed size {0} too small for encryption", csize));
				}
			}
			else
			{
				inputBuffer.CryptoTransform = null;
			}
			if (csize > 0 || (flags & 8) != 0)
			{
				if (method == 8 && inputBuffer.Available > 0)
				{
					inputBuffer.SetInflaterInput(inf);
				}
				internalReader = BodyRead;
				return BodyRead(destination, offset, count);
			}
			internalReader = ReadingNotAvailable;
			return 0;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot be negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Invalid offset/count combination");
			}
			return internalReader(buffer, offset, count);
		}

		private int BodyRead(byte[] buffer, int offset, int count)
		{
			if (crc == null)
			{
				throw new InvalidOperationException("Closed");
			}
			if (entry == null || count <= 0)
			{
				return 0;
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentException("Offset + count exceeds buffer size");
			}
			bool flag = false;
			switch (method)
			{
			case 8:
				count = base.Read(buffer, offset, count);
				if (count <= 0)
				{
					if (!inf.IsFinished)
					{
						throw new ZipException("Inflater not finished!");
					}
					inputBuffer.Available = inf.RemainingInput;
					if ((flags & 8) == 0 && ((inf.TotalIn != csize && csize != uint.MaxValue && csize != -1) || inf.TotalOut != size))
					{
						throw new ZipException("Size mismatch: " + csize + ";" + size + " <-> " + inf.TotalIn + ";" + inf.TotalOut);
					}
					inf.Reset();
					flag = true;
				}
				break;
			case 0:
				if (count > csize && csize >= 0)
				{
					count = (int)csize;
				}
				if (count > 0)
				{
					count = inputBuffer.ReadClearTextBuffer(buffer, offset, count);
					if (count > 0)
					{
						csize -= count;
						size -= count;
					}
				}
				if (csize == 0)
				{
					flag = true;
				}
				else if (count < 0)
				{
					throw new ZipException("EOF in stored block");
				}
				break;
			}
			if (count > 0)
			{
				crc.Update(buffer, offset, count);
			}
			if (flag)
			{
				CompleteCloseEntry(true);
			}
			return count;
		}

		public override void Close()
		{
			internalReader = ReadingNotAvailable;
			crc = null;
			entry = null;
			base.Close();
		}
	}
}
