using Disney.Mix.SDK;

namespace ClubPenguin.Net
{
	public class SearchedUser
	{
		public readonly IUnidentifiedUser MixSearchedUser;

		public string DisplayName
		{
			get
			{
				return (MixSearchedUser != null) ? MixSearchedUser.DisplayName.Text : null;
			}
		}

		public SearchedUser(IUnidentifiedUser User)
		{
			MixSearchedUser = User;
		}
	}
}
