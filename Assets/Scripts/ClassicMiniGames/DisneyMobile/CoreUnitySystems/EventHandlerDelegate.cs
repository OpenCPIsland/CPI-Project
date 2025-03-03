namespace DisneyMobile.CoreUnitySystems
{
	public delegate bool EventHandlerDelegate<T>(T evt) where T : BaseEvent;
}
