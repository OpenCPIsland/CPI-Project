namespace DI.Storage
{
	public interface IStorage
	{
		IDocument getDocument(string specification);

		IJSONDocument getJSONDocument(string specification);
	}
}
