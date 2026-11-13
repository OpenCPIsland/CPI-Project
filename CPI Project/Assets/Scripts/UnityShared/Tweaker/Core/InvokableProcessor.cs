using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class InvokableProcessor : IAttributeScanProcessor<InvokableAttribute, IInvokable>, IScanProcessor<InvokableAttribute, IInvokable>, IScanResultProvider<IInvokable>
	{
		private readonly TweakerOptions options;

		public event EventHandler<ScanResultArgs<IInvokable>> ResultProvided;

		public InvokableProcessor(TweakerOptions options)
		{
			this.options = options;
		}

		public void ProcessAttribute(InvokableAttribute input, Type type, IBoundInstance instance = null)
		{
			MemberInfo[] members = type.GetMembers(ReflectionUtil.GetBindingFlags(instance));
			foreach (MemberInfo memberInfo in members)
			{
				if ((memberInfo.MemberType == MemberTypes.Method || memberInfo.MemberType == MemberTypes.Event) && memberInfo.GetCustomAttributes(typeof(InvokableAttribute), false).Length == 0)
				{
					InvokableAttribute input2 = new InvokableAttribute(input.Name + "." + memberInfo.Name);
					ProcessAttribute(input2, memberInfo, instance);
				}
			}
		}

		public void ProcessAttribute(InvokableAttribute input, MemberInfo memberInfo, IBoundInstance instance = null)
		{
			IInvokable invokable = null;
			if (memberInfo.MemberType == MemberTypes.Method)
			{
				MethodInfo methodInfo = (MethodInfo)memberInfo;
				invokable = InvokableFactory.MakeInvokable(input, methodInfo, instance);
			}
			else
			{
				if (memberInfo.MemberType != MemberTypes.Event)
				{
					throw new ProcessorException("InvokableProcessor cannot process non MethodInfo or EventInfo types");
				}
				EventInfo eventInfo = (EventInfo)memberInfo;
				invokable = InvokableFactory.MakeInvokable(input, eventInfo, instance);
			}
			if (invokable != null)
			{
				ProvideResult(invokable);
			}
		}

		private void ProvideResult(IInvokable invokable)
		{
			IsTestAttribute customAttribute = invokable.GetCustomAttribute<IsTestAttribute>();
			if (((options.Flags & TweakerOptionFlags.IncludeTests) != 0 || customAttribute == null) && this.ResultProvided != null)
			{
				this.ResultProvided(this, new ScanResultArgs<IInvokable>(invokable));
			}
		}
	}
}
