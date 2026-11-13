public interface IScreenTransition
{
	void Play(bool forward = true, string callbackMethodName = "OnTransitionEnd");

	void PlayInstant(bool forward = true);
}
