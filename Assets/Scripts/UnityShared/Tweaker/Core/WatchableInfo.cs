namespace Tweaker.Core
{
	public abstract class WatchableInfo
	{
		public enum DisplayMode
		{
			Value,
			ValueGraph,
			Delta,
			DeltaGraph
		}

		public string Name
		{
			get;
			private set;
		}

		public DisplayMode Mode
		{
			get;
			private set;
		}

		public WatchableInfo(string name, DisplayMode mode = DisplayMode.Value)
		{
			Name = name;
			Mode = mode;
		}
	}
}
