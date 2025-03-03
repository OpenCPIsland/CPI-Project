using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.Net
{
	internal class PlayerActionService : BaseNetworkService, IPlayerActionService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.USER_LOCO_ACTION, onLocomotionAction);
		}

		public void LocomotionAction(LocomotionActionEvent action, bool droppable = false)
		{
			clubPenguinClient.GameServer.TriggerLocomotionAction(action, droppable);
		}

		private void onLocomotionAction(GameServerEvent gameServerEvent, object data)
		{
			LocomotionActionEvent action = (LocomotionActionEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerActionServiceEvents.LocomotionActionReceived(action.SessionId, action));
		}
	}
}
