using ClubPenguin.Net.Client.Event;

namespace ClubPenguin.Net
{
	public interface IPlayerActionService : INetworkService
	{
		void LocomotionAction(LocomotionActionEvent action, bool droppable = false);
	}
}
