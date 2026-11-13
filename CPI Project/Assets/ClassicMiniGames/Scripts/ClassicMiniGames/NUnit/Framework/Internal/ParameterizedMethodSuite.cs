using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class ParameterizedMethodSuite : TestSuite
	{
		private MethodInfo method;

		public MethodInfo Method
		{
			get
			{
				return method;
			}
		}

		public override string TestType
		{
			get
			{
				if (Method.ContainsGenericParameters)
				{
					return "GenericMethod";
				}
				return "ParameterizedMethod";
			}
		}

		public ParameterizedMethodSuite(MethodInfo method)
			: base(method.ReflectedType.FullName, method.Name)
		{
			this.method = method;
			maintainTestOrder = true;
		}
	}
}
