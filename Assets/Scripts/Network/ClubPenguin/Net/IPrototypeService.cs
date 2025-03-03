namespace ClubPenguin.Net
{
	public interface IPrototypeService : INetworkService
	{
		void SetState(object data);

		void SendAction(object data);
	}
}
