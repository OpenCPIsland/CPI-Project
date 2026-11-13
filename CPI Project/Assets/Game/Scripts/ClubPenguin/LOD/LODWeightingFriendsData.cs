using UnityEngine;

namespace ClubPenguin.LOD
{
	[CreateAssetMenu(menuName = "LOD/Rules/Friends")]
	public class LODWeightingFriendsData : LODWeightingData
	{
		[HelpBox("This weights Requests based on whether the remote player is a friend.", 25f)]
		public float FriendWeighting;

		[HelpBox("This weights Requests if there is a friend request pending for the remote player.", 25f)]
		public float PendingFriendWeighting;

		public override void InstantiateRequest(GameObject entity)
		{
			entity.AddComponent<LODWeightingFriends>().Data = this;
		}
	}
}
