using ClubPenguin.Net;

namespace ClubPenguin.LOD
{
	public class LODWeightingFriends : LODWeightingRule
	{
		public LODWeightingFriendsData Data;

		protected override float UpdateWeighting()
		{
			float result = 0f;
			FriendStatus friendStatus = FriendsDataModelService.GetFriendStatus(request.Data.PenguinHandle);
			if (friendStatus == FriendStatus.Friend)
			{
				result = Data.FriendWeighting;
			}
			else if (friendStatus == FriendStatus.IncomingInvite || friendStatus == FriendStatus.OutgoingInvite)
			{
				result = Data.PendingFriendWeighting;
			}
			return result;
		}
	}
}
