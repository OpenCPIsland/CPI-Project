using ClubPenguin.Core;
using System;

namespace ClubPenguin.Tags
{
	[Serializable]
	public struct TagsArray
	{
		public TagDefinition[] TagDefinitions;

		public TagsArray(TagDefinition[] definitions)
		{
			TagDefinitions = definitions;
		}

		public bool Contains(TagDefinition def)
		{
			int num = TagDefinitions.Length;
			for (int i = 0; i < num; i++)
			{
				if (TagDefinitions[i].Tag == def.Tag)
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsCategory(TagCategoryDefinition def)
		{
			int num = TagDefinitions.Length;
			for (int i = 0; i < num; i++)
			{
				if (TagDefinitions[i].Category.CategoryName == def.CategoryName)
				{
					return true;
				}
			}
			return false;
		}
	}
}
