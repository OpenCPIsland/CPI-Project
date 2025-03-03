using System;

namespace Tweaker.UI.Testbed
{
	public abstract class CustomTypeSerializer
	{
		public ITweakerSerializer BaseSerializer
		{
			get;
			private set;
		}

		public Type CustomType
		{
			get;
			private set;
		}

		public CustomTypeSerializer(ITweakerSerializer baseSerializer, Type customType)
		{
			BaseSerializer = baseSerializer;
			CustomType = customType;
		}

		public abstract string Serialize(object objectToSerialize);

		public abstract object Deserialize(string stringToDeserialize);
	}
}
