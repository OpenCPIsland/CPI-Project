using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Api
{
	public interface IParameterDataSource
	{
		IEnumerable GetData(ParameterInfo parameter);
	}
}
