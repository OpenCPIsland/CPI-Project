namespace ClubPenguin
{
	public class CatalogItemsRequest
	{
		public const string FriendCachePrefix = "Friend";

		public const string RecentCachePrefix = "Recent";

		public const string PopularCachePrefix = "Popular";

		protected string CacheId;

		public static string GetPrefixedCacheId(string prefix, long id)
		{
			return prefix + "_" + id;
		}
	}
}
