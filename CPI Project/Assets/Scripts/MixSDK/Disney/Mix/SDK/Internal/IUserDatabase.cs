using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IUserDatabase : IDisposable
	{
		IInternalAlert GetAlert(long alertId);

		IList<IInternalAlert> GetAlerts();

		void AddAlert(IInternalAlert alert);

		void RemoveAlert(long alertId);

		FriendDocument[] GetAllFriendDocuments();

		void DeleteFriend(string swid);

		void SetFriendIsTrusted(string swid, bool isTrusted);

		void InsertFriend(FriendDocument doc);

		void ClearFriends();

		void DeleteFriendInvitation(long invitationId);

		void InsertFriendInvitation(FriendInvitationDocument doc);

		void InsertOrUpdateFriendInvitation(FriendInvitationDocument doc);

		void ClearFriendInvitations();

		FriendInvitationDocument[] GetFriendInvitationDocuments(bool isInviter);

		UserDocument GetUserBySwid(string swid);

		UserDocument GetUserByDisplayName(string displayName);

		void InsertUserDocument(UserDocument userDocument);

		void UpdateUserDocument(UserDocument userDocument);

		void PersistUser(string swid, string hashedSwid, string displayName, string firstName, string status);

		void SyncToGetStateResponse(GetStateResponse response, Action callback);
	}
}
