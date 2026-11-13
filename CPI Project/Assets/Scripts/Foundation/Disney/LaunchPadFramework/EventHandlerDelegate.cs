namespace Disney.LaunchPadFramework
{
	public delegate bool EventHandlerDelegate<TEvent>(TEvent evt) where TEvent : struct;
}
