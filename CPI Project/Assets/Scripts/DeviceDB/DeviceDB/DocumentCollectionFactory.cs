namespace DeviceDB
{
	public class DocumentCollectionFactory : IDocumentCollectionFactory
	{
		public IDocumentCollection<TDocument> Create<TDocument>(string dirPath, byte[] key) where TDocument : AbstractDocument, new()
		{
			return CreateHighSecurityFileSystemCollection<TDocument>(dirPath, key);
		}

		public IDocumentCollection<TDocument> CreateHighSecurityFileSystemCollection<TDocument>(string dirPath, byte[] key) where TDocument : AbstractDocument, new()
		{
			FileSystem fileSystem = new FileSystem();
			if (!fileSystem.DirectoryExists(dirPath))
			{
				fileSystem.CreateDirectory(dirPath);
			}
			string path = HashedPathGenerator.GetPath(dirPath, "_journal");
			JournalPlayer journalPlayer = new JournalPlayer(path, fileSystem);
			JournalWriter journalWriter = new JournalWriter(path, fileSystem);
			string path2 = HashedPathGenerator.GetPath(dirPath, "_packedFile");
			string path3 = HashedPathGenerator.GetPath(dirPath, "_packedFileMeta");
			PackedFile packedFile = new PackedFile(path2, path3, journalWriter, fileSystem);
			IndexFactory indexFactory = new IndexFactory(dirPath, journalWriter, fileSystem);
			return new DocumentCollection<TDocument>(packedFile, indexFactory, key, journalPlayer, journalWriter);
		}
	}
}
