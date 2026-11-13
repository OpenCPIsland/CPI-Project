namespace SwrveUnity.Messaging
{
	public interface ISwrveTriggeredMessageListener
	{
		void OnMessageTriggered(SwrveMessage message);

		void DismissCurrentMessage();
	}
}
