using System;
using System.Collections.Generic;
using System.IO;

namespace DeviceDB
{
	internal class PackedFile
	{
		private struct Page
		{
			public uint DocumentOffset;

			public ushort DocumentLength;

			public ushort ReservedDocumentLength;
		}

		private const byte FormatVersion = 0;

		private const uint FileHeaderSize = 1u;

		private const uint PageSize = 8u;

		private const ushort MaxDocumentSize = ushort.MaxValue;

		private readonly string documentsPath;

		private readonly string pagesPath;

		private readonly JournalWriter journalWriter;

		private readonly IFileSystem fileSystem;

		private readonly BinaryReader documentsReader;

		private readonly BinaryReader pagesReader;

		private readonly MemoryStream memStream;

		private readonly BinaryWriter memWriter;

		private readonly Stream documentsStream;

		private readonly Stream pagesStream;

		private bool isDisposed;

		public bool IsEmpty
		{
			get
			{
				uint numPages = GetNumPages();
				if (numPages == 0)
				{
					return true;
				}
				pagesStream.Position = 1L;
				for (uint num = 0u; num < numPages; num++)
				{
					Page page = ReadPage();
					if (page.DocumentLength > 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public uint TotalSize
		{
			get
			{
				return (uint)(pagesStream.Length + documentsStream.Length);
			}
		}

		public uint Count
		{
			get
			{
				uint numPages = GetNumPages();
				if (numPages == 0)
				{
					return 0u;
				}
				uint num = 0u;
				pagesStream.Position = 1L;
				for (uint num2 = 0u; num2 < numPages; num2++)
				{
					Page page = ReadPage();
					if (page.DocumentLength > 0)
					{
						num++;
					}
				}
				return num;
			}
		}

		public IEnumerable<uint> DocumentIds
		{
			get
			{
				uint numPages = GetNumPages();
				pagesStream.Position = 1L;
				uint i = 0u;
				uint offset = 1u;
				while (i < numPages)
				{
					Page page = ReadPage();
					if (page.DocumentLength > 0)
					{
						yield return offset;
					}
					i++;
					offset += 8;
				}
			}
		}

		public IEnumerable<KeyValuePair<uint, byte[]>> Documents
		{
			get
			{
				uint numPages = GetNumPages();
				documentsStream.Position = 1L;
				pagesStream.Position = 1L;
				uint i = 0u;
				uint offset = 1u;
				while (i < numPages)
				{
					Page page = ReadPage();
					if (page.DocumentLength > 0)
					{
						byte[] document = ReadDocument(page);
						long documentsStreamPos = documentsStream.Position;
						long pagesStreamPos = pagesStream.Position;
						yield return new KeyValuePair<uint, byte[]>(offset, document);
						if (documentsStream.Position != documentsStreamPos)
						{
							documentsStream.Position = documentsStreamPos;
						}
						if (pagesStream.Position != pagesStreamPos)
						{
							pagesStream.Position = pagesStreamPos;
						}
						long curPos = documentsStream.Position;
						uint endPos = page.DocumentOffset + page.ReservedDocumentLength;
						if (curPos != endPos)
						{
							documentsStream.Position = endPos;
						}
					}
					else
					{
						documentsStream.Position += (int)page.ReservedDocumentLength;
					}
					i++;
					offset += 8;
				}
			}
		}

		public PackedFile(string documentsPath, string metadataPath, JournalWriter journalWriter, IFileSystem fileSystem)
		{
			memStream = new MemoryStream();
			memWriter = new BinaryWriter(memStream);
			this.documentsPath = documentsPath;
			pagesPath = metadataPath;
			this.journalWriter = journalWriter;
			this.fileSystem = fileSystem;
			documentsStream = fileSystem.OpenFileStream(documentsPath);
			pagesStream = fileSystem.OpenFileStream(pagesPath);
			if (pagesStream.Length >= 1)
			{
				if (documentsStream.Length < 1)
				{
					throw new CorruptionException("Documents is empty but metadata isn't");
				}
				ReadFileHeader(pagesStream);
				ReadFileHeader(documentsStream);
			}
			else
			{
				documentsStream.WriteByte(0);
				pagesStream.WriteByte(0);
			}
			documentsReader = new BinaryReader(documentsStream);
			pagesReader = new BinaryReader(pagesStream);
		}

		public uint Insert(byte[] document)
		{
			return Insert(document, uint.MaxValue);
		}

		public uint Update(uint documentId, byte[] document)
		{
			if (document.Length > 65535)
			{
				throw new ArgumentException("Document too large");
			}
			EnsureValidDocumentId(documentId);
			uint num = PageOffsetToPageIndex(documentId);
			uint numPages = GetNumPages();
			if (num >= numPages)
			{
				throw new ArgumentException("Document ID " + documentId + " not found");
			}
			Page page = ReadPage(num);
			if (page.ReservedDocumentLength >= document.Length)
			{
				OverwritePage(num, ref page, document);
			}
			else
			{
				FreePage(num, ref page);
				documentId = Insert(document, num);
			}
			return documentId;
		}

		public void Remove(uint documentId)
		{
			EnsureValidDocumentId(documentId);
			uint num = PageOffsetToPageIndex(documentId);
			uint numPages = GetNumPages();
			if (num >= numPages)
			{
				throw new ArgumentException("Document ID " + documentId + " not found");
			}
			Page page = ReadPage(num);
			FreePage(num, ref page);
		}

		public void Clear()
		{
			documentsStream.SetLength(1L);
			pagesStream.SetLength(1L);
		}

		public void JournaledClear()
		{
			journalWriter.WriteResizeEntry(documentsPath, 1u);
			journalWriter.WriteResizeEntry(pagesPath, 1u);
		}

		public void Delete()
		{
			if (!isDisposed)
			{
				Dispose();
				fileSystem.DeleteFile(documentsPath);
				fileSystem.DeleteFile(pagesPath);
			}
		}

		public bool Contains(uint documentId)
		{
			EnsureValidDocumentId(documentId);
			uint num = PageOffsetToPageIndex(documentId);
			uint numPages = GetNumPages();
			if (num >= numPages)
			{
				return false;
			}
			Page page = ReadPage(num);
			return page.DocumentLength != 0;
		}

		public byte[] Find(uint documentId)
		{
			EnsureValidDocumentId(documentId);
			uint num = PageOffsetToPageIndex(documentId);
			uint numPages = GetNumPages();
			if (num >= numPages)
			{
				return null;
			}
			Page page = ReadPage(num);
			if (page.DocumentLength == 0)
			{
				return null;
			}
			SeekToDocument(page);
			return ReadDocument(page);
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				documentsReader.Close();
				pagesReader.Close();
				memWriter.Close();
				memStream.Dispose();
			}
		}

		private static uint PageIndexToPageOffset(uint index)
		{
			return 1 + index * 8;
		}

		private static uint PageOffsetToPageIndex(uint offset)
		{
			return (offset - 1) / 8u;
		}

		private static void ReadFileHeader(Stream stream)
		{
			int num = stream.ReadByte();
			if (num != 0)
			{
				throw new CorruptionException("Unsupported format version: " + num);
			}
		}

		private uint GetNumPages()
		{
			return (uint)(pagesStream.Length - 1) / 8u;
		}

		private void SeekToDocument(Page page)
		{
			documentsStream.Position = page.DocumentOffset;
		}

		private void SeekToPageIndex(uint index)
		{
			pagesStream.Position = PageIndexToPageOffset(index);
		}

		private byte[] ReadDocument(Page page)
		{
			return documentsReader.ReadBytes(page.DocumentLength);
		}

		private void WriteDocument(Page page, byte[] document)
		{
			journalWriter.WriteWriteEntry(documentsPath, page.DocumentOffset, document);
			documentsStream.Position += document.Length;
		}

		private Page ReadPage()
		{
			Page page = default(Page);
			page.DocumentOffset = pagesReader.ReadUInt32();
			page.DocumentLength = pagesReader.ReadUInt16();
			page.ReservedDocumentLength = pagesReader.ReadUInt16();
			Page result = page;
			if (result.ReservedDocumentLength < result.DocumentLength)
			{
				throw new CorruptionException("Reserved length less than document length");
			}
			return result;
		}

		private Page ReadPage(uint index)
		{
			SeekToPageIndex(index);
			return ReadPage();
		}

		private void WritePage(Page page)
		{
			memWriter.Write(page.DocumentOffset);
			memWriter.Write(page.DocumentLength);
			memWriter.Write(page.ReservedDocumentLength);
			journalWriter.WriteWriteEntry(pagesPath, (uint)pagesStream.Position, memStream.ToArray());
			memStream.Seek(0L, SeekOrigin.Begin);
			memStream.SetLength(0L);
			pagesStream.Position += 8L;
		}

		private void OverwritePage(uint index, ref Page page, byte[] document)
		{
			ushort num = (ushort)document.Length;
			if (page.DocumentLength != num)
			{
				page.DocumentLength = (ushort)document.Length;
				SeekToPageIndex(index);
				WritePage(page);
			}
			SeekToDocument(page);
			WriteDocument(page, document);
		}

		private void OverwritePages(uint startIndex, uint endIndex, uint availableSpace, byte[] document)
		{
			SeekToPageIndex(startIndex);
			Page page = ReadPage();
			Page page2 = default(Page);
			uint num = endIndex - startIndex + 1;
			page.DocumentLength = (ushort)document.Length;
			if (num > 1 && page.DocumentLength < availableSpace)
			{
				ushort reservedDocumentLength = (ushort)(availableSpace - page.DocumentLength);
				page.ReservedDocumentLength = page.DocumentLength;
				page2.ReservedDocumentLength = reservedDocumentLength;
			}
			else
			{
				page2.ReservedDocumentLength = 0;
				page.ReservedDocumentLength = (ushort)availableSpace;
			}
			SeekToDocument(page);
			WriteDocument(page, document);
			SeekToPageIndex(startIndex);
			WritePage(page);
			for (uint num2 = startIndex + 1; num2 < endIndex; num2++)
			{
				WritePage(default(Page));
			}
			WritePage(page2);
		}

		private void FreePage(uint index, ref Page page)
		{
			page.DocumentLength = 0;
			SeekToPageIndex(index);
			WritePage(page);
		}

		private void WritePageAtEnd(byte[] document)
		{
			pagesStream.Position = pagesStream.Length;
			documentsStream.Position = documentsStream.Length;
			Page page = default(Page);
			page.DocumentOffset = (uint)documentsStream.Position;
			page.DocumentLength = (ushort)document.Length;
			page.ReservedDocumentLength = (ushort)document.Length;
			Page page2 = page;
			WritePage(page2);
			WriteDocument(page2, document);
		}

		private uint Insert(byte[] document, uint freedIndex)
		{
			if (document.Length > 65535)
			{
				throw new ArgumentException("Document too large");
			}
			uint num = 0u;
			uint num2 = uint.MaxValue;
			uint numPages = GetNumPages();
			pagesStream.Position = 1L;
			uint num3 = 0u;
			for (uint num4 = numPages; num3 < num4; num3++)
			{
				Page page = ReadPage();
				if (page.DocumentLength == 0 || num3 == freedIndex)
				{
					num += page.ReservedDocumentLength;
					if (num2 == uint.MaxValue)
					{
						if (page.DocumentOffset == 0)
						{
							num = 0u;
							continue;
						}
						num2 = num3;
					}
					if (num >= document.Length)
					{
						if (num2 < num3)
						{
							OverwritePages(num2, num3, num, document);
						}
						else
						{
							OverwritePage(num2, ref page, document);
						}
						return PageIndexToPageOffset(num2);
					}
				}
				else
				{
					num = 0u;
					num2 = uint.MaxValue;
				}
			}
			WritePageAtEnd(document);
			return PageIndexToPageOffset(numPages);
		}

		private static void EnsureValidDocumentId(uint documentId)
		{
			if (documentId == 0 || (documentId - 1) % 8u != 0)
			{
				throw new ArgumentException("Invalid document ID: " + documentId);
			}
		}
	}
}
