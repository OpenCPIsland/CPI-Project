using System;
using System.Reflection;
using Tweaker.Core;

namespace Tweaker.UI
{
	public class InvokableNode : BaseNode
	{
		private ITweakable[] argTweakables;

		public IInvokable Invokable
		{
			get;
			private set;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.Invokable;
			}
		}

		public InvokableNode(IInvokable invokable)
			: base((invokable != null) ? invokable.Name : "Virtual")
		{
			Invokable = invokable;
			if (invokable != null)
			{
				BindArgsToTweakables();
			}
		}

		private void BindArgsToTweakables()
		{
			base.Children.Add(new InvokableNode(null));
			Type[] parameterTypes = Invokable.ParameterTypes;
			argTweakables = new ITweakable[parameterTypes.Length];
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				Type tweakableType = parameterTypes[i];
				ParameterInfo parameterInfo = Invokable.Parameters[i];
				object virtualFieldRef;
				ITweakable tweakable = TweakableFactory.MakeTweakable(tweakableType, parameterInfo.Name, Invokable.InvokableInfo.ArgDescriptions[i], parameterInfo.GetCustomAttributes(typeof(Attribute), true) as Attribute[], out virtualFieldRef);
				argTweakables[i] = tweakable;
				if (parameterInfo.IsOptional && (parameterInfo.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault && !(parameterInfo.DefaultValue is DBNull))
				{
					tweakable.SetValue(parameterInfo.DefaultValue);
				}
				base.Children.Add(new TweakableNode(tweakable, virtualFieldRef));
			}
		}

		public void Invoke()
		{
			object[] array = new object[argTweakables.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = argTweakables[i].GetValue();
			}
			Invokable.Invoke(array);
		}
	}
}
