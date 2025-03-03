using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Encryption;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipFile : IEnumerable, IDisposable
	{
		public delegate void KeysRequiredEventHandler(object sender, KeysRequiredEventArgs e);

		[Flags]
		private enum HeaderTest
		{
			Extract = 0x1,
			Header = 0x2
		}

		private enum UpdateCommand
		{
			Copy,
			Modify,
			Add
		}

		private class UpdateComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				ZipUpdate zipUpdate = x as ZipUpdate;
				ZipUpdate zipUpdate2 = y as ZipUpdate;
				int num;
				if (zipUpdate == null)
				{
					num = ((zipUpdate2 != null) ? (-1) : 0);
				}
				else if (zipUpdate2 == null)
				{
					num = 1;
				}
				else
				{
					int num2 = (zipUpdate.Command != 0 && zipUpdate.Command != UpdateCommand.Modify) ? 1 : 0;
					int num3 = (zipUpdate2.Command != 0 && zipUpdate2.Command != UpdateCommand.Modify) ? 1 : 0;
					num = num2 - num3;
					if (num == 0)
					{
						long num4 = zipUpdate.Entry.Offset - zipUpdate2.Entry.Offset;
						num = ((num4 < 0) ? (-1) : ((num4 != 0) ? 1 : 0));
					}
				}
				return num;
			}
		}

		private class ZipUpdate
		{
			private ZipEntry entry_;

			private ZipEntry outEntry_;

			private UpdateCommand command_;

			private IStaticDataSource dataSource_;

			private string filename_;

			private long sizePatchOffset_ = -1L;

			private long crcPatchOffset_ = -1L;

			private long _offsetBasedSize = -1L;

			public ZipEntry Entry
			{
				get
				{
					return entry_;
				}
			}

			public ZipEntry OutEntry
			{
				get
				{
					if (outEntry_ == null)
					{
						outEntry_ = (ZipEntry)entry_.Clone();
					}
					return outEntry_;
				}
			}

			public UpdateCommand Command
			{
				get
				{
					return command_;
				}
			}

			public string Filename
			{
				get
				{
					return filename_;
				}
			}

			public long SizePatchOffset
			{
				get
				{
					return sizePatchOffset_;
				}
				set
				{
					sizePatchOffset_ = value;
				}
			}

			public long CrcPatchOffset
			{
				get
				{
					return crcPatchOffset_;
				}
				set
				{
					crcPatchOffset_ = value;
				}
			}

			public long OffsetBasedSize
			{
				get
				{
					return _offsetBasedSize;
				}
				set
				{
					_offsetBasedSize = value;
				}
			}

			public ZipUpdate(string fileName, ZipEntry entry)
			{
				command_ = UpdateCommand.Add;
				entry_ = entry;
				filename_ = fileName;
			}

			[Obsolete]
			public ZipUpdate(string fileName, string entryName, CompressionMethod compressionMethod)
			{
				command_ = UpdateCommand.Add;
				entry_ = new ZipEntry(entryName);
				entry_.CompressionMethod = compressionMethod;
				filename_ = fileName;
			}

			[Obsolete]
			public ZipUpdate(string fileName, string entryName)
				: this(fileName, entryName, CompressionMethod.Deflated)
			{
			}

			[Obsolete]
			public ZipUpdate(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
			{
				command_ = UpdateCommand.Add;
				entry_ = new ZipEntry(entryName);
				entry_.CompressionMethod = compressionMethod;
				dataSource_ = dataSource;
			}

			public ZipUpdate(IStaticDataSource dataSource, ZipEntry entry)
			{
				command_ = UpdateCommand.Add;
				entry_ = entry;
				dataSource_ = dataSource;
			}

			public ZipUpdate(ZipEntry original, ZipEntry updated)
			{
				throw new ZipException("Modify not currently supported");
			}

			public ZipUpdate(UpdateCommand command, ZipEntry entry)
			{
				command_ = command;
				entry_ = (ZipEntry)entry.Clone();
			}

			public ZipUpdate(ZipEntry entry)
				: this(UpdateCommand.Copy, entry)
			{
			}

			public Stream GetSource()
			{
				Stream result = null;
				if (dataSource_ != null)
				{
					result = dataSource_.GetSource();
				}
				return result;
			}
		}

		private class ZipString
		{
			private string comment_;

			private byte[] rawComment_;

			private bool isSourceString_;

			public bool IsSourceString
			{
				get
				{
					return isSourceString_;
				}
			}

			public int RawLength
			{
				get
				{
					MakeBytesAvailable();
					return rawComment_.Length;
				}
			}

			public byte[] RawComment
			{
				get
				{
					MakeBytesAvailable();
					return (byte[])rawComment_.Clone();
				}
			}

			public ZipString(string comment)
			{
				comment_ = comment;
				isSourceString_ = true;
			}

			public ZipString(byte[] rawString)
			{
				rawComment_ = rawString;
			}

			public void Reset()
			{
				if (isSourceString_)
				{
					rawComment_ = null;
				}
				else
				{
					comment_ = null;
				}
			}

			private void MakeTextAvailable()
			{
				if (comment_ == null)
				{
					comment_ = ZipConstants.ConvertToString(rawComment_);
				}
			}

			private void MakeBytesAvailable()
			{
				if (rawComment_ == null)
				{
					rawComment_ = ZipConstants.ConvertToArray(comment_);
				}
			}

			public static implicit operator string(ZipString zipString)
			{
				zipString.MakeTextAvailable();
				return zipString.comment_;
			}
		}

		private class ZipEntryEnumerator : IEnumerator
		{
			private ZipEntry[] array;

			private int index = -1;

			public object Current
			{
				get
				{
					return array[index];
				}
			}

			public ZipEntryEnumerator(ZipEntry[] entries)
			{
				array = entries;
			}

			public void Reset()
			{
				index = -1;
			}

			public bool MoveNext()
			{
				return ++index < array.Length;
			}
		}

		private class UncompressedStream : Stream
		{
			private Stream baseStream_;

			public override bool CanRead
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
					return baseStream_.CanWrite;
				}
			}

			public override bool CanSeek
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
					return 0L;
				}
			}

			public override long Position
			{
				get
				{
					return baseStream_.Position;
				}
				set
				{
				}
			}

			public UncompressedStream(Stream baseStream)
			{
				baseStream_ = baseStream;
			}

			public override void Close()
			{
			}

			public override void Flush()
			{
				baseStream_.Flush();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return 0;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return 0L;
			}

			public override void SetLength(long value)
			{
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				baseStream_.Write(buffer, offset, count);
			}
		}

		private class PartialInputStream : Stream
		{
			private ZipFile zipFile_;

			private Stream baseStream_;

			private long start_;

			private long length_;

			private long readPos_;

			private long end_;

			public override long Position
			{
				get
				{
					return readPos_ - start_;
				}
				set
				{
					long num = start_ + value;
					if (num < start_)
					{
						throw new ArgumentException("Negative position is invalid");
					}
					if (num >= end_)
					{
						throw new InvalidOperationException("Cannot seek past end");
					}
					readPos_ = num;
				}
			}

			public override long Length
			{
				get
				{
					return length_;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return true;
				}
			}

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanTimeout
			{
				get
				{
					return baseStream_.CanTimeout;
				}
			}

			public PartialInputStream(ZipFile zipFile, long start, long length)
			{
				start_ = start;
				length_ = length;
				zipFile_ = zipFile;
				baseStream_ = zipFile_.baseStream_;
				readPos_ = start;
				end_ = start + length;
			}

			public override int ReadByte()
			{
				if (readPos_ >= end_)
				{
					return -1;
				}
				lock (baseStream_)
				{
					baseStream_.Seek(readPos_++, SeekOrigin.Begin);
					return baseStream_.ReadByte();
				}
			}

			public override void Close()
			{
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				lock (baseStream_)
				{
					if (count > end_ - readPos_)
					{
						count = (int)(end_ - readPos_);
						if (count == 0)
						{
							return 0;
						}
					}
					baseStream_.Seek(readPos_, SeekOrigin.Begin);
					int num = baseStream_.Read(buffer, offset, count);
					if (num > 0)
					{
						readPos_ += num;
					}
					return num;
				}
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				long num = readPos_;
				switch (origin)
				{
				case SeekOrigin.Begin:
					num = start_ + offset;
					break;
				case SeekOrigin.Current:
					num = readPos_ + offset;
					break;
				case SeekOrigin.End:
					num = end_ + offset;
					break;
				}
				if (num < start_)
				{
					throw new ArgumentException("Negative position is invalid");
				}
				if (num >= end_)
				{
					throw new IOException("Cannot seek past end");
				}
				readPos_ = num;
				return readPos_;
			}

			public override void Flush()
			{
			}
		}

		private const int DefaultBufferSize = 4096;

		public KeysRequiredEventHandler KeysRequired;

		private bool isDisposed_;

		private string name_;

		private string comment_;

		private string rawPassword_;

		private Stream baseStream_;

		private bool isStreamOwner;

		private long offsetOfFirstEntry;

		private ZipEntry[] entries_;

		private byte[] key;

		private bool isNewArchive_;

		private UseZip64 useZip64_ = UseZip64.Dynamic;

		private ArrayList updates_;

		private long updateCount_;

		private Hashtable updateIndex_;

		private IArchiveStorage archiveStorage_;

		private IDynamicDataSource updateDataSource_;

		private bool contentsEdited_;

		private int bufferSize_ = 4096;

		private byte[] copyBuffer_;

		private ZipString newComment_;

		private bool commentEdited_;

		private IEntryFactory updateEntryFactory_ = new ZipEntryFactory();

		private byte[] Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public string Password
		{
			set
			{
				if (value == null || value.Length == 0)
				{
					key = null;
					return;
				}
				rawPassword_ = value;
				key = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(value));
			}
		}

		private bool HaveKeys
		{
			get
			{
				return key != null;
			}
		}

		public bool IsStreamOwner
		{
			get
			{
				return isStreamOwner;
			}
			set
			{
				isStreamOwner = value;
			}
		}

		public bool IsEmbeddedArchive
		{
			get
			{
				return offsetOfFirstEntry > 0;
			}
		}

		public bool IsNewArchive
		{
			get
			{
				return isNewArchive_;
			}
		}

		public string ZipFileComment
		{
			get
			{
				return comment_;
			}
		}

		public string Name
		{
			get
			{
				return name_;
			}
		}

		[Obsolete("Use the Count property instead")]
		public int Size
		{
			get
			{
				return entries_.Length;
			}
		}

		public long Count
		{
			get
			{
				return entries_.Length;
			}
		}

		[IndexerName("EntryByIndex")]
		public ZipEntry this[int index]
		{
			get
			{
				return (ZipEntry)entries_[index].Clone();
			}
		}

		public INameTransform NameTransform
		{
			get
			{
				return updateEntryFactory_.NameTransform;
			}
			set
			{
				updateEntryFactory_.NameTransform = value;
			}
		}

		public IEntryFactory EntryFactory
		{
			get
			{
				return updateEntryFactory_;
			}
			set
			{
				if (value == null)
				{
					updateEntryFactory_ = new ZipEntryFactory();
				}
				else
				{
					updateEntryFactory_ = value;
				}
			}
		}

		public int BufferSize
		{
			get
			{
				return bufferSize_;
			}
			set
			{
				if (value < 1024)
				{
					throw new ArgumentOutOfRangeException("value", "cannot be below 1024");
				}
				if (bufferSize_ != value)
				{
					bufferSize_ = value;
					copyBuffer_ = null;
				}
			}
		}

		public bool IsUpdating
		{
			get
			{
				return updates_ != null;
			}
		}

		public UseZip64 UseZip64
		{
			get
			{
				return useZip64_;
			}
			set
			{
				useZip64_ = value;
			}
		}

		private void OnKeysRequired(string fileName)
		{
			if (KeysRequired != null)
			{
				KeysRequiredEventArgs keysRequiredEventArgs = new KeysRequiredEventArgs(fileName, key);
				KeysRequired(this, keysRequiredEventArgs);
				key = keysRequiredEventArgs.Key;
			}
		}

		public ZipFile(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			name_ = name;
			baseStream_ = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
			isStreamOwner = true;
			try
			{
				ReadEntries();
			}
			catch
			{
				DisposeInternal(true);
				throw;
			}
		}

		public ZipFile(FileStream file)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (!file.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "file");
			}
			baseStream_ = file;
			name_ = file.Name;
			isStreamOwner = true;
			try
			{
				ReadEntries();
			}
			catch
			{
				DisposeInternal(true);
				throw;
			}
		}

		public ZipFile(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "stream");
			}
			baseStream_ = stream;
			isStreamOwner = true;
			if (baseStream_.Length > 0)
			{
				try
				{
					ReadEntries();
				}
				catch
				{
					DisposeInternal(true);
					throw;
				}
				return;
			}
			entries_ = new ZipEntry[0];
			isNewArchive_ = true;
		}

		internal ZipFile()
		{
			entries_ = new ZipEntry[0];
			isNewArchive_ = true;
		}

		~ZipFile()
		{
			Dispose(false);
		}

		public void Close()
		{
			DisposeInternal(true);
			GC.SuppressFinalize(this);
		}

		public static ZipFile Create(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			FileStream fileStream = File.Create(fileName);
			ZipFile zipFile = new ZipFile();
			zipFile.name_ = fileName;
			zipFile.baseStream_ = fileStream;
			zipFile.isStreamOwner = true;
			return zipFile;
		}

		public static ZipFile Create(Stream outStream)
		{
			if (outStream == null)
			{
				throw new ArgumentNullException("outStream");
			}
			if (!outStream.CanWrite)
			{
				throw new ArgumentException("Stream is not writeable", "outStream");
			}
			if (!outStream.CanSeek)
			{
				throw new ArgumentException("Stream is not seekable", "outStream");
			}
			ZipFile zipFile = new ZipFile();
			zipFile.baseStream_ = outStream;
			return zipFile;
		}

		public IEnumerator GetEnumerator()
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			return new ZipEntryEnumerator(entries_);
		}

		public int FindEntry(string name, bool ignoreCase)
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			for (int i = 0; i < entries_.Length; i++)
			{
				if (string.Compare(name, entries_[i].Name, ignoreCase, CultureInfo.InvariantCulture) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public ZipEntry GetEntry(string name)
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			int num = FindEntry(name, true);
			if (num < 0)
			{
				return null;
			}
			return (ZipEntry)entries_[num].Clone();
		}

		public Stream GetInputStream(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			long num = entry.ZipFileIndex;
			if (num < 0 || num >= entries_.Length || entries_[num].Name != entry.Name)
			{
				num = FindEntry(entry.Name, true);
				if (num < 0)
				{
					throw new ZipException("Entry cannot be found");
				}
			}
			return GetInputStream(num);
		}

		public Stream GetInputStream(long entryIndex)
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			long start = LocateEntry(entries_[entryIndex]);
			CompressionMethod compressionMethod = entries_[entryIndex].CompressionMethod;
			Stream stream = new PartialInputStream(this, start, entries_[entryIndex].CompressedSize);
			if (entries_[entryIndex].IsCrypted)
			{
				stream = CreateAndInitDecryptionStream(stream, entries_[entryIndex]);
				if (stream == null)
				{
					throw new ZipException("Unable to decrypt this entry");
				}
			}
			switch (compressionMethod)
			{
			case CompressionMethod.Deflated:
				stream = new InflaterInputStream(stream, new Inflater(true));
				break;
			default:
				throw new ZipException("Unsupported compression method " + compressionMethod);
			case CompressionMethod.Stored:
				break;
			}
			return stream;
		}

		public bool TestArchive(bool testData)
		{
			return TestArchive(testData, TestStrategy.FindFirstError, null);
		}

		public bool TestArchive(bool testData, TestStrategy strategy, ZipTestResultHandler resultHandler)
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			TestStatus testStatus = new TestStatus(this);
			if (resultHandler != null)
			{
				resultHandler(testStatus, null);
			}
			HeaderTest tests = testData ? (HeaderTest.Extract | HeaderTest.Header) : HeaderTest.Header;
			bool flag = true;
			try
			{
				int num = 0;
				while (flag && num < Count)
				{
					if (resultHandler != null)
					{
						testStatus.SetEntry(this[num]);
						testStatus.SetOperation(TestOperation.EntryHeader);
						resultHandler(testStatus, null);
					}
					try
					{
						TestLocalHeader(this[num], tests);
					}
					catch (ZipException ex)
					{
						testStatus.AddError();
						if (resultHandler != null)
						{
							resultHandler(testStatus, string.Format("Exception during test - '{0}'", ex.Message));
						}
						if (strategy == TestStrategy.FindFirstError)
						{
							flag = false;
						}
					}
					if (flag && testData && this[num].IsFile)
					{
						if (resultHandler != null)
						{
							testStatus.SetOperation(TestOperation.EntryData);
							resultHandler(testStatus, null);
						}
						Crc32 crc = new Crc32();
						using (Stream stream = GetInputStream(this[num]))
						{
							byte[] array = new byte[4096];
							long num2 = 0L;
							int num3;
							while ((num3 = stream.Read(array, 0, array.Length)) > 0)
							{
								crc.Update(array, 0, num3);
								if (resultHandler != null)
								{
									num2 += num3;
									testStatus.SetBytesTested(num2);
									resultHandler(testStatus, null);
								}
							}
						}
						if (this[num].Crc != crc.Value)
						{
							testStatus.AddError();
							if (resultHandler != null)
							{
								resultHandler(testStatus, "CRC mismatch");
							}
							if (strategy == TestStrategy.FindFirstError)
							{
								flag = false;
							}
						}
						if ((this[num].Flags & 8) != 0)
						{
							ZipHelperStream zipHelperStream = new ZipHelperStream(baseStream_);
							DescriptorData descriptorData = new DescriptorData();
							zipHelperStream.ReadDataDescriptor(this[num].LocalHeaderRequiresZip64, descriptorData);
							if (this[num].Crc != descriptorData.Crc)
							{
								testStatus.AddError();
							}
							if (this[num].CompressedSize != descriptorData.CompressedSize)
							{
								testStatus.AddError();
							}
							if (this[num].Size != descriptorData.Size)
							{
								testStatus.AddError();
							}
						}
					}
					if (resultHandler != null)
					{
						testStatus.SetOperation(TestOperation.EntryComplete);
						resultHandler(testStatus, null);
					}
					num++;
				}
				if (resultHandler != null)
				{
					testStatus.SetOperation(TestOperation.MiscellaneousTests);
					resultHandler(testStatus, null);
				}
			}
			catch (Exception ex2)
			{
				testStatus.AddError();
				if (resultHandler != null)
				{
					resultHandler(testStatus, string.Format("Exception during test - '{0}'", ex2.Message));
				}
			}
			if (resultHandler != null)
			{
				testStatus.SetOperation(TestOperation.Complete);
				testStatus.SetEntry(null);
				resultHandler(testStatus, null);
			}
			return testStatus.ErrorCount == 0;
		}

		private long TestLocalHeader(ZipEntry entry, HeaderTest tests)
		{
			lock (baseStream_)
			{
				bool flag = (tests & HeaderTest.Header) != 0;
				bool flag2 = (tests & HeaderTest.Extract) != 0;
				baseStream_.Seek(offsetOfFirstEntry + entry.Offset, SeekOrigin.Begin);
				if (ReadLEUint() != 67324752)
				{
					throw new ZipException(string.Format("Wrong local header signature @{0:X}", offsetOfFirstEntry + entry.Offset));
				}
				short num = (short)ReadLEUshort();
				short num2 = (short)ReadLEUshort();
				short num3 = (short)ReadLEUshort();
				short num4 = (short)ReadLEUshort();
				short num5 = (short)ReadLEUshort();
				uint num6 = ReadLEUint();
				long num7 = ReadLEUint();
				long num8 = ReadLEUint();
				int num9 = ReadLEUshort();
				int num10 = ReadLEUshort();
				byte[] array = new byte[num9];
				StreamUtils.ReadFully(baseStream_, array);
				byte[] array2 = new byte[num10];
				StreamUtils.ReadFully(baseStream_, array2);
				ZipExtraData zipExtraData = new ZipExtraData(array2);
				if (zipExtraData.Find(1))
				{
					num8 = zipExtraData.ReadLong();
					num7 = zipExtraData.ReadLong();
					if ((num2 & 8) != 0)
					{
						if (num8 != -1 && num8 != entry.Size)
						{
							throw new ZipException("Size invalid for descriptor");
						}
						if (num7 != -1 && num7 != entry.CompressedSize)
						{
							throw new ZipException("Compressed size invalid for descriptor");
						}
					}
				}
				else if (num >= 45 && ((int)num8 == -1 || (int)num7 == -1))
				{
					throw new ZipException("Required Zip64 extended information missing");
				}
				if (flag2 && entry.IsFile)
				{
					if (!entry.IsCompressionMethodSupported())
					{
						throw new ZipException("Compression method not supported");
					}
					if (num > 51 || (num > 20 && num < 45))
					{
						throw new ZipException(string.Format("Version required to extract this entry not supported ({0})", num));
					}
					if ((num2 & 0x3060) != 0)
					{
						throw new ZipException("The library does not support the zip version required to extract this entry");
					}
				}
				if (flag)
				{
					if (num <= 63 && num != 10 && num != 11 && num != 20 && num != 21 && num != 25 && num != 27 && num != 45 && num != 46 && num != 50 && num != 51 && num != 52 && num != 61 && num != 62 && num != 63)
					{
						throw new ZipException(string.Format("Version required to extract this entry is invalid ({0})", num));
					}
					if ((num2 & 0xC010) != 0)
					{
						throw new ZipException("Reserved bit flags cannot be set.");
					}
					if ((num2 & 1) != 0 && num < 20)
					{
						throw new ZipException(string.Format("Version required to extract this entry is too low for encryption ({0})", num));
					}
					if ((num2 & 0x40) != 0)
					{
						if ((num2 & 1) == 0)
						{
							throw new ZipException("Strong encryption flag set but encryption flag is not set");
						}
						if (num < 50)
						{
							throw new ZipException(string.Format("Version required to extract this entry is too low for encryption ({0})", num));
						}
					}
					if ((num2 & 0x20) != 0 && num < 27)
					{
						throw new ZipException(string.Format("Patched data requires higher version than ({0})", num));
					}
					if (num2 != entry.Flags)
					{
						throw new ZipException("Central header/local header flags mismatch");
					}
					if (entry.CompressionMethod != (CompressionMethod)num3)
					{
						throw new ZipException("Central header/local header compression method mismatch");
					}
					if (entry.Version != num)
					{
						throw new ZipException("Extract version mismatch");
					}
					if ((num2 & 0x40) != 0 && num < 62)
					{
						throw new ZipException("Strong encryption flag set but version not high enough");
					}
					if ((num2 & 0x2000) != 0 && (num4 != 0 || num5 != 0))
					{
						throw new ZipException("Header masked set but date/time values non-zero");
					}
					if ((num2 & 8) == 0 && num6 != (uint)entry.Crc)
					{
						throw new ZipException("Central header/local header crc mismatch");
					}
					if (num8 == 0 && num7 == 0 && num6 != 0)
					{
						throw new ZipException("Invalid CRC for empty entry");
					}
					if (entry.Name.Length > num9)
					{
						throw new ZipException("File name length mismatch");
					}
					string text = ZipConstants.ConvertToStringExt(num2, array);
					if (text != entry.Name)
					{
						throw new ZipException("Central header and local header file name mismatch");
					}
					if (entry.IsDirectory)
					{
						if (num8 > 0)
						{
							throw new ZipException("Directory cannot have size");
						}
						if (entry.IsCrypted)
						{
							if (num7 > 14)
							{
								throw new ZipException("Directory compressed size invalid");
							}
						}
						else if (num7 > 2)
						{
							throw new ZipException("Directory compressed size invalid");
						}
					}
					if (!ZipNameTransform.IsValidName(text, true))
					{
						throw new ZipException("Name is invalid");
					}
				}
				if ((num2 & 8) == 0 || num8 > 0 || num7 > 0)
				{
					if (num8 != entry.Size)
					{
						throw new ZipException(string.Format("Size mismatch between central header({0}) and local header({1})", entry.Size, num8));
					}
					if (num7 != entry.CompressedSize && num7 != uint.MaxValue && num7 != -1)
					{
						throw new ZipException(string.Format("Compressed size mismatch between central header({0}) and local header({1})", entry.CompressedSize, num7));
					}
				}
				int num11 = num9 + num10;
				return offsetOfFirstEntry + entry.Offset + 30 + num11;
			}
		}

		public void BeginUpdate(IArchiveStorage archiveStorage, IDynamicDataSource dataSource)
		{
			if (archiveStorage == null)
			{
				throw new ArgumentNullException("archiveStorage");
			}
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			if (IsEmbeddedArchive)
			{
				throw new ZipException("Cannot update embedded/SFX archives");
			}
			archiveStorage_ = archiveStorage;
			updateDataSource_ = dataSource;
			updateIndex_ = new Hashtable();
			updates_ = new ArrayList(entries_.Length);
			ZipEntry[] array = entries_;
			foreach (ZipEntry zipEntry in array)
			{
				int num = updates_.Add(new ZipUpdate(zipEntry));
				updateIndex_.Add(zipEntry.Name, num);
			}
			updates_.Sort(new UpdateComparer());
			int num2 = 0;
			foreach (ZipUpdate item in updates_)
			{
				if (num2 == updates_.Count - 1)
				{
					break;
				}
				item.OffsetBasedSize = ((ZipUpdate)updates_[num2 + 1]).Entry.Offset - item.Entry.Offset;
				num2++;
			}
			updateCount_ = updates_.Count;
			contentsEdited_ = false;
			commentEdited_ = false;
			newComment_ = null;
		}

		public void BeginUpdate(IArchiveStorage archiveStorage)
		{
			BeginUpdate(archiveStorage, new DynamicDiskDataSource());
		}

		public void BeginUpdate()
		{
			if (Name == null)
			{
				BeginUpdate(new MemoryArchiveStorage(), new DynamicDiskDataSource());
			}
			else
			{
				BeginUpdate(new DiskArchiveStorage(this), new DynamicDiskDataSource());
			}
		}

		public void CommitUpdate()
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			CheckUpdating();
			try
			{
				updateIndex_.Clear();
				updateIndex_ = null;
				if (contentsEdited_)
				{
					RunUpdates();
				}
				else if (commentEdited_)
				{
					UpdateCommentOnly();
				}
				else if (entries_.Length == 0)
				{
					byte[] comment = (newComment_ != null) ? newComment_.RawComment : ZipConstants.ConvertToArray(comment_);
					using (ZipHelperStream zipHelperStream = new ZipHelperStream(baseStream_))
					{
						zipHelperStream.WriteEndOfCentralDirectory(0L, 0L, 0L, comment);
					}
				}
			}
			finally
			{
				PostUpdateCleanup();
			}
		}

		public void AbortUpdate()
		{
			PostUpdateCleanup();
		}

		public void SetComment(string comment)
		{
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			CheckUpdating();
			newComment_ = new ZipString(comment);
			if (newComment_.RawLength > 65535)
			{
				newComment_ = null;
				throw new ZipException("Comment length exceeds maximum - 65535");
			}
			commentEdited_ = true;
		}

		private void AddUpdate(ZipUpdate update)
		{
			contentsEdited_ = true;
			int num = FindExistingUpdate(update.Entry.Name);
			if (num >= 0)
			{
				if (updates_[num] == null)
				{
					updateCount_++;
				}
				updates_[num] = update;
			}
			else
			{
				num = updates_.Add(update);
				updateCount_++;
				updateIndex_.Add(update.Entry.Name, num);
			}
		}

		public void Add(string fileName, CompressionMethod compressionMethod, bool useUnicodeText)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (isDisposed_)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			if (!ZipEntry.IsCompressionMethodSupported(compressionMethod))
			{
				throw new ArgumentOutOfRangeException("compressionMethod");
			}
			CheckUpdating();
			contentsEdited_ = true;
			ZipEntry zipEntry = EntryFactory.MakeFileEntry(fileName);
			zipEntry.IsUnicodeText = useUnicodeText;
			zipEntry.CompressionMethod = compressionMethod;
			AddUpdate(new ZipUpdate(fileName, zipEntry));
		}

		public void Add(string fileName, CompressionMethod compressionMethod)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (!ZipEntry.IsCompressionMethodSupported(compressionMethod))
			{
				throw new ArgumentOutOfRangeException("compressionMethod");
			}
			CheckUpdating();
			contentsEdited_ = true;
			ZipEntry zipEntry = EntryFactory.MakeFileEntry(fileName);
			zipEntry.CompressionMethod = compressionMethod;
			AddUpdate(new ZipUpdate(fileName, zipEntry));
		}

		public void Add(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			CheckUpdating();
			AddUpdate(new ZipUpdate(fileName, EntryFactory.MakeFileEntry(fileName)));
		}

		public void Add(string fileName, string entryName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			CheckUpdating();
			AddUpdate(new ZipUpdate(fileName, EntryFactory.MakeFileEntry(entryName)));
		}

		public void Add(IStaticDataSource dataSource, string entryName)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			CheckUpdating();
			AddUpdate(new ZipUpdate(dataSource, EntryFactory.MakeFileEntry(entryName, false)));
		}

		public void Add(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			CheckUpdating();
			ZipEntry zipEntry = EntryFactory.MakeFileEntry(entryName, false);
			zipEntry.CompressionMethod = compressionMethod;
			AddUpdate(new ZipUpdate(dataSource, zipEntry));
		}

		public void Add(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod, bool useUnicodeText)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException("dataSource");
			}
			if (entryName == null)
			{
				throw new ArgumentNullException("entryName");
			}
			CheckUpdating();
			ZipEntry zipEntry = EntryFactory.MakeFileEntry(entryName, false);
			zipEntry.IsUnicodeText = useUnicodeText;
			zipEntry.CompressionMethod = compressionMethod;
			AddUpdate(new ZipUpdate(dataSource, zipEntry));
		}

		public void Add(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			CheckUpdating();
			if (entry.Size != 0 || entry.CompressedSize != 0)
			{
				throw new ZipException("Entry cannot have any data");
			}
			AddUpdate(new ZipUpdate(UpdateCommand.Add, entry));
		}

		public void AddDirectory(string directoryName)
		{
			if (directoryName == null)
			{
				throw new ArgumentNullException("directoryName");
			}
			CheckUpdating();
			ZipEntry entry = EntryFactory.MakeDirectoryEntry(directoryName);
			AddUpdate(new ZipUpdate(UpdateCommand.Add, entry));
		}

		public bool Delete(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			CheckUpdating();
			bool flag = false;
			int num = FindExistingUpdate(fileName);
			if (num >= 0 && updates_[num] != null)
			{
				flag = true;
				contentsEdited_ = true;
				updates_[num] = null;
				updateCount_--;
				return flag;
			}
			throw new ZipException("Cannot find entry to delete");
		}

		public void Delete(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			CheckUpdating();
			int num = FindExistingUpdate(entry);
			if (num >= 0)
			{
				contentsEdited_ = true;
				updates_[num] = null;
				updateCount_--;
				return;
			}
			throw new ZipException("Cannot find entry to delete");
		}

		private void WriteLEShort(int value)
		{
			baseStream_.WriteByte((byte)(value & 0xFF));
			baseStream_.WriteByte((byte)((value >> 8) & 0xFF));
		}

		private void WriteLEUshort(ushort value)
		{
			baseStream_.WriteByte((byte)(value & 0xFF));
			baseStream_.WriteByte((byte)(value >> 8));
		}

		private void WriteLEInt(int value)
		{
			WriteLEShort(value & 0xFFFF);
			WriteLEShort(value >> 16);
		}

		private void WriteLEUint(uint value)
		{
			WriteLEUshort((ushort)(value & 0xFFFF));
			WriteLEUshort((ushort)(value >> 16));
		}

		private void WriteLeLong(long value)
		{
			WriteLEInt((int)(value & uint.MaxValue));
			WriteLEInt((int)(value >> 32));
		}

		private void WriteLEUlong(ulong value)
		{
			WriteLEUint((uint)(value & uint.MaxValue));
			WriteLEUint((uint)(value >> 32));
		}

		private void WriteLocalEntryHeader(ZipUpdate update)
		{
			ZipEntry outEntry = update.OutEntry;
			outEntry.Offset = baseStream_.Position;
			if (update.Command != 0)
			{
				if (outEntry.CompressionMethod == CompressionMethod.Deflated)
				{
					if (outEntry.Size == 0)
					{
						outEntry.CompressedSize = outEntry.Size;
						outEntry.Crc = 0L;
						outEntry.CompressionMethod = CompressionMethod.Stored;
					}
				}
				else if (outEntry.CompressionMethod == CompressionMethod.Stored)
				{
					outEntry.Flags &= -9;
				}
				if (HaveKeys)
				{
					outEntry.IsCrypted = true;
					if (outEntry.Crc < 0)
					{
						outEntry.Flags |= 8;
					}
				}
				else
				{
					outEntry.IsCrypted = false;
				}
				switch (useZip64_)
				{
				case UseZip64.Dynamic:
					if (outEntry.Size < 0)
					{
						outEntry.ForceZip64();
					}
					break;
				case UseZip64.On:
					outEntry.ForceZip64();
					break;
				}
			}
			WriteLEInt(67324752);
			WriteLEShort(outEntry.Version);
			WriteLEShort(outEntry.Flags);
			WriteLEShort((byte)outEntry.CompressionMethod);
			WriteLEInt((int)outEntry.DosTime);
			if (!outEntry.HasCrc)
			{
				update.CrcPatchOffset = baseStream_.Position;
				WriteLEInt(0);
			}
			else
			{
				WriteLEInt((int)outEntry.Crc);
			}
			if (outEntry.LocalHeaderRequiresZip64)
			{
				WriteLEInt(-1);
				WriteLEInt(-1);
			}
			else
			{
				if (outEntry.CompressedSize < 0 || outEntry.Size < 0)
				{
					update.SizePatchOffset = baseStream_.Position;
				}
				WriteLEInt((int)outEntry.CompressedSize);
				WriteLEInt((int)outEntry.Size);
			}
			byte[] array = ZipConstants.ConvertToArray(outEntry.Flags, outEntry.Name);
			if (array.Length > 65535)
			{
				throw new ZipException("Entry name too long.");
			}
			ZipExtraData zipExtraData = new ZipExtraData(outEntry.ExtraData);
			if (outEntry.LocalHeaderRequiresZip64)
			{
				zipExtraData.StartNewEntry();
				zipExtraData.AddLeLong(outEntry.Size);
				zipExtraData.AddLeLong(outEntry.CompressedSize);
				zipExtraData.AddNewEntry(1);
			}
			else
			{
				zipExtraData.Delete(1);
			}
			outEntry.ExtraData = zipExtraData.GetEntryData();
			WriteLEShort(array.Length);
			WriteLEShort(outEntry.ExtraData.Length);
			if (array.Length > 0)
			{
				baseStream_.Write(array, 0, array.Length);
			}
			if (outEntry.LocalHeaderRequiresZip64)
			{
				if (!zipExtraData.Find(1))
				{
					throw new ZipException("Internal error cannot find extra data");
				}
				update.SizePatchOffset = baseStream_.Position + zipExtraData.CurrentReadIndex;
			}
			if (outEntry.ExtraData.Length > 0)
			{
				baseStream_.Write(outEntry.ExtraData, 0, outEntry.ExtraData.Length);
			}
		}

		private int WriteCentralDirectoryHeader(ZipEntry entry)
		{
			if (entry.CompressedSize < 0)
			{
				throw new ZipException("Attempt to write central directory entry with unknown csize");
			}
			if (entry.Size < 0)
			{
				throw new ZipException("Attempt to write central directory entry with unknown size");
			}
			if (entry.Crc < 0)
			{
				throw new ZipException("Attempt to write central directory entry with unknown crc");
			}
			WriteLEInt(33639248);
			WriteLEShort(51);
			WriteLEShort(entry.Version);
			WriteLEShort(entry.Flags);
			WriteLEShort((byte)entry.CompressionMethod);
			WriteLEInt((int)entry.DosTime);
			WriteLEInt((int)entry.Crc);
			if (entry.IsZip64Forced() || entry.CompressedSize >= uint.MaxValue)
			{
				WriteLEInt(-1);
			}
			else
			{
				WriteLEInt((int)(entry.CompressedSize & uint.MaxValue));
			}
			if (entry.IsZip64Forced() || entry.Size >= uint.MaxValue)
			{
				WriteLEInt(-1);
			}
			else
			{
				WriteLEInt((int)entry.Size);
			}
			byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
			if (array.Length > 65535)
			{
				throw new ZipException("Entry name is too long.");
			}
			WriteLEShort(array.Length);
			ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
			if (entry.CentralHeaderRequiresZip64)
			{
				zipExtraData.StartNewEntry();
				if (entry.Size >= uint.MaxValue || useZip64_ == UseZip64.On)
				{
					zipExtraData.AddLeLong(entry.Size);
				}
				if (entry.CompressedSize >= uint.MaxValue || useZip64_ == UseZip64.On)
				{
					zipExtraData.AddLeLong(entry.CompressedSize);
				}
				if (entry.Offset >= uint.MaxValue)
				{
					zipExtraData.AddLeLong(entry.Offset);
				}
				zipExtraData.AddNewEntry(1);
			}
			else
			{
				zipExtraData.Delete(1);
			}
			byte[] entryData = zipExtraData.GetEntryData();
			WriteLEShort(entryData.Length);
			WriteLEShort((entry.Comment != null) ? entry.Comment.Length : 0);
			WriteLEShort(0);
			WriteLEShort(0);
			if (entry.ExternalFileAttributes != -1)
			{
				WriteLEInt(entry.ExternalFileAttributes);
			}
			else if (entry.IsDirectory)
			{
				WriteLEUint(16u);
			}
			else
			{
				WriteLEUint(0u);
			}
			if (entry.Offset >= uint.MaxValue)
			{
				WriteLEUint(uint.MaxValue);
			}
			else
			{
				WriteLEUint((uint)entry.Offset);
			}
			if (array.Length > 0)
			{
				baseStream_.Write(array, 0, array.Length);
			}
			if (entryData.Length > 0)
			{
				baseStream_.Write(entryData, 0, entryData.Length);
			}
			byte[] array2 = (entry.Comment != null) ? Encoding.ASCII.GetBytes(entry.Comment) : new byte[0];
			if (array2.Length > 0)
			{
				baseStream_.Write(array2, 0, array2.Length);
			}
			return 46 + array.Length + entryData.Length + array2.Length;
		}

		private void PostUpdateCleanup()
		{
			updateDataSource_ = null;
			updates_ = null;
			updateIndex_ = null;
			if (archiveStorage_ != null)
			{
				archiveStorage_.Dispose();
				archiveStorage_ = null;
			}
		}

		private string GetTransformedFileName(string name)
		{
			INameTransform nameTransform = NameTransform;
			if (nameTransform == null)
			{
				return name;
			}
			return nameTransform.TransformFile(name);
		}

		private string GetTransformedDirectoryName(string name)
		{
			INameTransform nameTransform = NameTransform;
			if (nameTransform == null)
			{
				return name;
			}
			return nameTransform.TransformDirectory(name);
		}

		private byte[] GetBuffer()
		{
			if (copyBuffer_ == null)
			{
				copyBuffer_ = new byte[bufferSize_];
			}
			return copyBuffer_;
		}

		private void CopyDescriptorBytes(ZipUpdate update, Stream dest, Stream source)
		{
			int num = GetDescriptorSize(update);
			if (num <= 0)
			{
				return;
			}
			byte[] buffer = GetBuffer();
			while (true)
			{
				if (num > 0)
				{
					int count = Math.Min(buffer.Length, num);
					int num2 = source.Read(buffer, 0, count);
					if (num2 <= 0)
					{
						break;
					}
					dest.Write(buffer, 0, num2);
					num -= num2;
					continue;
				}
				return;
			}
			throw new ZipException("Unxpected end of stream");
		}

		private void CopyBytes(ZipUpdate update, Stream destination, Stream source, long bytesToCopy, bool updateCrc)
		{
			if (destination == source)
			{
				throw new InvalidOperationException("Destination and source are the same");
			}
			Crc32 crc = new Crc32();
			byte[] buffer = GetBuffer();
			long num = bytesToCopy;
			long num2 = 0L;
			int num4;
			do
			{
				int num3 = buffer.Length;
				if (bytesToCopy < num3)
				{
					num3 = (int)bytesToCopy;
				}
				num4 = source.Read(buffer, 0, num3);
				if (num4 > 0)
				{
					if (updateCrc)
					{
						crc.Update(buffer, 0, num4);
					}
					destination.Write(buffer, 0, num4);
					bytesToCopy -= num4;
					num2 += num4;
				}
			}
			while (num4 > 0 && bytesToCopy > 0);
			if (num2 != num)
			{
				throw new ZipException(string.Format("Failed to copy bytes expected {0} read {1}", num, num2));
			}
			if (updateCrc)
			{
				update.OutEntry.Crc = crc.Value;
			}
		}

		private int GetDescriptorSize(ZipUpdate update)
		{
			int result = 0;
			if ((update.Entry.Flags & 8) != 0)
			{
				result = 12;
				if (update.Entry.LocalHeaderRequiresZip64)
				{
					result = 20;
				}
			}
			return result;
		}

		private void CopyDescriptorBytesDirect(ZipUpdate update, Stream stream, ref long destinationPosition, long sourcePosition)
		{
			int num = GetDescriptorSize(update);
			while (true)
			{
				if (num > 0)
				{
					int count = num;
					byte[] buffer = GetBuffer();
					stream.Position = sourcePosition;
					int num2 = stream.Read(buffer, 0, count);
					if (num2 <= 0)
					{
						break;
					}
					stream.Position = destinationPosition;
					stream.Write(buffer, 0, num2);
					num -= num2;
					destinationPosition += num2;
					sourcePosition += num2;
					continue;
				}
				return;
			}
			throw new ZipException("Unxpected end of stream");
		}

		private void CopyEntryDataDirect(ZipUpdate update, Stream stream, bool updateCrc, ref long destinationPosition, ref long sourcePosition)
		{
			long num = update.Entry.CompressedSize;
			Crc32 crc = new Crc32();
			byte[] buffer = GetBuffer();
			long num2 = num;
			long num3 = 0L;
			int num5;
			do
			{
				int num4 = buffer.Length;
				if (num < num4)
				{
					num4 = (int)num;
				}
				stream.Position = sourcePosition;
				num5 = stream.Read(buffer, 0, num4);
				if (num5 > 0)
				{
					if (updateCrc)
					{
						crc.Update(buffer, 0, num5);
					}
					stream.Position = destinationPosition;
					stream.Write(buffer, 0, num5);
					destinationPosition += num5;
					sourcePosition += num5;
					num -= num5;
					num3 += num5;
				}
			}
			while (num5 > 0 && num > 0);
			if (num3 != num2)
			{
				throw new ZipException(string.Format("Failed to copy bytes expected {0} read {1}", num2, num3));
			}
			if (updateCrc)
			{
				update.OutEntry.Crc = crc.Value;
			}
		}

		private int FindExistingUpdate(ZipEntry entry)
		{
			int result = -1;
			string transformedFileName = GetTransformedFileName(entry.Name);
			if (updateIndex_.ContainsKey(transformedFileName))
			{
				result = (int)updateIndex_[transformedFileName];
			}
			return result;
		}

		private int FindExistingUpdate(string fileName)
		{
			int result = -1;
			string transformedFileName = GetTransformedFileName(fileName);
			if (updateIndex_.ContainsKey(transformedFileName))
			{
				result = (int)updateIndex_[transformedFileName];
			}
			return result;
		}

		private Stream GetOutputStream(ZipEntry entry)
		{
			Stream stream = baseStream_;
			if (entry.IsCrypted)
			{
				stream = CreateAndInitEncryptionStream(stream, entry);
			}
			switch (entry.CompressionMethod)
			{
			case CompressionMethod.Stored:
				return new UncompressedStream(stream);
			case CompressionMethod.Deflated:
			{
				DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(stream, new Deflater(9, true));
				deflaterOutputStream.IsStreamOwner = false;
				return deflaterOutputStream;
			}
			default:
				throw new ZipException("Unknown compression method " + entry.CompressionMethod);
			}
		}

		private void AddEntry(ZipFile workFile, ZipUpdate update)
		{
			Stream stream = null;
			if (update.Entry.IsFile)
			{
				stream = update.GetSource();
				if (stream == null)
				{
					stream = updateDataSource_.GetSource(update.Entry, update.Filename);
				}
			}
			if (stream != null)
			{
				using (stream)
				{
					long length = stream.Length;
					if (update.OutEntry.Size < 0)
					{
						update.OutEntry.Size = length;
					}
					else if (update.OutEntry.Size != length)
					{
						throw new ZipException("Entry size/stream size mismatch");
					}
					workFile.WriteLocalEntryHeader(update);
					long position = workFile.baseStream_.Position;
					using (Stream destination = workFile.GetOutputStream(update.OutEntry))
					{
						CopyBytes(update, destination, stream, length, true);
					}
					long position2 = workFile.baseStream_.Position;
					update.OutEntry.CompressedSize = position2 - position;
					if ((update.OutEntry.Flags & 8) == 8)
					{
						ZipHelperStream zipHelperStream = new ZipHelperStream(workFile.baseStream_);
						zipHelperStream.WriteDataDescriptor(update.OutEntry);
					}
				}
				return;
			}
			workFile.WriteLocalEntryHeader(update);
			update.OutEntry.CompressedSize = 0L;
		}

		private void ModifyEntry(ZipFile workFile, ZipUpdate update)
		{
			workFile.WriteLocalEntryHeader(update);
			long position = workFile.baseStream_.Position;
			if (update.Entry.IsFile && update.Filename != null)
			{
				using (Stream destination = workFile.GetOutputStream(update.OutEntry))
				{
					using (Stream stream = GetInputStream(update.Entry))
					{
						CopyBytes(update, destination, stream, stream.Length, true);
					}
				}
			}
			long position2 = workFile.baseStream_.Position;
			update.Entry.CompressedSize = position2 - position;
		}

		private void CopyEntryDirect(ZipFile workFile, ZipUpdate update, ref long destinationPosition)
		{
			bool flag = false;
			if (update.Entry.Offset == destinationPosition)
			{
				flag = true;
			}
			if (!flag)
			{
				baseStream_.Position = destinationPosition;
				workFile.WriteLocalEntryHeader(update);
				destinationPosition = baseStream_.Position;
			}
			long num = 0L;
			long num2 = update.Entry.Offset + 26;
			baseStream_.Seek(num2, SeekOrigin.Begin);
			uint num3 = ReadLEUshort();
			uint num4 = ReadLEUshort();
			num = baseStream_.Position + num3 + num4;
			if (flag)
			{
				if (update.OffsetBasedSize != -1)
				{
					destinationPosition += update.OffsetBasedSize;
				}
				else
				{
					destinationPosition += num - num2 + 26 + update.Entry.CompressedSize + GetDescriptorSize(update);
				}
				return;
			}
			if (update.Entry.CompressedSize > 0)
			{
				CopyEntryDataDirect(update, baseStream_, false, ref destinationPosition, ref num);
			}
			CopyDescriptorBytesDirect(update, baseStream_, ref destinationPosition, num);
		}

		private void CopyEntry(ZipFile workFile, ZipUpdate update)
		{
			workFile.WriteLocalEntryHeader(update);
			if (update.Entry.CompressedSize > 0)
			{
				long offset = update.Entry.Offset + 26;
				baseStream_.Seek(offset, SeekOrigin.Begin);
				uint num = ReadLEUshort();
				uint num2 = ReadLEUshort();
				baseStream_.Seek(num + num2, SeekOrigin.Current);
				CopyBytes(update, workFile.baseStream_, baseStream_, update.Entry.CompressedSize, false);
			}
			CopyDescriptorBytes(update, workFile.baseStream_, baseStream_);
		}

		private void Reopen(Stream source)
		{
			if (source == null)
			{
				throw new ZipException("Failed to reopen archive - no source");
			}
			isNewArchive_ = false;
			baseStream_ = source;
			ReadEntries();
		}

		private void Reopen()
		{
			if (Name == null)
			{
				throw new InvalidOperationException("Name is not known cannot Reopen");
			}
			Reopen(File.Open(Name, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		private void UpdateCommentOnly()
		{
			long length = baseStream_.Length;
			ZipHelperStream zipHelperStream = null;
			if (archiveStorage_.UpdateMode == FileUpdateMode.Safe)
			{
				Stream stream = archiveStorage_.MakeTemporaryCopy(baseStream_);
				zipHelperStream = new ZipHelperStream(stream);
				zipHelperStream.IsStreamOwner = true;
				baseStream_.Close();
				baseStream_ = null;
			}
			else if (archiveStorage_.UpdateMode == FileUpdateMode.Direct)
			{
				baseStream_ = archiveStorage_.OpenForDirectUpdate(baseStream_);
				zipHelperStream = new ZipHelperStream(baseStream_);
			}
			else
			{
				baseStream_.Close();
				baseStream_ = null;
				zipHelperStream = new ZipHelperStream(Name);
			}
			using (zipHelperStream)
			{
				long num = zipHelperStream.LocateBlockWithSignature(101010256, length, 22, 65535);
				if (num < 0)
				{
					throw new ZipException("Cannot find central directory");
				}
				zipHelperStream.Position += 16L;
				byte[] rawComment = newComment_.RawComment;
				zipHelperStream.WriteLEShort(rawComment.Length);
				zipHelperStream.Write(rawComment, 0, rawComment.Length);
				zipHelperStream.SetLength(zipHelperStream.Position);
			}
			if (archiveStorage_.UpdateMode == FileUpdateMode.Safe)
			{
				Reopen(archiveStorage_.ConvertTemporaryToFinal());
			}
			else
			{
				ReadEntries();
			}
		}

		private void RunUpdates()
		{
			long num = 0L;
			long num2 = 0L;
			bool flag = false;
			long destinationPosition = 0L;
			ZipFile zipFile;
			if (IsNewArchive)
			{
				zipFile = this;
				zipFile.baseStream_.Position = 0L;
				flag = true;
			}
			else if (archiveStorage_.UpdateMode == FileUpdateMode.Direct)
			{
				zipFile = this;
				zipFile.baseStream_.Position = 0L;
				flag = true;
				updates_.Sort(new UpdateComparer());
			}
			else
			{
				zipFile = Create(archiveStorage_.GetTemporaryOutput());
				zipFile.UseZip64 = UseZip64;
				if (key != null)
				{
					zipFile.key = (byte[])key.Clone();
				}
			}
			try
			{
				foreach (ZipUpdate item in updates_)
				{
					if (item != null)
					{
						switch (item.Command)
						{
						case UpdateCommand.Copy:
							if (flag)
							{
								CopyEntryDirect(zipFile, item, ref destinationPosition);
							}
							else
							{
								CopyEntry(zipFile, item);
							}
							break;
						case UpdateCommand.Modify:
							ModifyEntry(zipFile, item);
							break;
						case UpdateCommand.Add:
							if (!IsNewArchive && flag)
							{
								zipFile.baseStream_.Position = destinationPosition;
							}
							AddEntry(zipFile, item);
							if (flag)
							{
								destinationPosition = zipFile.baseStream_.Position;
							}
							break;
						}
					}
				}
				if (!IsNewArchive && flag)
				{
					zipFile.baseStream_.Position = destinationPosition;
				}
				long position = zipFile.baseStream_.Position;
				foreach (ZipUpdate item2 in updates_)
				{
					if (item2 != null)
					{
						num += zipFile.WriteCentralDirectoryHeader(item2.OutEntry);
					}
				}
				byte[] comment = (newComment_ != null) ? newComment_.RawComment : ZipConstants.ConvertToArray(comment_);
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(zipFile.baseStream_))
				{
					zipHelperStream.WriteEndOfCentralDirectory(updateCount_, num, position, comment);
				}
				num2 = zipFile.baseStream_.Position;
				foreach (ZipUpdate item3 in updates_)
				{
					if (item3 != null)
					{
						if (item3.CrcPatchOffset > 0 && item3.OutEntry.CompressedSize > 0)
						{
							zipFile.baseStream_.Position = item3.CrcPatchOffset;
							zipFile.WriteLEInt((int)item3.OutEntry.Crc);
						}
						if (item3.SizePatchOffset > 0)
						{
							zipFile.baseStream_.Position = item3.SizePatchOffset;
							if (item3.OutEntry.LocalHeaderRequiresZip64)
							{
								zipFile.WriteLeLong(item3.OutEntry.Size);
								zipFile.WriteLeLong(item3.OutEntry.CompressedSize);
							}
							else
							{
								zipFile.WriteLEInt((int)item3.OutEntry.CompressedSize);
								zipFile.WriteLEInt((int)item3.OutEntry.Size);
							}
						}
					}
				}
			}
			catch
			{
				zipFile.Close();
				if (!flag && zipFile.Name != null)
				{
					File.Delete(zipFile.Name);
				}
				throw;
			}
			if (flag)
			{
				zipFile.baseStream_.SetLength(num2);
				zipFile.baseStream_.Flush();
				isNewArchive_ = false;
				ReadEntries();
			}
			else
			{
				baseStream_.Close();
				Reopen(archiveStorage_.ConvertTemporaryToFinal());
			}
		}

		private void CheckUpdating()
		{
			if (updates_ == null)
			{
				throw new InvalidOperationException("BeginUpdate has not been called");
			}
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		private void DisposeInternal(bool disposing)
		{
			if (!isDisposed_)
			{
				isDisposed_ = true;
				entries_ = new ZipEntry[0];
				if (IsStreamOwner && baseStream_ != null)
				{
					lock (baseStream_)
					{
						baseStream_.Close();
					}
				}
				PostUpdateCleanup();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			DisposeInternal(disposing);
		}

		private ushort ReadLEUshort()
		{
			int num = baseStream_.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException("End of stream");
			}
			int num2 = baseStream_.ReadByte();
			if (num2 < 0)
			{
				throw new EndOfStreamException("End of stream");
			}
			return (ushort)((ushort)num | (ushort)(num2 << 8));
		}

		private uint ReadLEUint()
		{
			return (uint)(ReadLEUshort() | (ReadLEUshort() << 16));
		}

		private ulong ReadLEUlong()
		{
			return ReadLEUint() | ((ulong)ReadLEUint() << 32);
		}

		private long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
		{
			using (ZipHelperStream zipHelperStream = new ZipHelperStream(baseStream_))
			{
				return zipHelperStream.LocateBlockWithSignature(signature, endLocation, minimumBlockSize, maximumVariableData);
			}
		}

		private void ReadEntries()
		{
			if (!baseStream_.CanSeek)
			{
				throw new ZipException("ZipFile stream must be seekable");
			}
			long num = LocateBlockWithSignature(101010256, baseStream_.Length, 22, 65535);
			if (num < 0)
			{
				throw new ZipException("Cannot find central directory");
			}
			ushort num2 = ReadLEUshort();
			ushort num3 = ReadLEUshort();
			ulong num4 = ReadLEUshort();
			ulong num5 = ReadLEUshort();
			ulong num6 = ReadLEUint();
			long num7 = ReadLEUint();
			uint num8 = ReadLEUshort();
			if (num8 != 0)
			{
				byte[] array = new byte[num8];
				StreamUtils.ReadFully(baseStream_, array);
				comment_ = ZipConstants.ConvertToString(array);
			}
			else
			{
				comment_ = string.Empty;
			}
			bool flag = false;
			if (num2 == ushort.MaxValue || num3 == ushort.MaxValue || num4 == 65535 || num5 == 65535 || num6 == uint.MaxValue || num7 == uint.MaxValue)
			{
				flag = true;
				long num9 = LocateBlockWithSignature(117853008, num, 0, 4096);
				if (num9 < 0)
				{
					throw new ZipException("Cannot find Zip64 locator");
				}
				ReadLEUint();
				ulong num10 = ReadLEUlong();
				ReadLEUint();
				baseStream_.Position = (long)num10;
				long num11 = ReadLEUint();
				if (num11 != 101075792)
				{
					throw new ZipException(string.Format("Invalid Zip64 Central directory signature at {0:X}", num10));
				}
				ReadLEUlong();
				ReadLEUshort();
				ReadLEUshort();
				ReadLEUint();
				ReadLEUint();
				num4 = ReadLEUlong();
				num5 = ReadLEUlong();
				num6 = ReadLEUlong();
				num7 = (long)ReadLEUlong();
			}
			entries_ = new ZipEntry[num4];
			if (!flag && num7 < num - (long)(4 + num6))
			{
				offsetOfFirstEntry = num - ((long)(4 + num6) + num7);
				if (offsetOfFirstEntry <= 0)
				{
					throw new ZipException("Invalid embedded zip archive");
				}
			}
			baseStream_.Seek(offsetOfFirstEntry + num7, SeekOrigin.Begin);
			ulong num12 = 0uL;
			while (true)
			{
				if (num12 < num4)
				{
					if (ReadLEUint() != 33639248)
					{
						break;
					}
					int madeByInfo = ReadLEUshort();
					int versionRequiredToExtract = ReadLEUshort();
					int num13 = ReadLEUshort();
					int method = ReadLEUshort();
					uint num14 = ReadLEUint();
					uint num15 = ReadLEUint();
					long num16 = ReadLEUint();
					long num17 = ReadLEUint();
					int num18 = ReadLEUshort();
					int num19 = ReadLEUshort();
					int num20 = ReadLEUshort();
					ReadLEUshort();
					ReadLEUshort();
					uint externalFileAttributes = ReadLEUint();
					long offset = ReadLEUint();
					byte[] array2 = new byte[Math.Max(num18, num20)];
					StreamUtils.ReadFully(baseStream_, array2, 0, num18);
					string name = ZipConstants.ConvertToStringExt(num13, array2, num18);
					ZipEntry zipEntry = new ZipEntry(name, versionRequiredToExtract, madeByInfo, (CompressionMethod)method);
					zipEntry.Crc = ((long)num15 & 4294967295L);
					zipEntry.Size = (num17 & uint.MaxValue);
					zipEntry.CompressedSize = (num16 & uint.MaxValue);
					zipEntry.Flags = num13;
					zipEntry.DosTime = num14;
					zipEntry.ZipFileIndex = (long)num12;
					zipEntry.Offset = offset;
					zipEntry.ExternalFileAttributes = (int)externalFileAttributes;
					if ((num13 & 8) == 0)
					{
						zipEntry.CryptoCheckValue = (byte)(num15 >> 24);
					}
					else
					{
						zipEntry.CryptoCheckValue = (byte)((num14 >> 8) & 0xFF);
					}
					if (num19 > 0)
					{
						byte[] array3 = new byte[num19];
						StreamUtils.ReadFully(baseStream_, array3);
						zipEntry.ExtraData = array3;
					}
					zipEntry.ProcessExtraData(false);
					if (num20 > 0)
					{
						StreamUtils.ReadFully(baseStream_, array2, 0, num20);
						zipEntry.Comment = ZipConstants.ConvertToStringExt(num13, array2, num20);
					}
					entries_[num12] = zipEntry;
					num12++;
					continue;
				}
				return;
			}
			throw new ZipException("Wrong Central Directory signature");
		}

		private long LocateEntry(ZipEntry entry)
		{
			return TestLocalHeader(entry, HeaderTest.Extract);
		}

		private Stream CreateAndInitDecryptionStream(Stream baseStream, ZipEntry entry)
		{
			CryptoStream cryptoStream = null;
			if (entry.Version < 50 || (entry.Flags & 0x40) == 0)
			{
				PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
				OnKeysRequired(entry.Name);
				if (!HaveKeys)
				{
					throw new ZipException("No password available for encrypted stream");
				}
				cryptoStream = new CryptoStream(baseStream, pkzipClassicManaged.CreateDecryptor(key, null), CryptoStreamMode.Read);
				CheckClassicPassword(cryptoStream, entry);
				return cryptoStream;
			}
			throw new ZipException("Decryption method not supported");
		}

		private Stream CreateAndInitEncryptionStream(Stream baseStream, ZipEntry entry)
		{
			CryptoStream cryptoStream = null;
			if (entry.Version < 50 || (entry.Flags & 0x40) == 0)
			{
				PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
				OnKeysRequired(entry.Name);
				if (!HaveKeys)
				{
					throw new ZipException("No password available for encrypted stream");
				}
				cryptoStream = new CryptoStream(new UncompressedStream(baseStream), pkzipClassicManaged.CreateEncryptor(key, null), CryptoStreamMode.Write);
				if (entry.Crc < 0 || (entry.Flags & 8) != 0)
				{
					WriteEncryptionHeader(cryptoStream, entry.DosTime << 16);
				}
				else
				{
					WriteEncryptionHeader(cryptoStream, entry.Crc);
				}
			}
			return cryptoStream;
		}

		private static void CheckClassicPassword(CryptoStream classicCryptoStream, ZipEntry entry)
		{
			byte[] array = new byte[12];
			StreamUtils.ReadFully(classicCryptoStream, array);
			if (array[11] != entry.CryptoCheckValue)
			{
				throw new ZipException("Invalid password");
			}
		}

		private static void WriteEncryptionHeader(Stream stream, long crcValue)
		{
			byte[] array = new byte[12];
			Random random = new Random();
			random.NextBytes(array);
			array[11] = (byte)(crcValue >> 24);
			stream.Write(array, 0, array.Length);
		}
	}
}
