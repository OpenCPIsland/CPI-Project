using ClubPenguin.Net.Client.Event;

namespace ClubPenguin.Net.Client
{
	public interface IOfflineRoomRunner
	{
		string RoomName
		{
			get;
		}

		void Start();

		void End();

		void HandleInteraction(LocomotionActionEvent action);
	}
}
