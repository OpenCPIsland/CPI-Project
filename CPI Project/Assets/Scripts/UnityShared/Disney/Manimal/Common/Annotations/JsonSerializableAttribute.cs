using System;

namespace Disney.Manimal.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class JsonSerializableAttribute : Attribute
	{
		public MappingType Type
		{
			get;
			set;
		}

		public JsonSerializableAttribute(MappingType type)
		{
			Type = type;
		}

		public JsonSerializableAttribute()
			: this(MappingType.Properties)
		{
		}
	}
}
