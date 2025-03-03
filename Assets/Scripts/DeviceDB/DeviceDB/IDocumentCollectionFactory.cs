namespace DeviceDB
{
	public interface IDocumentCollectionFactory
	{
		IDocumentCollection<TDocument> Create<TDocument>(string dirPath, byte[] key) where TDocument : AbstractDocument, new();

		IDocumentCollection<TDocument> CreateHighSecurityFileSystemCollection<TDocument>(string dirPath, byte[] key) where TDocument : AbstractDocument, new();
	}
}
