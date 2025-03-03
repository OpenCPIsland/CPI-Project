using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Breadcrumbs
{
	[Serializable]
	public class StaticBreadcrumbDefinitionKey : TypedStaticGameDataKey<StaticBreadcrumbDefinition, string>
	{
		public StaticBreadcrumbDefinitionKey(string id)
		{
			Id = id;
		}
	}
}
