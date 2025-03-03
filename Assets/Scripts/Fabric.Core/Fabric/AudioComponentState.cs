namespace Fabric
{
	public enum AudioComponentState
	{
		WaitingToPlay,
		Playing,
		WaitingToStop,
		ScheduledToStop,
		Stopped,
		Paused,
		Virtual,
		LostFocus
	}
}
