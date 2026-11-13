namespace SwrveUnity.Messaging
{
	public interface ISwrveMessageListener
	{
		void OnShow(SwrveMessageFormat format);

		void OnShowing(SwrveMessageFormat format);

		void OnDismiss(SwrveMessageFormat format);
	}
}
