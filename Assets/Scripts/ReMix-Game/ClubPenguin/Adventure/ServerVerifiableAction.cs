namespace ClubPenguin.Adventure
{
	public interface ServerVerifiableAction
	{
		string GetVerifiableType();

		object GetVerifiableParameters();
	}
}
