using Disney.Mix.SDK;

namespace ClubPenguin.Net
{
	public class Friend
	{
		public readonly IFriend MixFriend;

		public string DisplayName
		{
			get
			{
				return (MixFriend != null) ? MixFriend.DisplayName.Text : null;
			}
		}

		public string Swid
		{
			get
			{
				return (MixFriend != null) ? MixFriend.Id : null;
			}
		}

		public Friend(IFriend friend)
		{
			MixFriend = friend;
		}
	}
}
