namespace Fabric
{
	public enum EventStatus
	{
		Idle,
		InQueue,
		Handled,
		Handled_Virtualized,
		Not_Handled,
		Not_Handled_MinimumPlaybackInterval,
		Not_Handled_Probability,
		Not_Handled_VolumeThreshold,
		Not_Handled_PostCountMax,
		Failed_Uknown,
		Failed_Invalid_Instance,
		Failed_No_Listeners,
		Failed_Invalid_GameObject,
		Failed_SetProperty,
		Failed_MaxInstancesLimit,
		Failed_MaxAudioComponentsLimit,
		Failed_NotEnoughVirtualEvents,
		Not_Handled_Stopped
	}
}
