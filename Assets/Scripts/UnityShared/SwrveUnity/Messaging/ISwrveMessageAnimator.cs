namespace SwrveUnity.Messaging
{
	public interface ISwrveMessageAnimator
	{
		void InitMessage(SwrveMessageFormat format);

		void AnimateMessage(SwrveMessageFormat format);

		void AnimateButton(SwrveButton button);

		void AnimateButtonPressed(SwrveButton button);

		bool IsMessageDismissed(SwrveMessageFormat format);
	}
}
