using System;

namespace Disney.Manimal.Common.Annotations
{
	public class JsonCollectionAttribute : Attribute
	{
		public Type CollectionItemType
		{
			get;
			set;
		}

		public JsonCollectionAttribute(Type collectionItemType)
		{
			CollectionItemType = collectionItemType;
		}
	}
}
