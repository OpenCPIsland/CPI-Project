using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class InvokeArgNumberException : Exception, ISerializable
	{
		public InvokeArgNumberException(string name, object[] args, Type[] expectedArgTypes)
			: base(string.Format("Invokation of '{0}'){1}) failed. {2} args were provided but {3} args were expected. The expected types are: [{4}]", name, PrettyPrinter.PrintObjectArray(args), args.Length, expectedArgTypes.Length, PrettyPrinter.PrintTypeArray(expectedArgTypes)))
		{
		}
	}
}
