using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.Breadcrumbs
{
	[Serializable]
	public class PersistentBreadcrumbTypeDefinitionKey : TypedStaticGameDataKey<PersistentBreadcrumbTypeDefinition, int>
	{
		public PersistentBreadcrumbTypeDefinitionKey(int type)
		{
			Id = type;
		}
	}
}
