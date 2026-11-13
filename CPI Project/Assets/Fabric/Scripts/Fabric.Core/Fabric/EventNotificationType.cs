namespace Fabric
{
	public enum EventNotificationType
	{
		OnFinished,
		OnStolenOldest,
		OnStolenNewest,
		OnStolenFarthest,
		OnAudioComponentStopped,
		OnSequenceNextEntry,
		OnSequenceAdvance,
		OnSequenceEnd,
		OnSequenceLoop,
		OnSwitch,
		OnMarker,
		OnRegionSet,
		OnRegionQueued,
		OnRegionEnd,
		OnAudioComponentPlay,
		OnPlay,
		OnPlayNotHandled,
		OnStolenNone
	}
}
