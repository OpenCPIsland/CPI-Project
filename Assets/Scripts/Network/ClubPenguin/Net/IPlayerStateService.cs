using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public interface IPlayerStateService : INetworkService
	{
		string GetAccessToken();

		void SetAccessToken(string accessToken);

		void SetSWID(string swid);

		void SetOutfit(PlayerOutfit outfit);

		void SetProfile(Profile profile);

		void SetLocomotionState(LocomotionState state);

		void GetLocalPlayerData(IBaseNetworkErrorHandler errorHandler);

		void GetOtherPlayerDataBySwid(string swid);

		void GetOtherPlayerDataBySwids(IList<string> swids);

		void GetOtherPlayerDataByDisplayName(string displayName);

		void GetOtherPlayerDataByDisplayNames(IList<string> displayNames);

		void GetOtherPlayerDataBySessionId(long sessionId);

		void GetOnlinePlayersBySwids(IList<string> swids);

		void EquipDispensable(int id);

		void EquipDurable(int propId);

		void DequipHeldObject();

		void MigrateLegacy(CPIDCredentials cpidCreds);

		void SelectTube(int id);

		void SetReferral(string referrer);

		void SetAirBubble(float value, int diveState);

		void RemoveAirBubble();

		void SetAwayFromKeyboard(int value);

		void SetTemporaryHeadStatus(int value);
	}
}
