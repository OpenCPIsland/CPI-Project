namespace ClubPenguin.Core
{
	public interface IScreenContainerStateHandler
	{
		bool IsKeyboardShown
		{
			get;
		}

		bool IsOpen
		{
			get;
		}
	}
}
