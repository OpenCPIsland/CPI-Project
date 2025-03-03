namespace Tweaker.UI
{
	public interface IHexGridController
	{
		ITweakerConsoleController Console
		{
			get;
		}

		BaseNode CurrentDisplayNode
		{
			get;
		}

		void DisplayNode(BaseNode nodeToDisplay);
	}
}
