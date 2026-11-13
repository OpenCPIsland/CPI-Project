using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.Net
{
	internal class PrototypeService : BaseNetworkService, IPrototypeService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PROTOTYPE_ACTION, onPrototypeAction);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PROTOTYPE_STATE, onPrototypeState);
		}

		private void onPrototypeState(GameServerEvent gameServerEvent, object data)
		{
			PrototypeState prototypeState = (PrototypeState)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PrototypeServiceEvents.StateChangeEvent(prototypeState.id, prototypeState.data));
		}

		private void onPrototypeAction(GameServerEvent gameServerEvent, object data)
		{
			PrototypeAction prototypeAction = (PrototypeAction)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PrototypeServiceEvents.ActionEvent(prototypeAction.userid, prototypeAction.data));
		}

		public void SetState(object data)
		{
			clubPenguinClient.GameServer.PrototypeSetState(data);
		}

		public void SendAction(object data)
		{
			clubPenguinClient.GameServer.PrototypeSendAction(data);
		}
	}
}
