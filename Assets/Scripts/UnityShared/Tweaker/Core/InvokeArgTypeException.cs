using System;
using System.Runtime.Serialization;

namespace Tweaker.Core
{
	public class InvokeArgTypeException : Exception, ISerializable
	{
		public InvokeArgTypeException(string name, object[] args, Type[] argTypes, Type[] expectedArgTypes, string extraMessage = "")
			: base(string.Format("Invokation of '{0}({1})' failed. The expected arg types are [{2}] but [{3}] was provided. {4}", name, PrettyPrinter.PrintObjectArray(args), PrettyPrinter.PrintTypeArray(expectedArgTypes), PrettyPrinter.PrintTypeArray(argTypes), extraMessage))
		{
		}
	}
}
