namespace Disney.Kelowna.Common.DataModel
{
	public interface IEntityIdentifierData<T>
	{
		bool Match(T identifier);
	}
}
