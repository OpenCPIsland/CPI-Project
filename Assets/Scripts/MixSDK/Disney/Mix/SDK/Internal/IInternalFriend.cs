namespace Disney.Mix.SDK.Internal
{
	public interface IInternalFriend : IFriend
	{
		string Swid
		{
			get;
		}

		void ChangeTrust(bool isTrusted);
	}
}
