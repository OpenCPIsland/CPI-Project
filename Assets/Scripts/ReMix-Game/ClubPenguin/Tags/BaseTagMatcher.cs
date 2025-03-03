using ClubPenguin.Core;
using System.Collections.Generic;

namespace ClubPenguin.Tags
{
	public abstract class BaseTagMatcher
	{
		public MatchType MatchType;

		public TagDefinition[] Tags;

		public TagCategoryDefinition[] Categories;

		public virtual bool isMatch(TagsArray[] tagCollections, object rawData = null)
		{
			switch (MatchType)
			{
			case MatchType.ALL:
				return tagsMatch(tagCollections) && categoriesMatch(tagCollections);
			case MatchType.ANY:
				return tagsMatch(tagCollections) || categoriesMatch(tagCollections);
			default:
				return false;
			}
		}

		protected bool tagsMatch(TagsArray[] tagCollections)
		{
			if (Tags != null && Tags.Length > 0)
			{
				int num = tagCollections.Length;
				for (int i = 0; i < num; i++)
				{
					if (tagCollectionMatches(tagCollections[i]))
					{
						return true;
					}
				}
				return false;
			}
			return defaultMatchingValue(MatchType);
		}

		private bool categoriesMatch(TagsArray[] tagCollections)
		{
			if (Categories != null && Categories.Length > 0)
			{
				int num = tagCollections.Length;
				for (int i = 0; i < num; i++)
				{
					if (tagCollectionMatchesCategory(tagCollections[i]))
					{
						return true;
					}
				}
				return false;
			}
			return defaultMatchingValue(MatchType);
		}

		private bool tagCollectionMatches(TagsArray collection)
		{
			if (collection.TagDefinitions.Length > 0)
			{
				int num = Tags.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag = collection.Contains(Tags[i]);
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
				return defaultMatchingValue(MatchType);
			}
			return false;
		}

		private bool tagCollectionMatchesCategory(TagsArray collection)
		{
			if (collection.TagDefinitions.Length > 0)
			{
				int num = Categories.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag = collection.ContainsCategory(Categories[i]);
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
				return defaultMatchingValue(MatchType);
			}
			return false;
		}

		protected bool defaultMatchingValue(MatchType type)
		{
			return (type == MatchType.ALL) ? true : false;
		}

		public virtual Dictionary<string, object> GetExportParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			HashSet<string> hashSet = new HashSet<string>();
			List<object> list = new List<object>();
			if (Tags != null)
			{
				TagDefinition[] tags = Tags;
				foreach (TagDefinition tagDefinition in tags)
				{
					hashSet.Add(tagDefinition.Tag);
				}
			}
			ITagMatcher tagMatcher = this as ITagMatcher;
			if (tagMatcher != null && tagMatcher.Matchers != null)
			{
				BaseTagMatcher[] matchers = tagMatcher.Matchers;
				foreach (BaseTagMatcher baseTagMatcher in matchers)
				{
					list.Add(baseTagMatcher.GetExportParameters());
				}
			}
			dictionary.Add("type", MatchType);
			dictionary.Add("tags", new List<string>(hashSet));
			dictionary.Add("matchers", list);
			return dictionary;
		}
	}
}
