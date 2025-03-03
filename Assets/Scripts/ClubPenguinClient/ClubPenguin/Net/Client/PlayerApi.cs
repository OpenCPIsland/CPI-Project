using ClubPenguin.Net.Client.Operations;
using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public class PlayerApi
	{
		private ClubPenguinClient clubPenguinClient;

		public PlayerApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<SetOutfitOperation> SetOutfit(PlayerOutfit outfit)
		{
			SetOutfitOperation operation = new SetOutfitOperation(outfit);
			return new APICall<SetOutfitOperation>(clubPenguinClient, operation);
		}

		public APICall<SetProfileOperation> SetProfile(Profile profile)
		{
			SetProfileOperation operation = new SetProfileOperation(profile);
			return new APICall<SetProfileOperation>(clubPenguinClient, operation);
		}

		public APICall<GetLocalPlayerDataOperation> GetLocalPlayerData()
		{
			GetLocalPlayerDataOperation operation = new GetLocalPlayerDataOperation();
			return new APICall<GetLocalPlayerDataOperation>(clubPenguinClient, operation);
		}

		public APICall<GetOtherPlayerDataBySwidOperation> GetOtherPlayerDataBySwid(string swid)
		{
			GetOtherPlayerDataBySwidOperation operation = new GetOtherPlayerDataBySwidOperation(swid);
			return new APICall<GetOtherPlayerDataBySwidOperation>(clubPenguinClient, operation);
		}

		public APICall<GetOtherPlayerDataBySwidsOperation> GetOtherPlayerDataBySwids(IList<string> swids)
		{
			GetOtherPlayerDataBySwidsOperation operation = new GetOtherPlayerDataBySwidsOperation(swids);
			return new APICall<GetOtherPlayerDataBySwidsOperation>(clubPenguinClient, operation);
		}

		public APICall<GetOtherPlayerDataByDisplayNameOperation> GetOtherPlayerDataByDisplayName(string displayName)
		{
			GetOtherPlayerDataByDisplayNameOperation operation = new GetOtherPlayerDataByDisplayNameOperation(displayName);
			return new APICall<GetOtherPlayerDataByDisplayNameOperation>(clubPenguinClient, operation);
		}

		public APICall<GetOtherPlayerDataByDisplayNamesOperation> GetOtherPlayerDataByDisplayNames(IList<string> displayNames)
		{
			GetOtherPlayerDataByDisplayNamesOperation operation = new GetOtherPlayerDataByDisplayNamesOperation(displayNames);
			return new APICall<GetOtherPlayerDataByDisplayNamesOperation>(clubPenguinClient, operation);
		}

		public APICall<GetOtherPlayerDataBySessionIdOperation> GetOtherPlayerDataBySessionId(long sessionId)
		{
			GetOtherPlayerDataBySessionIdOperation operation = new GetOtherPlayerDataBySessionIdOperation(sessionId);
			return new APICall<GetOtherPlayerDataBySessionIdOperation>(clubPenguinClient, operation);
		}

		public APICall<GetOnlinePlayersBySwidsOperation> GetOnlinePlayersBySwids(IList<string> swids)
		{
			GetOnlinePlayersBySwidsOperation operation = new GetOnlinePlayersBySwidsOperation(swids);
			return new APICall<GetOnlinePlayersBySwidsOperation>(clubPenguinClient, operation);
		}

		public APICall<MigrateLegacyAccountOperation> MigrateLegacyAccount(CPIDCredentials cpidCreds)
		{
			MigrateLegacyAccountOperation operation = new MigrateLegacyAccountOperation(cpidCreds);
			return new APICall<MigrateLegacyAccountOperation>(clubPenguinClient, operation);
		}

		public APICall<GetPreregistrationDataOperation> GetPreregistrationData()
		{
			GetPreregistrationDataOperation operation = new GetPreregistrationDataOperation();
			return new APICall<GetPreregistrationDataOperation>(clubPenguinClient, operation);
		}

		public APICall<PreregisterOperation> Preregister(string clientId)
		{
			PreregisterOperation operation = new PreregisterOperation(clientId);
			return new APICall<PreregisterOperation>(clubPenguinClient, operation);
		}

		public APICall<SetReferralOperation> SetReferral(string referrerDisplayName)
		{
			SetReferralOperation operation = new SetReferralOperation(referrerDisplayName);
			return new APICall<SetReferralOperation>(clubPenguinClient, operation);
		}
	}
}
