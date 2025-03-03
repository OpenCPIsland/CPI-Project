using ClubPenguin.Core;

namespace ClubPenguin.Tags
{
	public interface ITaggable
	{
		TagDefinition[] Tags
		{
			get;
			set;
		}
	}
}
