using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class CastFishingRodSequence
	{
		private ClubPenguinClient clubPenguinClient;

		private ICastFishingRodErrorHandler errorHandler;

		public CastFishingRodSequence(ClubPenguinClient client, ICastFishingRodErrorHandler errorHandler)
		{
			clubPenguinClient = client;
			this.errorHandler = errorHandler;
		}

		public void CastFishingRod()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.STATE_SIGNED, onStateSigned);
			clubPenguinClient.GameServer.GetSignedStateOfLocalPlayer();
		}

		private void onStateSigned(GameServerEvent gameServerEvent, object data)
		{
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.STATE_SIGNED, onStateSigned);
			SignedResponse<InWorldState> state = (SignedResponse<InWorldState>)data;
			APICall<FishingCastOperation> aPICall = clubPenguinClient.MinigameApi.FishingCast(state);
			aPICall.OnComplete += onFishingCastComplete;
			aPICall.OnError += onFishingCastError;
			aPICall.Execute();
		}

		private void onFishingCastError(FishingCastOperation operation, HttpResponse response)
		{
			NetworkErrorService.OnError(response, errorHandler, onFishingCastErrorMapper);
		}

		private static bool onFishingCastErrorMapper(NetworkErrorType errorType, ICastFishingRodErrorHandler handler)
		{
			if (errorType == NetworkErrorType.INPUT_BAD_REQUEST)
			{
				handler.onBadFishingState();
				return true;
			}
			return false;
		}

		private void onFishingCastComplete(FishingCastOperation operation, HttpResponse response)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new MinigameServiceEvents.FishingResultRecieved(operation.SignedFishingResult));
		}
	}
}
