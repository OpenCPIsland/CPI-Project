using System;

namespace UnityTest
{
	public abstract class ActionBaseGeneric<T> : ActionBase
	{
		protected override bool UseCache
		{
			get
			{
				return true;
			}
		}

		protected override bool Compare(object objVal)
		{
			return Compare((T)objVal);
		}

		protected abstract bool Compare(T objVal);

		public override Type[] GetAccepatbleTypesForA()
		{
			return new Type[1]
			{
				typeof(T)
			};
		}

		public override Type GetParameterType()
		{
			return typeof(T);
		}
	}
}
