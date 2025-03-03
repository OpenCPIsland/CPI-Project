namespace ClubPenguin.Tags
{
	public abstract class BaseRecursiveTagMatcher : BaseTagMatcher, ITagMatcher
	{
		public abstract BaseTagMatcher[] Matchers
		{
			get;
		}

		public override bool isMatch(TagsArray[] tagCollections, object rawData = null)
		{
			switch (MatchType)
			{
			case MatchType.ALL:
				return base.isMatch(tagCollections, rawData) && matchersMatch(tagCollections, rawData);
			case MatchType.ANY:
				return base.isMatch(tagCollections, rawData) || matchersMatch(tagCollections, rawData);
			default:
				return false;
			}
		}

		protected bool matchersMatch(TagsArray[] tagCollections, object rawData)
		{
			if (Matchers != null && Matchers.Length > 0)
			{
				int num = Matchers.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag = Matchers[i].isMatch(tagCollections, rawData);
					switch (MatchType)
					{
					case MatchType.ALL:
						if (!flag)
						{
							return false;
						}
						break;
					case MatchType.ANY:
						if (flag)
						{
							return true;
						}
						break;
					}
				}
			}
			return defaultMatchingValue(MatchType);
		}
	}
}
