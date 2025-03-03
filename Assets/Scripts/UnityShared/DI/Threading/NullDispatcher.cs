namespace DI.Threading
{
	public class NullDispatcher : DispatcherBase
	{
		public static NullDispatcher Null = new NullDispatcher();

		protected override void CheckAccessLimitation()
		{
		}

		internal override void AddTask(Task task)
		{
			task.DoInternal();
		}
	}
}
