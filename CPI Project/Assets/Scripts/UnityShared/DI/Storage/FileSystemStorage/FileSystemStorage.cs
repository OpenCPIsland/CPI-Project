using DI.JSON;
using System;
using System.IO;

namespace DI.Storage.FileSystemStorage
{
	public class FileSystemStorage : IStorage
	{
		private FileSystemStorageFactory factory;

		public FileSystemStorage(FileSystemStorageFactory factory)
		{
			this.factory = factory;
		}

		public IDocument getDocument(string specification)
		{
			return getFileDocument(specification);
		}

		public IJSONDocument getJSONDocument(string specification)
		{
			IJSONParser parser = factory.getParser();
			if (parser == null)
			{
				throw new StorageException("No JSON parser available for document creation.");
			}
			IDocument fileDocument = getFileDocument(specification);
			if (fileDocument == null)
			{
				return null;
			}
			return new JSONDocument(fileDocument, parser);
		}

		protected IDocument getFileDocument(string path)
		{
			byte[] fileContents = getFileContents(path);
			if (fileContents == null)
			{
				return null;
			}
			string text = path;
			if (text.EndsWith("/"))
			{
				text = text.Substring(0, text.Length - 1);
			}
			int num = text.LastIndexOf('/');
			if (num != -1 && num != text.Length - 1)
			{
				text = path.Substring(num + 1);
			}
			return new ByteDocument(path, text, fileContents);
		}

		protected byte[] getFileContents(string path)
		{
			try
			{
				return File.ReadAllBytes(path);
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}
