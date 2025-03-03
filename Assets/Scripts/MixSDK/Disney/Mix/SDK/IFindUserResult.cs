namespace Disney.Mix.SDK
{
	public interface IFindUserResult
	{
		bool Success
		{
			get;
		}

		IUnidentifiedUser User
		{
			get;
		}
	}
}
