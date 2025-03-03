using System;
using System.Reflection;

namespace Tweaker.Core
{
	public abstract class BaseInvokable : TweakerObject, IInvokable, ITweakerObject
	{
		private Type declaringType;

		private Type[] parameterTypes;

		public InvokableInfo InvokableInfo
		{
			get;
			private set;
		}

		public IInvokableManager Manager
		{
			get;
			set;
		}

		public abstract string MethodSignature
		{
			get;
		}

		public Type DeclaringType
		{
			get
			{
				return declaringType;
			}
		}

		public Type[] ParameterTypes
		{
			get
			{
				if (parameterTypes != null)
				{
					return parameterTypes.Clone() as Type[];
				}
				return new Type[0];
			}
		}

		public ParameterInfo[] Parameters
		{
			get;
			private set;
		}

		public BaseInvokable(InvokableInfo info, Type declaringType, Assembly assembly, WeakReference instance, bool isPublic, ParameterInfo[] parameters)
			: this(info, declaringType, assembly, instance, isPublic)
		{
			this.declaringType = declaringType;
			SetParameters(parameters);
		}

		public BaseInvokable(InvokableInfo info, Type declaringType, Assembly assembly, WeakReference instance, bool isPublic)
			: base(info, assembly, instance, isPublic)
		{
			this.declaringType = declaringType;
			InvokableInfo = info;
		}

		protected void SetParameters(ParameterInfo[] parameters)
		{
			Parameters = parameters;
			parameterTypes = new Type[Parameters.Length];
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				parameterTypes[i] = Parameters[i].ParameterType;
			}
		}

		public object Invoke(params object[] args)
		{
			if (CheckInstanceIsValid())
			{
				CheckArgsAreValid(args);
				try
				{
					return DoInvoke(args);
				}
				catch (Exception inner)
				{
					throw new InvokeException(base.Name, args, inner);
				}
			}
			return null;
		}

		protected abstract object DoInvoke(object[] args);

		private void CheckArgsAreValid(object[] args)
		{
			if (parameterTypes == null)
			{
				return;
			}
			if (args == null)
			{
				args = new object[0];
			}
			if (args.Length != parameterTypes.Length)
			{
				throw new InvokeArgNumberException(base.Name, args, parameterTypes);
			}
			Type[] array = new Type[args.Length];
			int i;
			for (i = 0; i < args.Length; i++)
			{
				if (args[i] != null)
				{
					array[i] = args[i].GetType();
				}
			}
			i = 0;
			while (true)
			{
				if (i >= args.Length)
				{
					return;
				}
				if (args[i] != null && !parameterTypes[i].IsAssignableFrom(array[i]))
				{
					throw new InvokeArgTypeException(base.Name, args, array, parameterTypes, "Target arg is not assignable from the provided arg.");
				}
				if (args[i] == null && parameterTypes[i].IsValueType)
				{
					args[i] = Activator.CreateInstance(parameterTypes[i]);
					if (args[i] == null)
					{
						break;
					}
				}
				i++;
			}
			throw new InvokeArgTypeException(base.Name, args, array, parameterTypes, string.Format("Could not construct an instance of value type parameter {0}.", parameterTypes[i].FullName));
		}

		protected override bool CheckInstanceIsValid()
		{
			if (!base.CheckInstanceIsValid())
			{
				Manager.UnregisterInvokable(this);
				return false;
			}
			return true;
		}
	}
}
