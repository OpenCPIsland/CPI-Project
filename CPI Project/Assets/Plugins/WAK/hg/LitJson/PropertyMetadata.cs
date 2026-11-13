using System;
using System.Reflection;

namespace hg.LitJson
{
	internal struct PropertyMetadata
	{
		public MemberInfo Info;

		public bool IsField;

		public Type Type;

		public string JsonPropertyName;
	}
}
