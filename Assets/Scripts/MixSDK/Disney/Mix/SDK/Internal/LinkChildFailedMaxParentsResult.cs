namespace Disney.Mix.SDK.Internal
{
	public class LinkChildFailedMaxParentsResult : ILinkChildFailedMaxParentsResult, ILinkChildResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}
	}
}
