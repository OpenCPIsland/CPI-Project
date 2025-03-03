using System;
using System.Reflection;

namespace Tweaker.Core
{
	public class StepTweakable<T> : IStepTweakable
	{
		private readonly BaseTweakable<T> baseTweakable;

		private readonly MethodInfo addMethod;

		private readonly MethodInfo subtractMethod;

		public BaseTweakable<T> BaseTweakable
		{
			get
			{
				return baseTweakable;
			}
		}

		public object StepSize
		{
			get
			{
				return baseTweakable.TweakableInfo.StepSize.Size;
			}
		}

		public Type TweakableType
		{
			get
			{
				return typeof(T);
			}
		}

		public StepTweakable(BaseTweakable<T> baseTweakable)
		{
			this.baseTweakable = baseTweakable;
			if (TweakableType.IsPrimitive)
			{
				addMethod = typeof(PrimitiveHelper).GetMethod("Add", BindingFlags.Static | BindingFlags.Public);
				subtractMethod = typeof(PrimitiveHelper).GetMethod("Subtract", BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				addMethod = TweakableType.GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
				subtractMethod = TweakableType.GetMethod("op_Subtraction", BindingFlags.Static | BindingFlags.Public);
			}
			if (addMethod == null)
			{
				throw new StepTweakableInvalidException(baseTweakable.Name, "No 'operator +' could be found on type '" + TweakableType.FullName + "'");
			}
			if (subtractMethod == null)
			{
				throw new StepTweakableInvalidException(baseTweakable.Name, "No 'operator -' could be found on type '" + TweakableType.FullName + "'");
			}
		}

		public object StepNext()
		{
			T val = (T)addMethod.Invoke(null, new object[2]
			{
				(T)baseTweakable.GetValue(),
				StepSize
			});
			baseTweakable.SetValue(val);
			return baseTweakable.GetValue();
		}

		public object StepPrevious()
		{
			T val = (T)subtractMethod.Invoke(null, new object[2]
			{
				(T)baseTweakable.GetValue(),
				StepSize
			});
			baseTweakable.SetValue(val);
			return baseTweakable.GetValue();
		}
	}
}
