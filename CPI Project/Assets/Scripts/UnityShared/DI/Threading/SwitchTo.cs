namespace DI.Threading
{
	public class SwitchTo
	{
		public enum TargetType
		{
			Main,
			Thread
		}

		public static readonly SwitchTo MainThread = new SwitchTo(TargetType.Main);

		public static readonly SwitchTo Thread = new SwitchTo(TargetType.Thread);

		public TargetType Target
		{
			get;
			private set;
		}

		private SwitchTo(TargetType target)
		{
			Target = target;
		}
	}
}
