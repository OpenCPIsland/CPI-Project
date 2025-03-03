using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Extensibility
{
	public interface IParameterDataProvider
	{
		bool HasDataFor(ParameterInfo parameter);

		IEnumerable GetDataFor(ParameterInfo parameter);
	}
}
