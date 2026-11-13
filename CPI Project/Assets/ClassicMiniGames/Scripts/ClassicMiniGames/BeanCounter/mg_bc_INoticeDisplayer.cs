namespace BeanCounter
{
	public interface mg_bc_INoticeDisplayer
	{
		void ShowMessage(string _message, float _duration);

		void NoticeUpdate(float _deltaTime);
	}
}
