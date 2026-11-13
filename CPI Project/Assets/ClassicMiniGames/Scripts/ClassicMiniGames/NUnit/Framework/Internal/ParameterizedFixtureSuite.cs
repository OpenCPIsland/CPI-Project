using System;

namespace NUnit.Framework.Internal
{
	public class ParameterizedFixtureSuite : TestSuite
	{
		private Type type;

		public Type ParameterizedType
		{
			get
			{
				return type;
			}
		}

		public override string TestType
		{
			get
			{
				if (ParameterizedType.ContainsGenericParameters)
				{
					return "GenericFixture";
				}
				return "ParameterizedFixture";
			}
		}

		public ParameterizedFixtureSuite(Type type)
			: base(type.Namespace, TypeHelper.GetDisplayName(type))
		{
			this.type = type;
		}
	}
}
