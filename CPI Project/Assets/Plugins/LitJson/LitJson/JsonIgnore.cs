using System;

namespace LitJson
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class JsonIgnore : Attribute
	{
		public JsonIgnoreWhen Usage
		{
			get;
			private set;
		}

		public JsonIgnore(JsonIgnoreWhen usage = JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)
		{
			Usage = usage;
		}
	}
}
