namespace DI.Storage
{
	public interface IDocument
	{
		string getReference();

		string getName();

		string getContents();

		byte[] getData();
	}
}
