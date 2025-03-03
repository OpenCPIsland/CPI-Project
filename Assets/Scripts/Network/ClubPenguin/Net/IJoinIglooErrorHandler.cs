namespace ClubPenguin.Net
{
	public interface IJoinIglooErrorHandler : IBaseNetworkErrorHandler
	{
		void onRoomFull();

		void onNoServerFound();

		void onRoomJoinError();

		void onRoomChanged();

		void onIglooNotAvailable();
	}
}
