namespace ClubPenguin.Net
{
	public interface IBaseNetworkErrorHandler
	{
		void onRequestTimeOut();

		void onGeneralNetworkError();
	}
}
