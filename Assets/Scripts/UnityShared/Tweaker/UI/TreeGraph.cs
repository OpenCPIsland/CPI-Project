namespace Tweaker.UI
{
	public class TreeGraph<TValue>
	{
		public TValue Root
		{
			get;
			private set;
		}

		public TreeGraph(TValue root)
		{
			Root = root;
		}
	}
}
